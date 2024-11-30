using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : ISelectableObject {
    [SerializeField] public Player player;
    [SerializeField] public Trial trial;
    [SerializeField] private float detectionRadius = 50f;
    [SerializeField] private float stoppingDistance = 3f;
    [SerializeField] private float refreshRate = 0.5f;

    private NavMeshAgent navMeshAgent;
    private bool isWalking = false;
    private bool isDying = false;
    private int health = 10;


    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(UpdatePath());
    }

    private IEnumerator UpdatePath() {
        if (!isDying) {
            while (true) {
                if (CanSeePlayer()) {
                    SetDestinationToPlayer();
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }

    private bool CanSeePlayer() {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= detectionRadius;
    }

    private void SetDestinationToPlayer() {
        if (!isDying) {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer > stoppingDistance) {
                navMeshAgent.SetDestination(player.transform.position);
                isWalking = true;
            } else {
                // Stop walking if too close to the player
                navMeshAgent.ResetPath();
                isWalking = false;
            }
        }
    }

    public bool IsWalking() {
        return isWalking;
    }

    public bool IsDying() {
        return isDying;
    }


    public override void Interact(Player player) {
        if (!isDying) {
            ReduceHealth(5);
        }
    }


    public void ReduceHealth(int amount) {
        health -= amount;
        if (health <= 0) {
            Die();
        }
    }

    private void Die() {
        isDying = true;
        StartCoroutine(DieWithDelay());
    }

    private IEnumerator DieWithDelay() {
        trial.RemoveEnemyFromList(this);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void WhenSpawned() {
        trial.AddEnemyToList(this);
    }
}
