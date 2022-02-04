using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class WeaponController : EntityBehaviour<IKFCPlayerState>
{
    int testCounter;

    //[SerializeField] Transform shotRaycastPoint;
    //[SerializeField] LayerMask shotLayerMask;
    [SerializeField] Transform weaponHolder;
    [SerializeField] Weapon[] initialWeapons;
    [SerializeField] Transform shotPoint;
    public string attackAnimationTriggerParameterName = "Attack";

    PlayerAnalytics playerAnalytics;
    Animator animator;

    Weapon[] weapons;
    int currentWeaponIndex;
    public Weapon CurrentWeapon { get => weapons[currentWeaponIndex]; }
    // TODO Fazer sistema de troca de armas

    void Awake()
    {
        weapons = new Weapon[initialWeapons.Length];
        if (weapons.Length > 0)
        {
            weapons[0] = Instantiate(initialWeapons[0], weaponHolder);

            for (int i = 1; i < weapons.Length; i++)
            {
                Weapon w = Instantiate(initialWeapons[i], weaponHolder);
                w.gameObject.SetActive(false);
                weapons[i] = w;
            }
        }
    }

    public override void Attached()
    {
        state.OnPlayerShoot = Photon_Shoot;
        playerAnalytics = GetComponent<PlayerAnalytics>();
        animator = GetComponentInChildren<Animator>();
    }

    void Photon_Shoot()
    {
        if (animator)
            animator.SetTrigger(attackAnimationTriggerParameterName);
        CurrentWeapon.Shoot(shotPoint, playerAnalytics);
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
        if (CurrentWeapon.TryConsumeShot())
        {
            //state.PlayerShotDirection = forward;
            state.PlayerShoot();
        }
    }

    public void SwitchWeapon(bool next)
    {
        CurrentWeapon.gameObject.SetActive(false);

        if (next)
        {
            currentWeaponIndex++;
            if (currentWeaponIndex > weapons.Length - 1)
                currentWeaponIndex = 0;
        }
        else
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
                currentWeaponIndex = weapons.Length - 1;
        }

        CurrentWeapon.gameObject.SetActive(true);
    }

    public void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex == currentWeaponIndex || weaponIndex < 0 || weaponIndex > weapons.Length - 1)
            return;

        CurrentWeapon.gameObject.SetActive(false);
        currentWeaponIndex = weaponIndex;
        CurrentWeapon.gameObject.SetActive(true);
    }

    public void AddWeapon(Weapon weapon)
    {
        Weapon w = Instantiate(weapon, weaponHolder);
        w.gameObject.SetActive(false);

        Weapon[] _weapons = new Weapon[weapons.Length + 1];
        for (int i = 0; i < weapons.Length; i++)
            _weapons[i] = weapons[i];

        _weapons[_weapons.Length - 1] = w;
        weapons = _weapons;
    }

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
