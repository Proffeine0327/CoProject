using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player player { get; private set; }

    [SerializeField] private float movespeed;

    private Rigidbody2D rb2d;

    private void Awake() 
    {
        player = this;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        var h = Input.GetAxisRaw("Horizontal");    
        var v = Input.GetAxisRaw("Vertical");
        var dir = new Vector2(h, v).normalized;

        transform.Translate(dir * Time.deltaTime * movespeed);
    }
}
