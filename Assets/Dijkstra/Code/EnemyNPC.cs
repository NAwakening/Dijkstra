using System.Collections;
using UnityEngine;

namespace NAwakening.Dijkstra
{

    public class EnemyNPC : MonoBehaviour
    {
        #region References

        [SerializeField] protected FiniteStateMachine fsm;
        [SerializeField] protected Animator animator;

        #endregion

        #region Knobs

        [SerializeField] protected float movingSpeed;
        [SerializeField] protected float turningSpeed;

        #endregion

        #region RunTumeVariables

        [SerializeField] protected NPC_SO behaviour;
        protected Behaviour currentBehaviour;
        protected int currentBehaviourIndex;

        #endregion

        #region UnityMethods

        void Start()
        {
            currentBehaviour = behaviour.behaviour[0];
            InitializeSubstate();
            InvokeStateMechanic();
            if (currentBehaviour.velocity >= 0 && currentBehaviour.stateMechanic != StateMechanic.MOVE)
            {
                StartCoroutine(TimerForEnemyBehaviour());
            }
        }

        void Update()
        {

        }

        #endregion

        #region LocalMethods

        protected void InvokeStateMechanic()
        {
            switch (currentBehaviour.stateMechanic)
            {
                case StateMechanic.STOP:
                    fsm.StateMechanic(StateMechanic.STOP);
                    break;
                case StateMechanic.MOVE:
                    fsm.StateMechanic(StateMechanic.MOVE);
                    break;
            }
        }

        protected void InitializeSubstate()
        {
            switch (currentBehaviour.stateMechanic)
            {
                case StateMechanic.STOP:
                    InitializeStopSubstateMechanic();
                    break;
                case StateMechanic.MOVE:
                    InitializeMoveSubstateMechanic();
                    break;
            }
        }

        protected void FinalizeSubstate()
        {
            switch (currentBehaviour.stateMechanic)
            {
                case StateMechanic.MOVE:
                    FinalizeMoveSubstateMechanic();
                    break;
            }
        }

        #endregion

        #region PublicMethods

        public void GoToNextBehaviour()
        {
            currentBehaviourIndex++;
            if (currentBehaviourIndex >= behaviour.behaviour.Count)
            {
                currentBehaviourIndex = 0;
            }
            currentBehaviour = behaviour.behaviour[currentBehaviourIndex];
            InitializeSubstate();
            InvokeStateMechanic();
            if (currentBehaviour.velocity >= 0 && currentBehaviour.stateMechanic != StateMechanic.MOVE)
            {
                StartCoroutine(TimerForEnemyBehaviour());
            }
        }

        #endregion

        #region SubStateMechanic

        protected void InitializeStopSubstateMechanic()
        {
            fsm.SetMoveDirection = Vector3.zero;
            fsm.SetMoveSpeed = 0;
        }

        protected void InitializeMoveSubstateMechanic()
        {
            fsm.SetMoveDirection = currentBehaviour.destinyDirection;
            fsm.SetMoveSpeed = currentBehaviour.velocity;
        }

        protected void FinalizeMoveSubstateMechanic()
        {
            fsm.SetMoveDirection = Vector3.zero;
            fsm.SetMoveSpeed = 0;
            transform.position = currentBehaviour.destinyDirection;
        }

        #endregion

        #region Corrutines

        IEnumerator TimerForEnemyBehaviour()
        {
            yield return new WaitForSeconds(currentBehaviour.velocity);
            FinalizeSubstate();
            GoToNextBehaviour();
        }

        #endregion

        #region GettersAndSetters

        public Animator GetAnimator
        {
            get { return animator; }
        }

        #endregion
    }
}