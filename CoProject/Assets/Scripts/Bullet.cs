using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletHitParticle;

    private float speed;
    private float damage;
    private Vector2 dir;

    private void Start() 
    {
        Destroy(gameObject, 3f);
    }

    public void Init(float speed, Vector2 direction, float damage)
    {
        this.speed = speed;
        dir = direction;
        this.damage = damage;
    }

    private void Update()
    {
        var prepos = transform.position;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        var hits = Physics2D.LinecastAll(prepos, transform.position).OrderBy(h => Vector2.Distance(transform.position, h.point)).ToArray();
        foreach(var hit in hits) 
        {
            if(hit.collider.CompareTag("Enemy"))
                if(hit.collider.TryGetComponent<Enemy>(out var comp)) comp.Damage(damage);

            Instantiate(bulletHitParticle, hit.point, Quaternion.Euler(transform.eulerAngles.z + 90, -90, 90));
            Destroy(gameObject);
            break;
        }
    }
}
