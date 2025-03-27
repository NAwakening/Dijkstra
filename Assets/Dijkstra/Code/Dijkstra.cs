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
        [SerializeField] protected GameObject _connection;
        [SerializeField] protected Transform _connectionParent;

        #endregion

        #region Parameters

        [SerializeField] protected Vector2 _mapDimensions;
        [SerializeField] protected Vector2 _numberOfNodes;

        #endregion

        #region RuntimeVariables

        [SerializeField] protected List<Node> _nodes;
        protected GameObject node;
        protected Vector2 _distance;
        [SerializeField] protected List<Connection> _connections;
        protected GameObject connection;
        protected float _maxDistance;
        protected Vector3 _directionAndMagnitude;
        protected RaycastHit _hit;
        protected bool _allreadyConnected;

        #endregion

        #region EditorButtons

        public void CreateLayout()
        {
            Probing();
            CreateGraph();
        }

        public void ResestAll()
        {
            DestroyEverything();
        }

        #endregion

        #region LocalMethods

        protected void Probing()
        {
            _distance = new Vector2(_mapDimensions.x / (_numberOfNodes.x - 1f), _mapDimensions.y / (_numberOfNodes.y - 1f));
            for (int i = 0; i < _numberOfNodes.y; i++)
            {
                for (int j = 0; j < _numberOfNodes.x; j++) 
                { 
                    node = Instantiate(_node);
                    node.transform.parent = _nodeParent;
                    node.transform.position = new Vector3(transform.position.x + (j * _distance.x), transform.position.y, transform.position.z + (i * _distance.y));
                    Collider[] colliders = Physics.OverlapSphere(node.transform.position, 0.35f);
                    if (colliders.Length > 0)
                    {
                        foreach (Collider collider in colliders)
                        {
                            if (collider.gameObject.layer == 6)
                            {
                                node.GetComponent<Node>().State = NodeState.DESHABILITADO;
                            }
                        }
                    }
                    node.GetComponent<Node>().SetIconToNode();
                    _nodes.Add(node.GetComponent<Node>());
                }
            }
        }

        protected void CreateGraph()
        {
            _maxDistance = Mathf.Sqrt(Mathf.Pow(_distance.x, 2) + Mathf.Pow(_distance.y, 2));
            for (int i = 0 ; i < _nodes.Count; i++)
            {
                if (_nodes[i].State == NodeState.HABILITADO)
                {
                    for (int j = 0; j < _nodes.Count; j++)
                    {
                        if (_nodes[i] != _nodes[j] && _nodes[j].State == NodeState.HABILITADO)
                        {
                            _directionAndMagnitude = _nodes[j].transform.position - _nodes[i].transform.position;
                            if (_directionAndMagnitude.magnitude <= _maxDistance)
                            {
                                _allreadyConnected = false;
                                if (_nodes[i].Connections.Count > 0)
                                {
                                    foreach (Connection connection in _nodes[i].Connections)
                                    {
                                        if (connection.NodeA == _nodes[j] || connection.NodeB == _nodes[j])
                                        {
                                            _allreadyConnected = true;
                                        }
                                    }
                                }
                                if (!_allreadyConnected)
                                {
                                    if (!Physics.Raycast(_nodes[i].transform.position, _directionAndMagnitude.normalized, out _hit, _maxDistance, LayerMask.GetMask("Obstacle")))
                                    {
                                        if (!Physics.Raycast(_nodes[j].transform.position, (_directionAndMagnitude * -1).normalized, out _hit, _maxDistance, LayerMask.GetMask("Obstacle")))
                                        {
                                            connection = Instantiate(_connection);
                                            connection.transform.parent = _connectionParent;
                                            connection.GetComponent<Connection>().NodeA = _nodes[i];
                                            connection.GetComponent<Connection>().NodeB = _nodes[j];
                                            connection.GetComponent<Connection>().Distance = _directionAndMagnitude.magnitude;
                                            connection.GetComponent<Connection>().SetIconToConnection();
                                            connection.transform.position = _nodes[i].transform.position + (_directionAndMagnitude / 2);
                                            _connections.Add(connection.GetComponent<Connection>());
                                            _nodes[i].Connections.Add(connection.GetComponent<Connection>());
                                            _nodes[j].Connections.Add(connection.GetComponent<Connection>());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void DestroyEverything()
        {
            foreach (Node node in _nodeParent.GetComponentsInChildren<Node>())
            {
                DestroyImmediate(node.gameObject);
            }
            _nodes.Clear();

            foreach (Connection connection in _connectionParent.GetComponentsInChildren<Connection>())
            {
                DestroyImmediate(connection.gameObject);
            }
            _connections.Clear();
        }

        #endregion
    }
}

