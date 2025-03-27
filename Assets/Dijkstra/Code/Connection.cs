using UnityEngine;

namespace NAwakening.Dijkstra
{
    public class Connection : MonoBehaviour
    {
        #region RuntimeVariables

        [SerializeField] protected Node _nodeA;
        [SerializeField] protected Node _nodeB;
        [SerializeField] protected float distance;

        #endregion

        #region PublicMethods
    
        public void SetIconToConnection()
        {
            gameObject.name = _nodeA.gameObject.name + "-" + _nodeB.gameObject.name;
            transform.GetChild(0).gameObject.name = distance.ToString("F2");
            IconManager.SetIcon(gameObject, IconManager.LabelIcon.Magenta);
            IconManager.SetIcon(transform.GetChild(0).gameObject, IconManager.LabelIcon.Yellow);
        }

        public Node OtherNode(Node value)
        {
            if (value == _nodeA || value == _nodeB)
            {
                if (value == _nodeA)
                {
                    return _nodeB;
                }
                else
                {
                    return _nodeA;
                }
            }
            Debug.LogError(this.name + " " + gameObject.name + " - Node " + value.name + " in asking for a connection not valid with " + _nodeA.name + " - " + _nodeB.name + ".", value.gameObject);
            return null;
        }

        #endregion

        #region GettersAndSetteres

        public Node NodeA 
        { 
            get { return _nodeA; } 
            set { _nodeA = value; }
        }

        public Node NodeB 
        { 
            get { return _nodeB; } 
            set { _nodeB = value; }
        }

        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        #endregion
    }
}