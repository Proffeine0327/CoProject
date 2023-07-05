using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Dialogue;

public class Player : MonoBehaviour
{
    private static Player instance;

    [Header("Move")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeedRatio;
    [Header("Interact")]
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactLayer;

    private Rigidbody2D rb;

    private void Awake() 
    {
        instance = this;

        rb = GetComponent<Rigidbody2D>();    
    }

    private void Update() 
    {
        Move();
        Interact();
    }

    private void Move()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        if(
            SequenceManager.isPlayingSequence ||
            AnnounceUI.isShowingAnnounce
        )
        {
            h = 0;
            v = 0;
        }

        rb.velocity = new Vector2(h, v).normalized * moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1f);
    }

    private void Interact()
    {
        InteractUI.DisplayUI(false, Vector2.zero, "");
        
        if(
            SequenceManager.isPlayingSequence ||
            AnnounceUI.isShowingAnnounce
        ) return;

        var hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactLayer).OrderBy(h => Vector2.Distance(transform.position, h.transform.position));

        foreach(var hit in hits)
        {
            if(hit.TryGetComponent<Interactable>(out var comp))
            {
                if(!comp.canTalk) continue;

                comp.DisplayUI();
                if(Input.GetKeyDown(KeyCode.Z)) comp.Interact();
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
