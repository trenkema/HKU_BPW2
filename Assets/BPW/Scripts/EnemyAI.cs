using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    public System.Action OnEnemyDead;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Settings")]
    public int enemyID;
    public GameObject[] dropReward;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;
    public float shootForceForward, shootForceUp;
    public bool isExplosive = false;
    public int ExplosiveDamage;
    public float attackForceBackwards;
    public GameObject explosionParticleSystem;

    private StateEnum state;
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float sightRange, attackRange, explosiveAttackRange;
    public bool playerInSightRange, playerInAttackRange;

    public enum StateEnum { Patrolling, Chasing, Attacking }

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        agent = GetComponent<NavMeshAgent>();
        if (isExplosive)
        {
            attackRange = explosiveAttackRange;
        }
    }

    private void CheckState()
    {
        switch (state)
        {
            case StateEnum.Patrolling: PatrollingState(); break;
            case StateEnum.Chasing: ChasePlayerState(); break;
            case StateEnum.Attacking: AttackPlayerState(); break;
        }
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) state = StateEnum.Patrolling;
        if (playerInSightRange && !playerInAttackRange) state = StateEnum.Chasing;
        if (playerInAttackRange && playerInSightRange) state = StateEnum.Attacking;

        CheckState();
    }

    private void PatrollingState()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
            return;
        }

        agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayerState()
    {
        agent.SetDestination(player.position);
        transform.LookAt(player);
    }

    private void AttackPlayerState()
    {
        if (!isExplosive)
        {
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            if (!alreadyAttacked)
            {
                GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * shootForceForward, ForceMode.Impulse);
                rb.AddForce(transform.up * shootForceUp, ForceMode.Impulse);

                alreadyAttacked = true;
                Invoke("ResetAttack", timeBetweenAttacks);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        if (GameManager.Instance.activeQuest != null && GameManager.Instance.activeQuest.isActive)
        {
            GameManager.Instance.activeQuest.goal.EnemyKilled(enemyID);
        }

        if (dropReward.Length > 0)
        {
            int randomDrop = Random.Range(0, dropReward.Length);
            if (dropReward[randomDrop] != null)
            {
                Instantiate(dropReward[randomDrop], transform.position, Quaternion.identity);
            }
        }
        OnEnemyDead?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isExplosive)
        {
            Debug.Log("ATTACKED PLAYER!");
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.TakeDamage(ExplosiveDamage);
                GameObject particles = Instantiate(explosionParticleSystem, transform.position, Quaternion.identity);
                Destroy(particles, 2.5f);
                Destroy(gameObject);
                OnEnemyDead?.Invoke();
            }
        }
    }
}
