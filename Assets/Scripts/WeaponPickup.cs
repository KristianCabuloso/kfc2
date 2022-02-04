using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] Weapon weaponPrefab;

    void OnTriggerEnter(Collider c)
    {
        PlayerCharacterController pcc = c.GetComponentInParent<PlayerCharacterController>();
        if (pcc && pcc.entity.IsOwner)
        {
            pcc.GetComponent<WeaponController>().AddWeapon(weaponPrefab);
            Destroy(gameObject);
        }
    }
}
