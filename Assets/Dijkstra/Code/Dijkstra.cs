using System.Collections.Generic;
using UnityEngine;

namespace NAwakening.Dijkstra
{
    public class Dijkstra : MonoBehaviour
    {
        #region References

        [SerializeField] protected GameObject _node;
        [SerializeField] protected Transform _startPosition;
        [SerializeField] protected Transform _endPosition;
        [SerializeField] protected Transform _nodeParent;

        #endregion

        #region Parameters

        [SerializeField] protected Vector2 _mapDimensions;
        [SerializeField] protected Vector2 _numberOfNodes;

        #endregion

        #region RuntimeVariables

        [SerializeField] protected List<Node> _nodes;
        protected GameObject node;

        #endregion

        #region EditorButtons

        public void CreateLayout()
        {
            Probing();
        }

        public void ResestAll()
        {
            DestroyNodes();
        }

        #endregion

        // Overlap
        #region LocalMethods

        protected void Probing()
        {
            for (int i = 0; i < _numberOfNodes.y; i++)
            {
                for (int j = 0; j < _numberOfNodes.x; j++) 
                { 
                    node = Instantiate(_node);
                    node.transform.parent = _nodeParent;
                    node.transform.position = new Vector3(transform.position.x + (j * (_mapDimensions.x / _numberOfNodes.x)), transform.position.y, transform.position.z + (i * (_mapDimensions.y / _numberOfNodes.y)));
                    Collider[] colliders = Physics.OverlapSphere(node.transform.position, 1f);
                    if (colliders.Length > 0)
                    {
                        foreach (Collider collider in colliders)
                        {
                            if (collider.gameObject.layer == 6)
                            {
                                node.GetComponent<Node>().SetState = NodeState.DESHABILITADO;
                            }
                        }
                    }
                    node.GetComponent<Node>().SetIconoToThisGameObject();
                    _nodes.Add(node.GetComponent<Node>());
                }
            }
        }

        protected void DestroyNodes()
        {
            for (int i = _nodes.Count-1; i >= 0; i--)
            {
                node = _nodes[i].gameObject;
                _nodes.RemoveAt(i);
                DestroyImmediate(node);
            }
            _nodes.Clear();
        }

        #endregion
    }
}

