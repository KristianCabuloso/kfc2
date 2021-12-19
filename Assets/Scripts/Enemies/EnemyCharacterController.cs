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

    BattleManager battleManager;
    Health target;

    EnemyState enemyState;
    Vector3 velocity;
    bool isGrounded;

    public float walkingSpeed = 6f;
    public float gravity = -9.81f;
    public float startFightDistance = 1f;
    public float followTargetToShotDistance = 5f;
    //public float fleeTargetDistance = 1f;
    public Transform baseTransform;

    public Transform groundCheck; //usado para armazenar a posi??o do groundCheck no jogo
    public float groundDistance = 0.4f; // defini o raio de detec??o do algum objeto
    public LayerMask groundMask; // Utilizado para definir se um objeto sera reconhecido como ch?o, para saber se o personagem pode pular/ desacelerar a queda

    void Start()
    {
        controller = GetComponent<CharacterController>();
        weaponController = GetComponent<WeaponController>();
    }

    public override void Attached()
    {
        // Setar o transform do Bolt (online)
        state.SetTransforms(state.PlayerTransform, transform);
        battleManager = FindObjectOfType<BattleManager>();

        Health health = GetComponent<Health>();
        if (health)
            battleManager.NPCs.Add(health);
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

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

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
        Vector3 oldForward = transform.forward;
        Vector3 _rot = transform.eulerAngles;
        transform.eulerAngles = Vector3.up * _rot.y;

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
            //moveDirection = _forward;
        }
        else
        {
            weaponController.TryShoot(oldForward);
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

        controller.Move(transform.forward * walkingSpeed * BoltNetwork.FrameDeltaTime);
    }

    bool TryFindNewTarget()
    {
        Vector3 _position = transform.position;
        Health closerPlayer = null;
        float closerDistance = startFightDistance;

        foreach (Health h in battleManager.players)
        {
            print(h.name);
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
