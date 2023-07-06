using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bulletPrefeb;
    [SerializeField] private Transform hSpawnPos;
    [SerializeField] private Transform vSpawnPos;

    public override void Attack()
    {
        GameObject bullet;

        if(direction is PlayerDirection.left or PlayerDirection.right) bullet = Instantiate(bulletPrefeb, hSpawnPos.position, Quaternion.identity);
        else bullet = Instantiate(bulletPrefeb, vSpawnPos.position, Quaternion.identity);

        switch(direction)
        {
            case PlayerDirection.left:  bullet.GetComponent<Bullet>().Init(bulletSpeed, Vector2.left); break;
            case PlayerDirection.right: bullet.GetComponent<Bullet>().Init(bulletSpeed, Vector2.right); break;
            case PlayerDirection.down:  bullet.GetComponent<Bullet>().Init(bulletSpeed, Vector2.down); break;
            case PlayerDirection.up:    bullet.GetComponent<Bullet>().Init(bulletSpeed, Vector2.up); break;
        }
    }

    protected override void Update()
    {
        base.Update();
    }
}
