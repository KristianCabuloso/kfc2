using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    const float REPAIR_SHOT_DIRECTION_DISTANCE_TOLERANCE = 1f;

    [SerializeField] LayerMask layerMask;
    [SerializeField] float speed = 1f;
    [SerializeField] int damage = 1;
    [SerializeField] float size = 1f;
    [SerializeField, Tooltip("Tempo para o projétil se autodestruir. <= 0 desabilita autodestruição")]
    float selfDestructTime = 5f;

    [HideInInspector] public PlayerAnalytics playerAnalytics;

    public LayerMask LayerMask { private set => layerMask = value; get => layerMask; }

    /*Vector3 centerPosition;
    [SerializeField] bool isAligningCenter;*/

    /*public void Setup(Vector3 centerPosition)
    {
        this.centerPosition = centerPosition;
        isAligningCenter = true;
    }*/

    void Start()
    {
        if (selfDestructTime > 0f)
            Destroy(gameObject, selfDestructTime);

        //Debug.Break();
    }

    void Update()
    {
        /*if (isAligningCenter)
            transform.LookAt(centerPosition);*/

        transform.position += transform.forward * speed * BoltNetwork.FrameDeltaTime;

        /*if (isAligningCenter && Vector3.Distance(_pos, centerPosition) <= REPAIR_SHOT_DIRECTION_DISTANCE_TOLERANCE)
            isAligningCenter = false;*/

        Collider[] colliders = Physics.OverlapSphere(transform.position, size, layerMask);

        if (colliders.Length > 0)
        {
            bool usingAnalytics = playerAnalytics != null;

            foreach (Collider c in colliders)
            {
                Health h = c.GetComponentInParent<Health>();
                if (h && h.entity.IsAttached)
                {
                    bool killed = h.ReceiveDamage(damage);

                    if (usingAnalytics)
                    {
                        if (h.ReviveController)
                        {
                            if (killed)
                                playerAnalytics.kodPlayers++;

                            playerAnalytics.hittedPlayers++;
                        }
                        else if (h.GetComponent<EnemyCharacterController>())
                        {
                            if (killed)
                                playerAnalytics.killedEnemies++;

                            playerAnalytics.hittedEnemies++;
                        }
                    }
                }
            }

            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, size * 0.5f);
        //Gizmos.DrawSphere(transform.position, size * 0.5f);
    }
}
