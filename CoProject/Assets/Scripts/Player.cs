using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;

    private void Awake() 
    {
        instance = this;

        rb = GetComponent<Rigidbody2D>();    
    }

    private void Update() 
    {
        Move();
    }

    private void Move()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(h, v).normalized * moveSpeed;
    }
}
