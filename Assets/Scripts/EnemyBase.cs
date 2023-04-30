using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int health = 2;
    [SerializeField] private int bodyshotDamage = 1;
    [SerializeField] private int headshotDamage = 2;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform headTransform;
    [SerializeField] private NavMeshAgent agent;
    private bool canSeePlayer = false;

    public void TakeDamage(bool headshot)
    {
        if(headshot)
        {
            health -= headshotDamage;
        }
        else
        {
            health -= bodyshotDamage;
        }
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        RaycastHit hit;
        if(Physics.Linecast(headTransform.position, playerTransform.position, out hit))
        {
            if(hit.collider.CompareTag("Player"))
            {
                canSeePlayer = true;
            }
            else
            {
                canSeePlayer = false;
            }
        }

        if(canSeePlayer)
        {
            agent.SetDestination(playerTransform.position);
        }
    }
}
