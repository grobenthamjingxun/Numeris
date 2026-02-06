using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolChaseFSM : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Optional: who to chase when interaction fails (can be set at runtime)")]
    [SerializeField] private Transform defaultChaseTarget;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 1.5f;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float chaseSpeed = 4.5f;

    [Header("Chase End (optional)")]
    [SerializeField] private bool allowStopChaseIfFar = true;
    [SerializeField] private float loseDistance = 12f; // only used if allowStopChaseIfFar = true

    [Header("Catch")]
    [SerializeField] private float catchDistance = 1.2f;
    [SerializeField] private bool stopAfterCatch = true;

    [Header("NavMesh Safety")]
    [SerializeField] private float navMeshSampleRadius = 2f;
    [SerializeField] private float agentWarpSearchRadius = 3f;

    [Header("Chase Repath")]
    [SerializeField] private float chaseRepathInterval = 0.15f;

    [Header("Player Damage")]
    [SerializeField] private int damageOnCatch = 10; // Damage to deal when catching player
    [SerializeField] private PlayerHealth playerHealth; // Reference to PlayerHealth script

    public enum State { Patrol, Chase }
    public State currentState;

    private Coroutine stateRoutine;
    private int patrolIndex = 0;

    private Transform chaseTarget;
    private float loseSqr;

    private bool hasCaughtTarget;

    // ADDED: Player trigger detection
    private bool isInPlayerTrigger = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        loseSqr = loseDistance * loseDistance;

        EnsureOnNavMesh();
        ForceState(State.Patrol);
    }

    // ADDED: Trigger enter/exit for player detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{name} entered Player trigger zone");
            isInPlayerTrigger = true;
            
            // If we're chasing and enter player trigger, catch the player
            if (currentState == State.Chase && !hasCaughtTarget && chaseTarget != null)
            {
                hasCaughtTarget = true;
                agent.isStopped = true;
                // completely stop the chaser from moving; no sliding forward
                agent.velocity = Vector3.zero;
                agent.ResetPath();
                
                OnCatchTarget(chaseTarget);
                
                if (stopAfterCatch)
                {
                    ChangeState(State.Patrol);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayerTrigger = false;
        }
    }

    // =========================
    // PUBLIC TRIGGERS (CALL THESE)
    // =========================

    /// <summary>
    /// Call this when your interaction fails. Pass the thing to chase (player head / XR origin / etc).
    /// </summary>
    public void OnInteractionFailed(Transform targetToChase)
    {
        hasCaughtTarget = false;

        chaseTarget = targetToChase != null ? targetToChase : defaultChaseTarget;

        if (chaseTarget != null)
            ChangeState(State.Chase);
    }

    /// <summary>
    /// Optional: force the enemy back to patrol (eg. on interaction success).
    /// </summary>
    public void ReturnToPatrol()
    {
        hasCaughtTarget = false;
        chaseTarget = null;
        ChangeState(State.Patrol);
    }

    // =========================
    // CUSTOMIZE THIS
    // =========================

    /// <summary>
    /// Called ONCE when the enemy gets within catchDistance of the chaseTarget.
    /// Put your "do something" logic here (jumpscare, fail state, etc).
    /// </summary>
    protected virtual void OnCatchTarget(Transform target)
    {
        Debug.Log($"{name} caught {target.name}");
        // Apply damage to player
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageOnCatch);
            Debug.Log($"Dealt {damageOnCatch} damage to player. Health: {playerHealth.currentHealth}");
        }
        else
        {
            Debug.LogWarning("PlayerHealth not found - cannot deal damage");
        }
    }

    // =========================
    // FSM
    // =========================

    private void ChangeState(State newState)
    {
        if (currentState == newState) return;
        ForceState(newState);
    }

    private void ForceState(State newState)
    {
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);

        currentState = newState;

        EnsureOnNavMesh();

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }

        switch (newState)
        {
            case State.Patrol: stateRoutine = StartCoroutine(Patrol()); break;
            case State.Chase:  stateRoutine = StartCoroutine(Chase());  break;
        }
    }

    private IEnumerator Patrol()
    {
        agent.speed = patrolSpeed;
        agent.isStopped = false;

        if (!HasValidPatrolPoints())
            yield break;

        patrolIndex = GetNextValidPatrolIndex(patrolIndex);
        if (patrolIndex == -1)
            yield break;

        SetDestinationSafe(patrolPoints[patrolIndex].position);

        while (currentState == State.Patrol)
        {
            // If someone triggered a chase (interaction failure), switch
            if (chaseTarget != null)
            {
                ChangeState(State.Chase);
                yield break;
            }

            // If patrol point is unreachable, move on
            if (!agent.pathPending && agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                patrolIndex = GetNextValidPatrolIndex(patrolIndex + 1);
                if (patrolIndex == -1) yield break;

                SetDestinationSafe(patrolPoints[patrolIndex].position);
                yield return null;
                continue;
            }

            bool arrived = !agent.pathPending &&
                           (agent.remainingDistance <= agent.stoppingDistance + 0.1f);

            if (arrived)
            {
                agent.isStopped = true;

                float wait = 0f;
                while (wait < patrolWaitTime)
                {
                    if (chaseTarget != null)
                    {
                        agent.isStopped = false;
                        ChangeState(State.Chase);
                        yield break;
                    }

                    wait += Time.deltaTime;
                    yield return null;
                }

                agent.isStopped = false;

                patrolIndex = GetNextValidPatrolIndex(patrolIndex + 1);
                if (patrolIndex == -1) yield break;

                SetDestinationSafe(patrolPoints[patrolIndex].position);
            }

            yield return null;
        }
    }

    private IEnumerator Chase()
    {
        agent.speed = chaseSpeed;
        agent.isStopped = false;

        float repathTimer = 0f;

        while (currentState == State.Chase)
        {
            if (chaseTarget == null)
            {
                ChangeState(State.Patrol);
                yield break;
            }

            // ---- CATCH VIA TRIGGER ----
            // Moved to OnTriggerEnter for better physics-based detection
            
            // ---- OPTIONAL: STOP CHASE IF FAR ----
            if (allowStopChaseIfFar)
            {
                // Flat distance
                Vector3 a = transform.position; a.y = 0f;
                Vector3 b = chaseTarget.position; b.y = 0f;
                float distSqr = (b - a).sqrMagnitude;
                
                if (distSqr > loseSqr)
                {
                    chaseTarget = null;
                    ChangeState(State.Patrol);
                    yield break;
                }
            }

            // ---- REPATH ----
            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f && !hasCaughtTarget)
            {
                repathTimer = chaseRepathInterval;
                SetDestinationSafe(chaseTarget.position);
            }

            yield return null;
        }
    }

    // =========================
    // HELPERS
    // =========================

    private bool HasValidPatrolPoints()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return false;
        for (int i = 0; i < patrolPoints.Length; i++)
            if (patrolPoints[i] != null) return true;
        return false;
    }

    private int GetNextValidPatrolIndex(int startIndex)
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return -1;

        int len = patrolPoints.Length;
        int idx = ((startIndex % len) + len) % len;

        for (int tries = 0; tries < len; tries++)
        {
            if (patrolPoints[idx] != null) return idx;
            idx = (idx + 1) % len;
        }
        return -1;
    }

    private void EnsureOnNavMesh()
    {
        if (!agent) return;
        if (agent.isOnNavMesh) return;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, agentWarpSearchRadius, NavMesh.AllAreas))
            agent.Warp(hit.position);
    }

    private void SetDestinationSafe(Vector3 worldPos)
    {
        if (!agent) return;

        EnsureOnNavMesh();
        if (!agent.isOnNavMesh) return;

        if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(worldPos);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (allowStopChaseIfFar)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, loseDistance);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
    }
#endif
}