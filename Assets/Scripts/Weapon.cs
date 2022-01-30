using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Adaptar (talvez) solução para online
public class Weapon : MonoBehaviour
{
    public Sprite targetSprite;
    [SerializeField] int maxAmmo = 10;
    [Tooltip("ammo já inicia (Start()) com maxAmmo"), SerializeField]
    int ammo;
    [SerializeField] int maxRecharges = 10;
    [Tooltip("recharges já inicia (Start()) com maxRecharges"), SerializeField]
    int recharges;
    [SerializeField] bool infinityRecharge;
    [SerializeField] float shotCooldown = 1f;
    [SerializeField] Projectile projectile;

    public Projectile Projectile { private set => projectile = value; get => projectile; }
    //[SerializeField] Transform shotPoint;
    //[SerializeField] int damage = 1;

    //public int Damage { private set => damage = value;  get => damage; }

    float shotCooldownCount;

    void Start()
    {
        ammo = maxAmmo;
        recharges = maxRecharges;
    }

    void Update()
    {
        if (shotCooldownCount > 0f)
            shotCooldownCount -= Time.deltaTime;
    }

    public bool TryConsumeShot()
    {
        if (ammo <= 0)
            Recharge();

        if (ammo > 0 && shotCooldownCount <= 0f)
        {
            ammo--;
            shotCooldownCount = shotCooldown;
            return true;
        }

        return false;
    }

    public void Shoot(Vector3 _position, Quaternion _rotation)
    {
        Instantiate(projectile, _position, _rotation);
    }

    void Recharge()
    {
        if (infinityRecharge || recharges > 0)
        {
            recharges--;
            ammo = maxAmmo;
        }
    }
}
