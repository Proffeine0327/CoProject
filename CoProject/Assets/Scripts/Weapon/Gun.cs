using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Transform h;
    [SerializeField] private Transform v;
    [SerializeField] private GameObject bulletPrefeb;
    [SerializeField] private Transform hSpawnPos;
    [SerializeField] private Transform vSpawnPos;
    [Header("Var")]
    [SerializeField] private int maxAmmo;
    [SerializeField] private float reloadTime;
    [SerializeField] private float shootTime;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private bool isSingleShot;
    [Header("Ui")]
    [SerializeField] private Sprite uiDisplaySprite;
    [SerializeField] private string uiDisplayType;

    private PlayerDirection direction;
    private int curAmmo;
    private float curShootTime;
    private bool isReloading;

    public bool IsReloading => isReloading;
    public int CurAmmo => curAmmo;
    public int MaxAmmo => maxAmmo;
    public Sprite UIDisplaySprite => uiDisplaySprite;
    public string UIDisplayType => uiDisplayType;

    private void OnDisable()
    {
        isReloading = false;
    }

    private void Start()
    {
        curAmmo = maxAmmo;
    }

    public void Attack()
    {
        if (curShootTime > 0) return;
        if (isReloading) return;
        if (curAmmo <= 0)
        {
            Reload();
            return;
        }

        if (isSingleShot && !Input.GetKeyDown(KeyCode.A)) return;
        
        float rotation = direction switch 
        {
            PlayerDirection.right => 0,
            PlayerDirection.up => 90,
            PlayerDirection.left => 180,
            PlayerDirection.down => 270,
            _ => 0
        };
        rotation += Random.Range(-1.5f, 1.5f);
        Vector2 dir = new(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad));
        var bullet = 
            Instantiate(
                bulletPrefeb, 
                direction is PlayerDirection.left or PlayerDirection.right ? hSpawnPos.position : vSpawnPos.position, 
                Quaternion.Euler(0, 0, rotation)
                ).GetComponent<Bullet>();

        bullet.Init(bulletSpeed, dir);

        curAmmo--; 
        curShootTime = shootTime;
    }

    public void Reload()
    {
        if (isReloading) return;

        isReloading = true;
        this.Invoke(() => { isReloading = false; curAmmo = maxAmmo; }, reloadTime);
    }

    public void SetDirection(PlayerDirection playerDirection)
    {
        direction = playerDirection;
    }

    private void Update()
    {
        h.gameObject.SetActive(direction is PlayerDirection.left or PlayerDirection.right);
        h.localScale = new Vector3(direction is PlayerDirection.left ? -1 : 1, 1, 1);

        v.gameObject.SetActive(direction is PlayerDirection.up or PlayerDirection.down);
        v.localScale = new Vector3(1, direction is PlayerDirection.down ? -1 : 1, 1);

        if (curShootTime > 0) curShootTime -= Time.deltaTime;
    }
}
