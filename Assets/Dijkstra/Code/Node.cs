using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace NAwakening.Dijkstra
{
    #region Enumerator
    
    public enum NodeState
    {
        HABILITADO,
        DESHABILITADO
    }

    #endregion

    public class Node : MonoBehaviour
    {
        #region Parameters

        [SerializeField] protected bool _startNode;
        [SerializeField] protected bool _endNode;
        [SerializeField] protected NodeState _state;

        #endregion

        #region RunTimeVariables

        [SerializeField]protected List<Connection> _connections;

        #endregion

        #region PublicMethods

        public void SetIconToNode()
        {
            gameObject.name = "node" + "(" + transform.position.x.ToString("F2") + ", " + transform.position.z.ToString("F2") + ")";
            if (_state == NodeState.DESHABILITADO)
            {
                IconManager.SetIcon(gameObject, IconManager.LabelIcon.Gray);
            }
            else
            {
                if (_startNode)
                {
                    IconManager.SetIcon(gameObject, IconManager.LabelIcon.Blue);
                }
                else if (_endNode)
                {
                    IconManager.SetIcon(gameObject, IconManager.LabelIcon.Red);
                }
                else
                {
                    IconManager.SetIcon(gameObject, IconManager.LabelIcon.Green);
                }
            }
        }

        #endregion

        #region SettersAndGetters

        public NodeState State
        {
            set { _state = value; }
            get { return _state; }
        }

        public List<Connection> Connections
        {
            get { return _connections; }
        }

        public bool SetStartNode
        {
            set { _startNode = value; }
        }

        public bool SetEndNode
        {
            set { _endNode = value; }
        }

        #endregion
    }
}