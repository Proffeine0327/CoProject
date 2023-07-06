using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Dialogue;

public enum PlayerDirection { left, right, down, up }

public class Player : MonoBehaviour
{
    private static Player instance;

    [Header("Move")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeedRatio;
    [SerializeField] private float maxStemina;
    [SerializeField] private float steminaAdditiveAmount;
    [SerializeField] private float steminaSubtractAmount;
    [Header("Attack")]
    [SerializeField] private Weapon[] weaponSlots;
    [Header("Interact")]
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactLayer;
    [Header("Light")]
    [SerializeField] private GameObject handLight;

    private Rigidbody2D rb;
    private PlayerDirection playerDirection;
    private int curWeaponIndex = 0;
    private float curStemina;
    private float playerRotation;
    private bool isRunning;

    private void Awake()
    {
        instance = this;

        rb = GetComponent<Rigidbody2D>();
        curStemina = maxStemina;
    }

    private void Update()
    {
        Move();
        Interact();
        Weapon();
    }

    private void Move()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        if (
            SequenceManager.isPlayingSequence ||
            AnnounceUI.isShowingAnnounce
        )
        {
            h = 0;
            v = 0;
        }
        
        if(v == -1) playerDirection = PlayerDirection.down;
        if(v == 1)  playerDirection = PlayerDirection.up;
        if(h == -1) playerDirection = PlayerDirection.left;
        if(h == 1)  playerDirection = PlayerDirection.right;

        if(Input.GetKeyDown(KeyCode.LeftShift)) isRunning = true;
        if(Input.GetKeyUp(KeyCode.LeftShift)) isRunning = false;

        if(isRunning && rb.velocity.magnitude > 0)
        {
            curStemina -= steminaSubtractAmount * Time.deltaTime;
            if(curStemina <= 0) isRunning = false;
        }
        else curStemina += steminaAdditiveAmount * Time.deltaTime;
        curStemina = Mathf.Clamp(curStemina, 0, maxStemina);

        rb.velocity = new Vector2(h, v).normalized * moveSpeed * (isRunning ? runSpeedRatio : 1);
        handLight.transform.rotation = Quaternion.Euler(0, 0, playerRotation);
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
        for(int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].SetDirection(playerDirection);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
