using System;
using UnityEngine;

namespace NAwakening.Dijkstra
{
    #region Enum

    public enum States
    {
        IDLE,
        MOVING,
        TURNING
    }

    public enum StateMechanic
    {
        STOP,
        MOVE,
        TURN
    }

    #endregion

    public class FiniteStateMachine : MonoBehaviour
    {
        #region References

        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected EnemyNPC agent;

        #endregion

        #region RuntimeVariables

        protected States state;
        protected Vector3 moveDirection;
        protected float moveSpeed;
        protected float turnSpeed;
        protected float lerpCronometer;
        protected Vector3 initialValue;

        #endregion

        #region UnityMethods

        private void FixedUpdate()
        {
            switch (state)
            {
                case States.MOVING:
                    ExecutingMovingState();
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void EnteredState(States value)
        {
            state = value;
            switch (state)
            {
                case States.IDLE:
                    InitializeIdleState();
                    break;
                case States.MOVING:
                    InitializeMovingState();
                    break;
            }
        }

        public void StateMechanic(StateMechanic value)
        {
            CleanFlags();
            agent.GetAnimator.SetBool(value.ToString(), true);
        }

        #endregion

        #region LocalMethods

        protected void CleanFlags()
        {
            foreach (StateMechanic value in (StateMechanic[])Enum.GetValues(typeof(StateMechanic)))
            {
                agent.GetAnimator.SetBool(value.ToString(), false);
            }
        }

        #endregion

        #region FSMMethods

        protected void InitializeIdleState()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        protected void InitializeMovingState()
        {
            rb.angularVelocity = Vector3.zero;
            lerpCronometer = 0f;
            initialValue = transform.position;
        }

        protected void ExecutingMovingState()
        {
            rb.linearVelocity = (moveDirection - transform.position).normalized * moveSpeed;
            transform.forward = Vector3.Slerp(transform.forward, (moveDirection - transform.position).normalized, Time.fixedDeltaTime * 2.5f);
            if (Vector3.Distance(transform.position, moveDirection) <= 0.1f)
            {
                (agent).GoToNextBehaviour();
            }
        }

        #endregion

        #region GettersAndSetters

        public Vector3 SetMoveDirection
        {
            set { moveDirection = value; }
        }

        public float SetMoveSpeed
        {
            set { moveSpeed = value; }
        }

        #endregion
    }
}

