using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Control;
using RPG.Saving;

namespace RPG.Movement
{
    public class MoveHandler : MonoBehaviour, IAction,ISaveable
    {
        [SerializeField] float maxSpeed = 6f;

        NavMeshAgent agent;
        Transform target;

        private Animator anim;

        private Vector3 newTarget;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponentInChildren<Animator>();
        }
        // Update is called once per frame
        void Update()
        {
            UpdateAnimator();
            if (newTarget.magnitude > 0 && TryGetComponent<PlayerController>(out PlayerController controller))
            {
                if (Vector3.Distance(transform.position, newTarget) < 0.1f)
                {
                    Cancel();
                }
            }
        }
        public void StartMoveAction(Vector3 destination, float speedFraction) //for movement
        {
            GetComponent<ActionScheduler>().StartAction(this);
            newTarget = destination;
            MoveTo(destination, speedFraction);
        }
        public void MoveTo(Vector3 destination, float speedFraction) //combat calling
        {
            agent.SetDestination(destination);
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
        }
        public void Cancel()
        {
            if (agent != null)
            {
                agent.isStopped = true;
            }
        }
        void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            anim.SetFloat("Movement", speed);
        }

        public object Capture()
        {
            return new SerializableVector(transform.position);
        }

        public void Restore(object state)
        {
            SerializableVector position = (SerializableVector)state;
            agent.enabled = false;
            transform.position = position.ToVector();
            agent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}

