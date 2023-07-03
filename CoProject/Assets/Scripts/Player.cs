using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Dialogue;

public class Player : MonoBehaviour
{
    private static Player instance;

    [SerializeField] private float moveSpeed;
    [SerializeField] private Situation test;
    [SerializeField] private UnityEvent ev;

    private Rigidbody2D rb;

    private void Awake() 
    {
        instance = this;

        rb = GetComponent<Rigidbody2D>();    
    }

    private void Update() 
    {
        Move();

        if(!SequenceManager.isPlayingSequence)
        {
            if(Input.GetKeyDown(KeyCode.C))
                SequenceManager.StartSequence(test);
        }
    }

    private void Move()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        if(SequenceManager.isPlayingSequence)
        {
            h = 0;
            v = 0;
        }

        rb.velocity = new Vector2(h, v).normalized * moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1f);
    }
}
