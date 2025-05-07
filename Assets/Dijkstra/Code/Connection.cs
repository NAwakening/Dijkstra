using UnityEngine;

namespace NAwakening.Dijkstra
{
    #region

    public enum ConnectionDirection
    {
        Horizontal,
        Vertical,
        DiagonalSO_NE,
        DiagonalSE_NO,
        Modified
    }

    #endregion

    public class Connection : MonoBehaviour
    {
        #region RuntimeVariables

        [SerializeField] protected Node _nodeA;
        [SerializeField] protected Node _nodeB;
        [SerializeField] protected float distance;
        [SerializeField] protected ConnectionDirection _direction;

        Vector3 _origin;
        Vector3 _directionAndMagnitude;

        #endregion

        #region PublicMethods
    
        public void SetIconToConnection()
        {
            gameObject.name = _nodeA.gameObject.name + "-" + _nodeB.gameObject.name;
            transform.GetChild(0).gameObject.name = distance.ToString("F2") + " - " + _direction;
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

        public bool AmINodeA(Node value)
        {
            if (value == _nodeA)
            {
                return true;
            }
            else return false;
        }

        #endregion

        private void OnDrawGizmos()
        {
            _origin = _nodeA.transform.position;
            _directionAndMagnitude = _nodeB.transform.position - _origin;
            Debug.DrawRay(_origin, _directionAndMagnitude, Color.black);
        }

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

        public ConnectionDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        #endregion
    }
}