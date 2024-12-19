using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Resources;
using Utils;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [Header("Chase Settings")]
        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float suspicionDuration = 3f;
        [Header("Patrol Settings")]
        [SerializeField] float waypointWaitDuration = 2f;
        [SerializeField] float waypointTolerance = 1f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] PatrolPathController patrolPath;


        private FightHandler fightHandler;

        private MoveHandler moveHandler;

        private Health health;

        private NavMeshAgent agent;

        private GameObject playerTarget;

        LazyValue<Vector3> guardPosition;

        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;

        private int currentWaypointIndex = 0;
        private void Awake()
        {
            playerTarget = GameObject.FindGameObjectWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
            fightHandler = GetComponent<FightHandler>();
            health = GetComponent<Health>();
            moveHandler = GetComponent<MoveHandler>();
            guardPosition = new LazyValue<Vector3>(GetCurrentPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (!health.IsAlive()) { return; }

            if (InAttackRangeOfPlayer() && fightHandler.CanAttack(playerTarget))
            {
                //Attack State
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer <= suspicionDuration)
            {
                //Suspicion State
                SuspicionBehaviour();
            }
            else
            {
                //Guard State
                PatrolBehaviour();
            }


            UpdateTimers();
        }
        private Vector3 GetCurrentPosition()
        {
            return transform.position;
        }
        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        void AttackBehaviour()
        {
            fightHandler.Attack(playerTarget);
            timeSinceLastSawPlayer = 0;
        }
        void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycyleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint >= waypointWaitDuration)
            {
                moveHandler.StartMoveAction(nextPosition,patrolSpeedFraction);
            }
        }
        private bool InAttackRangeOfPlayer()
        {
            return Vector3.Distance(transform.position, playerTarget.transform.position) <= chaseRadius;
        }
        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) <= waypointTolerance;
        }
        private void CycyleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextWaypointIndex(currentWaypointIndex);
        }
        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWayPoint(currentWaypointIndex);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}

