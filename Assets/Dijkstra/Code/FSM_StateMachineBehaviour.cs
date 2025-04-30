using System;
using UnityEngine;

namespace NAwakening.Dijkstra
{
    public class FSM_StateMachineBehaviour : StateMachineBehaviour
    {
        public States state;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.transform.parent.GetComponent<FiniteStateMachine>().EnteredState(state);
        }
    }
}