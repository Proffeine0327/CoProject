using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [Header("Ref")]
    [SerializeField] private GameObject bulletPrefeb;
    [SerializeField] private Transform hSpawnPos;
    [SerializeField] private Transform vSpawnPos;
    [Header("Var")]
    [SerializeField] private int maxBulletAmount;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float reloadTime;
    [SerializeField] private float shootTime;
    [SerializeField] private bool isSingleShot;

    private int curBulletAmount;
    private float curShootTime;
    private bool isReloading;

    private void OnDisable() 
    {
        isReloading = false;
    }

    private void Start() 
    {
        curBulletAmount = maxBulletAmount;    
    }

    public override void Attack()
    {
        if (isReloading) return;
        if (curShootTime > 0) return;

        if(isSingleShot) if(!Input.GetKeyDown(KeyCode.A)) return;

        GameObject bullet;

        if (direction is PlayerDirection.left or PlayerDirection.right) bullet = Instantiate(bulletPrefeb, hSpawnPos.position, Quaternion.identity);
        else bullet = Instantiate(bulletPrefeb, vSpawnPos.position, Quaternion.identity);

        switch (direction)
        {
            case PlayerDirection.left: bullet.GetComponent<Bullet>().Init(bulletSpeed, Vector2.left); break;
            case PlayerDirection.right: bullet.GetComponent<Bullet>().Init(bulletSpeed, Vector2.right); break;
            case PlayerDirection.down: bullet.GetComponent<Bullet>().Init(bulletSpeed, Vector2.down); break;
            case PlayerDirection.up: bullet.GetComponent<Bullet>().Init(bulletSpeed, Vector2.up); break;
        }

        curBulletAmount--;
        if(curBulletAmount <= 0) Reload();
        
        curShootTime = shootTime;
    }

    public override void Reload()
    {
        if(isReloading) return;
        if(curBulletAmount == maxBulletAmount) return;

        isReloading = true;
        this.Invoke(() => 
        {
            curBulletAmount = maxBulletAmount;
            isReloading = false;
        }, reloadTime);
    }

    protected override void Update()
    {
        base.Update();

        if (curShootTime > 0) curShootTime -= Time.deltaTime;
    }
}
