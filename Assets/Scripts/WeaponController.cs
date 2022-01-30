using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class WeaponController : EntityBehaviour<IKFCPlayerState>
{
    //[SerializeField] Transform shotRaycastPoint;
    //[SerializeField] LayerMask shotLayerMask;
    [SerializeField] Transform weaponHolder;
    [SerializeField] Weapon[] initialWeapons;
    [SerializeField] Transform shotPoint;

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
        state.OnPlayerShoot = Photon_Shoot;
        playerAnalytics = GetComponent<PlayerAnalytics>();
    }

    void Photon_Shoot()
    {
        weapon.Shoot(shotPoint.position, shotPoint.rotation);
        /*print("ATIROU");
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
        }*/
    }

    public void TryShoot()
    {
        if (weapon.TryConsumeShot())
        {
            //state.PlayerShotDirection = forward;
            state.PlayerShoot();
        }
    }

    public bool CheckForwardTargetHealth()
    {
        RaycastHit hit;
        return Physics.Raycast(shotPoint.position, shotPoint.forward, out hit, float.PositiveInfinity, weapon.Projectile.LayerMask);
    }

    /*void OnDrawGizmos()
    {
        Gizmos.DrawRay(shotRaycastPoint.position, transform.forward);
    }*/
}
