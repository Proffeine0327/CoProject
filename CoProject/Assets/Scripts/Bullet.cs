using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private Vector2 dir;

    public void Init(float speed, Vector2 direction)
    {
        this.speed = speed;
        dir = direction;
    }

    private void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }
}
