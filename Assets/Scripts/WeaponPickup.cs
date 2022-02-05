using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] string weaponPrefabPath;
    bool waitingForDestroy;

    void OnTriggerEnter(Collider c)
    {
        if (waitingForDestroy)
            return;

        PlayerCharacterController pcc = c.GetComponentInParent<PlayerCharacterController>();
        if (pcc && pcc.entity.IsOwner)
        {
            pcc.state.PlayerPickupWeaponPath = weaponPrefabPath;
            waitingForDestroy = true;
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
                meshRenderer.enabled = false;
            //pcc.GetComponent<WeaponController>().AddWeapon(weaponPrefabPath);
            StartCoroutine(WaitToAnnulateAndDestroy(pcc));
        }
    }

    IEnumerator WaitToAnnulateAndDestroy(PlayerCharacterController pcc)
    {
        yield return new WaitForSeconds(2f);
        pcc.state.PlayerPickupWeaponPath = "";
        Destroy(gameObject);
    }
}
