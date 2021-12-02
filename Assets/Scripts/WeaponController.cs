using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] Transform shotRaycastPoint;
    [SerializeField] LayerMask shotLayerMask;
    [SerializeField] Transform weaponHolder;
    [SerializeField] Weapon[] initialWeapons;

    Weapon weapon;
    public Weapon CurrentWeapon { get => weapon; }

    void Awake()
    {
        weapon = Instantiate(initialWeapons[0], weaponHolder);
    }

    public void TryShoot(Vector3 forward)
    {
        if (weapon.TryShoot())
        {
            print("ATIROU");
            Health hitHealth = GetForwardRaycastHitHealth(forward);
            if (hitHealth)
            {
                hitHealth.ReceiveDamage(weapon.Damage);
                print(hitHealth.name + " TOMOU " + weapon.Damage + " DANO");
            }
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
