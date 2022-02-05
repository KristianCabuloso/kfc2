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
    public string attackAnimationTriggerParameterName = "Attack";

    PlayerAnalytics playerAnalytics;
    Animator animator;

    int oldWeaponIndex;
    Weapon[] weapons;
    //int currentWeaponIndex;
    public Weapon CurrentWeapon { private set; get; }
    // TODO Fazer sistema de troca de armas

    
    void Awake()
    {
        weapons = new Weapon[initialWeapons.Length];
        if (weapons.Length > 0)
        {
            Weapon w = weapons[0] = Instantiate(initialWeapons[0], weaponHolder);
            CurrentWeapon = w;

            for (int i = 1; i < weapons.Length; i++)
            {
                w = Instantiate(initialWeapons[i], weaponHolder);
                w.gameObject.SetActive(false);
                weapons[i] = w;
            }
        }
    }

    public override void Attached()
    {
        state.PlayerWeaponIndex = 0;
        state.OnPlayerShoot = Photon_Shoot;
        state.AddCallback("PlayerPickupWeaponPath", Photon_AddWeapon);
        //state.AddCallback("PlayerWeaponIndex", Photon_SwitchWeapon);
        playerAnalytics = GetComponent<PlayerAnalytics>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!entity.IsOwner)
        {
            int _weaponIndex = state.PlayerWeaponIndex;
            if (oldWeaponIndex != _weaponIndex)
                SwitchWeapon(_weaponIndex, oldWeaponIndex);
        }
    }

    void Photon_Shoot()
    {
        if (animator)
            animator.SetTrigger(attackAnimationTriggerParameterName);
        CurrentWeapon.Shoot(shotPoint, playerAnalytics);
    }

    public void TryShoot()
    {
        if (CurrentWeapon.TryConsumeShot())
        {
            //state.PlayerShotDirection = forward;
            state.PlayerShoot();
        }
    }

    public void SwitchWeapon(bool next)
    {
        int currentWeaponIndex = state.PlayerWeaponIndex;
        int newWeaponIndex = currentWeaponIndex;

        if (next)
        {
            newWeaponIndex++;
            if (newWeaponIndex > weapons.Length - 1)
                newWeaponIndex = 0;
        }
        else
        {
            newWeaponIndex--;
            if (newWeaponIndex < 0)
                newWeaponIndex = weapons.Length - 1;
        }

        SwitchWeapon(newWeaponIndex, currentWeaponIndex);
    }

    public void SwitchWeapon(int weaponIndex, int currentWeaponIndex = -1)
    {
        if (currentWeaponIndex == -1)
            currentWeaponIndex = state.PlayerWeaponIndex;

        if (weaponIndex == currentWeaponIndex || weaponIndex < 0 || weaponIndex > weapons.Length - 1)
            return;
            
        weapons[currentWeaponIndex].gameObject.SetActive(false);

        Weapon w = weapons[weaponIndex];
        w.gameObject.SetActive(true);
        CurrentWeapon = w;

        state.PlayerWeaponIndex = weaponIndex;
        oldWeaponIndex = weaponIndex;
    }

    public void Photon_AddWeapon()
    {
        string _weaponPrefabPath = state.PlayerPickupWeaponPath;

        if (string.IsNullOrEmpty(_weaponPrefabPath))
            return;

        Weapon w = Instantiate(Resources.Load<Weapon>(_weaponPrefabPath), weaponHolder);
        //Weapon w = Instantiate(weapon, weaponHolder);
        w.gameObject.SetActive(false);

        Weapon[] _weapons = new Weapon[weapons.Length + 1];
        for (int i = 0; i < weapons.Length; i++)
            _weapons[i] = weapons[i];

        _weapons[_weapons.Length - 1] = w;
        weapons = _weapons;
    }

    /*public void Photon_SwitchWeapon()
    {
        Debug.LogError("000");
        if (!entity.IsOwner)
            Debug.LogError("111");  SwitchWeapon(state.PlayerWeaponIndex);
    }*/

    public bool CheckForwardTargetHealth()
    {
        RaycastHit hit;
        return Physics.Raycast(shotPoint.position, shotPoint.forward, out hit, float.PositiveInfinity, CurrentWeapon.Projectile.LayerMask);
    }

    /*void OnDrawGizmos()
    {
        Gizmos.DrawRay(shotRaycastPoint.position, transform.forward);
    }*/
}
