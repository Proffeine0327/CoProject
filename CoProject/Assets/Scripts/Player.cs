using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDirection { left, right, down, up }
public enum WeaponSelection { none, assault, pistol, knife }

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Hp")]
    [SerializeField] private float maxHp;
    [Header("Move")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeedRatio;
    [SerializeField] private float maxStemina;
    [SerializeField] private float steminaAdditiveAmount;
    [SerializeField] private float steminaSubtractAmount;
    [Header("Attack")]
    [SerializeField] private Gun assault;
    [SerializeField] private Gun pistol;
    [Header("Interact")]
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactLayer;
    [Header("Light")]
    [SerializeField] private GameObject handLight;

    private Rigidbody2D rb;
    private PlayerDirection playerDirection;
    private WeaponSelection weaponSelection = WeaponSelection.none;
    private float curStemina;
    [SerializeField] private float curHp;
    private bool isRunning;
    private bool isAttacking;
    private bool isMoving;
    private bool hasAssault;
    private bool hasPistol;
    private bool isGameOver;

    public float MaxHp => maxHp;
    public float CurHp => curHp;

    public Gun CurrentGun
    {
        get
        {
            return weaponSelection switch
            {
                WeaponSelection.assault => assault,
                WeaponSelection.pistol => pistol,
                _ => null,
            };
        }
    }

    public void Damage(float amount)
    {
        curHp -= amount;
    }

    public void GetWeapon(WeaponSelection selection)
    {
        switch (selection)
        {
            case WeaponSelection.assault: hasAssault = true; break;
            case WeaponSelection.pistol: hasPistol = true; break;
            case WeaponSelection.knife: break;
        }
    }

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        curStemina = maxStemina;
        curHp = maxHp;
    }

    private void Update()
    {
        Hp();
        Move();
        Interact();
        Weapon();
    }

    private void Hp()
    {
        if(curHp < 0)
        {
            //gameover
            isGameOver = true;
        }
    }

    private void Move()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        if (
            SequenceManager.isPlayingSequence ||
            AnnounceUI.isShowingAnnounce ||
            isAttacking
        )
        {
            h = 0;
            v = 0;
        }

        if (v == -1) playerDirection = PlayerDirection.down;
        if (v == 1) playerDirection = PlayerDirection.up;
        if (h == -1) playerDirection = PlayerDirection.left;
        if (h == 1) playerDirection = PlayerDirection.right;

        isMoving = h != 0 || v != 0;

        if (Input.GetKeyDown(KeyCode.LeftShift)) isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) isRunning = false;

        if (isRunning && rb.velocity.magnitude > 0)
        {
            curStemina -= steminaSubtractAmount * Time.deltaTime;
            if (curStemina <= 0) isRunning = false;
        }
        else
        {
            curStemina += steminaAdditiveAmount * Time.deltaTime;
        }

        curStemina = Mathf.Clamp(curStemina, 0, maxStemina);

        rb.velocity = new Vector2(h, v).normalized * moveSpeed * (isRunning ? runSpeedRatio : 1);

        switch (playerDirection)
        {
            case PlayerDirection.left: handLight.transform.rotation = Quaternion.Euler(0, 0, -90); break;
            case PlayerDirection.right: handLight.transform.rotation = Quaternion.Euler(0, 0, 90); break;
            case PlayerDirection.up: handLight.transform.rotation = Quaternion.Euler(0, 0, 180); break;
            case PlayerDirection.down: handLight.transform.rotation = Quaternion.Euler(0, 0, 0); break;
        }
    }

    private void Interact()
    {
        InteractUI.DisplayUI(false, Vector2.zero, "");

        if (
            SequenceManager.isPlayingSequence ||
            AnnounceUI.isShowingAnnounce
        ) return;

        var hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactLayer).OrderBy(h => Vector2.Distance(transform.position, h.transform.position));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Interactable>(out var comp))
            {
                if (!comp.canTalk) continue;

                comp.DisplayUI();
                if (Input.GetKeyDown(KeyCode.Z)) comp.Interact();
                break;
            }
        }
    }

    private void Weapon()
    {
        assault.gameObject.SetActive(weaponSelection is WeaponSelection.assault);
        pistol.gameObject.SetActive(weaponSelection is WeaponSelection.pistol);
        //knife

        assault.SetDirection(playerDirection);
        pistol.SetDirection(playerDirection);
        //knife

        if (SequenceManager.isPlayingSequence) return;
        if (AnnounceUI.isShowingAnnounce) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && hasAssault) weaponSelection = weaponSelection is WeaponSelection.assault ? WeaponSelection.none : WeaponSelection.assault;
        if (Input.GetKeyDown(KeyCode.Alpha2) && hasPistol) weaponSelection = weaponSelection is WeaponSelection.pistol ? WeaponSelection.none : WeaponSelection.pistol;
        if (Input.GetKeyDown(KeyCode.Alpha3) /* && hasKnife */) weaponSelection = weaponSelection is WeaponSelection.knife ? WeaponSelection.none : WeaponSelection.knife;

        if (Input.GetKey(KeyCode.A) && !isMoving)
        {
            switch (weaponSelection)
            {
                case WeaponSelection.assault:
                    if (hasAssault && !assault.IsReloading)
                    {
                        assault.Attack();
                        isAttacking = true;
                    }
                    break;
                case WeaponSelection.pistol:
                    if (hasPistol && !pistol.IsReloading)
                    {
                        pistol.Attack();
                        isAttacking = true;
                    }
                    break;
                case WeaponSelection.knife:
                    break;
            }
        }
        else
        {
            isAttacking = false;
        }

        if (Input.GetKey(KeyCode.S))
        {
            switch (weaponSelection)
            {
                case WeaponSelection.assault:
                    if (hasAssault)
                        assault.Reload();
                    break;
                case WeaponSelection.pistol:
                    if (hasPistol)
                        pistol.Reload();
                    break;
                case WeaponSelection.knife:
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
    }