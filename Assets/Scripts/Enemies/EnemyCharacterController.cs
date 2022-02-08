using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public enum EnemyState : byte
{
    Resting,
    Fighting,
    BackingToBase
}

[RequireComponent(typeof(CharacterController), typeof(BoltEntity))]
public class EnemyCharacterController : EntityBehaviour<IKFCPlayerState>
{
    CharacterController controller;
    WeaponController weaponController;
    Animator animator;

    BattleManager battleManager;
    Health target;
    Collider[] colliders;

    EnemyState enemyState;
    Vector3 velocity;
    bool isGrounded;
    float jumpStartTime;

    public float walkingSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float jumpCooldown = 5f;
    public float jumpYDistanceTrigger = 1.3f;

    public string animationSpeedFloatParameterName = "MoveSpeed";
    public float startFightDistance = 1f;
    public float followTargetToShotDistance = 5f;
    //public float fleeTargetDistance = 1f;
    [SerializeField] Transform enemyHead;
    public Transform baseTransform;

    public Transform groundCheck; //usado para armazenar a posi??o do groundCheck no jogo
    public float groundDistance = 0.4f; // defini o raio de detec??o do algum objeto
    public LayerMask groundMask; // Utilizado para definir se um objeto sera reconhecido como ch?o, para saber se o personagem pode pular/ desacelerar a queda

    void Start()
    {
        controller = GetComponent<CharacterController>();
        weaponController = GetComponent<WeaponController>();
        animator = GetComponentInChildren<Animator>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public override void Attached()
    {
        // Setar o transform do Bolt (online)
        state.SetTransforms(state.PlayerTransform, transform);
        state.SetTransforms(state.PlayerHeadTransform, enemyHead);
        /*if (animator)
            state.SetAnimator(animator);*/

        battleManager = FindObjectOfType<BattleManager>();

        Health health = GetComponent<Health>();
        if (health)
            battleManager.NPCs.Add(health);
    }

    void Update()
    {
        if (animator)
            animator.SetFloat(animationSpeedFloatParameterName, state.EnemySpeed);
    }

    public override void SimulateOwner()
    {
        switch (enemyState)
        {
            case EnemyState.Resting: 
                RestUpdate(); break;

            case EnemyState.Fighting:
                FightUpdate(); break;

            case EnemyState.BackingToBase:
                BackBaseUpdate(); break;
        }

        velocity.y += gravity * BoltNetwork.FrameDeltaTime;

        GroundCheck();

        controller.Move(velocity * BoltNetwork.FrameDeltaTime);
    }

    void RestUpdate()
    {
        if (TryFindNewTarget())
            enemyState = EnemyState.Fighting;
    }

    void FightUpdate()
    {
        if (!target && !TryFindNewTarget())
        {
            enemyState = EnemyState.BackingToBase;
            return;
        }

        Transform targetTransform = target.transform;

        transform.LookAt(targetTransform);

        //Vector3 oldForward = transform.forward;
        Vector3 _rot = transform.eulerAngles;
        transform.eulerAngles = Vector3.up * _rot.y;

        enemyHead.LookAt(targetTransform);

        //Vector3 _forward = transform.forward;

        //Vector3 moveDirection = new Vector3();

        float dist = Vector3.Distance(transform.position, targetTransform.position);

        /*if (dist <= fleeTargetDistance)
        {
            moveDirection = -_forward;
        }
        else */if (dist > followTargetToShotDistance)
        {
            controller.Move(transform.forward * walkingSpeed * BoltNetwork.FrameDeltaTime);

            TryJump(targetTransform.position.y);

            if (animator)
                state.EnemySpeed = walkingSpeed;
            //moveDirection = _forward;
        }
        else
        {
            if (animator)
                state.EnemySpeed = 0;
            weaponController.TryShoot();
        }
    }

    void BackBaseUpdate()
    {
        if (!baseTransform || Vector3.Distance(transform.position, baseTransform.position) <= 1f)
        {
            enemyState = EnemyState.Resting;
            return;
        }

        transform.LookAt(baseTransform);
        Vector3 _rot = transform.eulerAngles;
        transform.eulerAngles = Vector3.up * _rot.y;

        TryJump(baseTransform.position.y);

        controller.Move(transform.forward * walkingSpeed * BoltNetwork.FrameDeltaTime);
    }

    private void GroundCheck()
    {
        Collider[] _colliders = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);

        if (_colliders.Length <= colliders.Length)
        {
            int selfColliders = 0;

            foreach (Collider ca in _colliders)
            {
                foreach (Collider cb in colliders)
                {
                    if (ca == cb)
                    {
                        selfColliders++;
                        break;
                    }
                }
            }

            isGrounded = selfColliders != _colliders.Length;
        }
        else
        {
            isGrounded = _colliders.Length > 0;
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

    }

    void TryJump(float followTargetYPosition)
    {
        if (isGrounded && Time.time - jumpStartTime > jumpCooldown)
        {
            float yDiff = followTargetYPosition - transform.position.y;
            if (Mathf.Abs(yDiff) >= jumpYDistanceTrigger)
            {
                velocity.y = Mathf.Sqrt((Mathf.Max(yDiff, 0) * Random.Range(1f, 20f)) + jumpHeight * -2f * gravity);
                jumpStartTime = Time.time;
            }
        }
    }

    bool TryFindNewTarget()
    {
        Vector3 _position = transform.position;
        Health closerPlayer = null;
        float closerDistance = startFightDistance;

        foreach (Health h in battleManager.players)
        {
            float dist = Vector3.Distance(_position, h.transform.position);
            if (dist < closerDistance)
            {
                closerDistance = dist;
                closerPlayer = h;
            }
        }

        if (closerPlayer)
        {
            target = closerPlayer;
            return true;
        }

        return false;
    }

    public override void Detached()
    {
        Health health = GetComponent<Health>();
        if (health)
            battleManager.NPCs.Remove(health);
    }

    /*void Update()
    {
        transform.Translate(Vector3.forward * walkingSpeed * Time.deltaTime);
        transform.LookAt(player.transform.position);
        
    }*/
}
