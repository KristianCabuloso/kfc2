using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] Weapon weaponPrefab;

    void OnTriggerEnter(Collider c)
    {
        WeaponController wc = c.GetComponentInParent<WeaponController>();
        if (wc && wc.entity.IsOwner)
        {
            wc.AddWeapon(weaponPrefab);
            Destroy(gameObject);
        }
    }
}
