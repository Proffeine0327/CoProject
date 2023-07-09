using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Hp")]
    [SerializeField] private float maxHp;
    [Header("Attack")]
    [SerializeField] private float recognizeRange;
    [SerializeField] private float atkWaitTime;
    [SerializeField] private float atkCoolTime;
    [SerializeField] private float atkRange;
    [SerializeField] private float damage;

    private float curHp;
    private bool isAttacking;
    private bool isRecognizePlayer;

    private NavMeshAgent agnet;

    public void Damage(float amount)
    {
        curHp -= amount;
    }

    private void Awake() 
    {
        curHp = maxHp;
        agnet = GetComponent<NavMeshAgent>();
        agnet.updateRotation = false;
        agnet.updateUpAxis = false;
    }

    private void Update() 
    {
        if(curHp <= 0)
        {
            Destroy(gameObject);
            return; 
        }

        if(!isRecognizePlayer)
        {
            if(Vector2.Distance(transform.position, Player.Instance.transform.position) > recognizeRange) return;

            var hits = Physics2D.LinecastAll(transform.position, Player.Instance.transform.position);
            if(hits.Length > 2) return;

            isRecognizePlayer = true;
        }

        if(!isAttacking)
        {
            agnet.enabled = true;
            agnet.SetDestination(Player.Instance.transform.position);

            if(Vector2.Distance(transform.position, Player.Instance.transform.position) < atkRange - 0.3f)
            {
                isAttacking = true;
                StartCoroutine(Attack());
            }
        }
        else
        {
            agnet.enabled = false;
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(atkWaitTime);
        
        //attack
        if(Vector2.Distance(transform.position, Player.Instance.transform.position) < atkRange)
            Player.Instance.Damage(damage);

        yield return new WaitForSeconds(atkCoolTime);
        isAttacking = false;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, recognizeRange);
    }
}
