using UnityEngine;
using System.Collections.Generic;

namespace NAwakening.Dijkstra
{
    #region Struct

    [System.Serializable]
    public struct Behaviour
    {
        public StateMechanic stateMechanic;
        [SerializeField] public Vector3 destinyDirection;
        public float velocity;
    }

    #endregion

    [CreateAssetMenu(fileName = "NPC_SO", menuName = "Dijkstra/NPC_SO")]
    public class NPC_SO : ScriptableObject
    {
        [SerializeField] public List<Behaviour> behaviour;
    }
}

