using NUnit.Framework;
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

        //protected List<conection> = new List<conection>

        #endregion

        #region PublicMethods

        public void SetIconoToThisGameObject()
        {
            if (_state == NodeState.DESHABILITADO)
            {
                IconManager.SetIcon(gameObject, IconManager.LabelIcon.Gray);
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                IconManager.SetIcon(gameObject, IconManager.LabelIcon.Green);
                transform.GetChild(0).gameObject.name = transform.position.ToString();
                IconManager.SetIcon(transform.GetChild(0).gameObject, IconManager.LabelIcon.Teal);
            }
        }

        #endregion

        #region SettersAndGetters

        public NodeState SetState
        {
            set { _state = value; }
        }

        #endregion
    }
}