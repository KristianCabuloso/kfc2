using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class WeaponController : EntityBehaviour<IKFCPlayerState>
{
    [SerializeField] Transform shotRaycastPoint;
    [SerializeField] LayerMask shotLayerMask;
    [SerializeField] Transform weaponHolder;
    [SerializeField] Weapon[] initialWeapons;

    PlayerAnalytics playerAnalytics;

    Weapon weapon;
    public Weapon CurrentWeapon { get => weapon; }
    // TODO Fazer sistema de troca de armas

    void Awake()
    {
        weapon = Instantiate(initialWeapons[0], weaponHolder);
    }

    public override void Attached()
    {
        state.OnPlayerShoot = Shoot;
        playerAnalytics = GetComponent<PlayerAnalytics>();
    }

    void Shoot()
    {
        print("ATIROU");
        Health hitHealth = GetForwardRaycastHitHealth(state.PlayerShotDirection);
        if (hitHealth)
        {
            int _damage = weapon.Damage;

            if (playerAnalytics)
            {
                if (hitHealth.ReviveController)
                {
                    playerAnalytics.hittedPlayers++;
                    if (hitHealth.state.PlayerHealth - _damage <= 0)
                        playerAnalytics.kodPlayers++;
                }
                else if (hitHealth.GetComponent<EnemyCharacterController>())
                {
                    playerAnalytics.hittedEnemies++;
                    if (hitHealth.state.PlayerHealth - _damage <= 0)
                        playerAnalytics.killedEnemies++;
                }
            }

            hitHealth.ReceiveDamage(_damage);
            print(hitHealth.name + " TOMOU " + _damage + " DANO");
        }
    }

    public void TryShoot(Vector3 forward)
    {
        if (weapon.TryShoot())
        {
            state.PlayerShotDirection = forward;
            state.PlayerShoot();
        }
    }

    public Health GetForwardRaycastHitHealth(Vector3 forward)
    {
        RaycastHit hit;
        if (Physics.Raycast(shotRaycastPoint.position, forward, out hit, float.PositiveInfinity, shotLayerMask))
            return hit.transform.GetComponentInParent<Health>();

        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(shotRaycastPoint.position, transform.forward);
    }
}
