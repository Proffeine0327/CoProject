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

        if(isSingleShot && !Input.GetKeyDown(KeyCode.A)) return;

        Bullet bullet = null;

        if (direction is PlayerDirection.left or PlayerDirection.right)
            bullet = Instantiate(bulletPrefeb, hSpawnPos.position, Quaternion.identity).GetComponent<Bullet>();
        if (direction is PlayerDirection.up or PlayerDirection.down)
            bullet = Instantiate(bulletPrefeb, vSpawnPos.position, Quaternion.identity).GetComponent<Bullet>();

        Vector2 dir = direction switch
        {
            PlayerDirection.left => Vector2.left,
            PlayerDirection.right => Vector2.right,
            PlayerDirection.up => Vector2.up,
            PlayerDirection.down => Vector2.down,
            _ => Vector2.zero
        };

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
