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
    [SerializeField] int spawnBulletConsumeAmount = 1;
    [SerializeField] int bulletConsumeAmount = 1;
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
        if (ammo == 0)
            Recharge();

        if (ammo > 0 && shotCooldownCount <= 0f)
        {
            ammo -= bulletConsumeAmount;
            ammo = Mathf.Max(0, ammo);
            shotCooldownCount = shotCooldown;
            return true;
        }

        return false;
    }

    public void Shoot(Transform shotPoint, PlayerAnalytics playerAnalytics = null)
    {
        Vector3 _position = shotPoint.position;
        Vector3 _rotation = shotPoint.eulerAngles;

        bool canUsePlayerAnalytics = playerAnalytics != null;

        /*Vector3 spForward = shotPoint.forward;
        Vector3 forwardAxis = new Vector3(spForward.y, spForward.x, 0f);*/

        for (float i = -spawnBulletConsumeAmount / 2f; i < spawnBulletConsumeAmount / 2f; i++)
        {
            //Vector3 _rot = _rotation + (forwardAxis * i);
            Projectile p = Instantiate(projectile, _position, Quaternion.Euler(_rotation + Vector3.up * i));
            if (canUsePlayerAnalytics)
                p.playerAnalytics = playerAnalytics;
        }
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
