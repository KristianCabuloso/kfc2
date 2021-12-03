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
    }

    void Shoot()
    {
        print("ATIROU");
        Health hitHealth = GetForwardRaycastHitHealth(state.PlayerShotDirection);
        if (hitHealth)
        {
            hitHealth.ReceiveDamage(weapon.Damage);
            print(hitHealth.name + " TOMOU " + weapon.Damage + " DANO");
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
}
