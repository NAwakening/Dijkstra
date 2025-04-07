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
        protected Vector2 _distance;
        protected Node _startNode;
        protected Node _endNode;
        [SerializeField] protected List<Connection> _connections;
        protected RaycastHit _hit;

        #endregion

        #region EditorButtons

        public void CreateLayout()
        {
            Probing();
            CreateGraph();
        }

        public void SimplifyLayout()
        {
            EliminateRedudantConections();
        }

        public void StartDijkstra()
        {

        }

        public void ResestAll()
        {
            DestroyEverything();
        }

        #endregion

        #region LocalMethods

        protected void Probing()
        {
            float t_startMinDistance = 0;
            float t_endMinDistance = 0;
            _distance = new Vector2(_mapDimensions.x / (_numberOfNodes.x - 1f), _mapDimensions.y / (_numberOfNodes.y - 1f));
            for (int i = 0; i < _numberOfNodes.y; i++)
            {
                for (int j = 0; j < _numberOfNodes.x; j++) 
                { 
                    GameObject t_node = Instantiate(_node);
                    t_node.transform.parent = _nodeParent;
                    t_node.transform.position = new Vector3(transform.position.x + (j * _distance.x), transform.position.y, transform.position.z + (i * _distance.y));
                    Collider[] colliders = Physics.OverlapSphere(t_node.transform.position, 0.35f);
                    if (colliders.Length > 0)
                    {
                        foreach (Collider collider in colliders)
                        {
                            if (collider.gameObject.layer == 6)
                            {
                                t_node.GetComponent<Node>().State = NodeState.DESHABILITADO;
                            }
                        }
                    }
                    if (t_node.GetComponent<Node>().State == NodeState.HABILITADO)
                    {
                        if (t_startMinDistance == 0)
                        {
                            float t_magnitude = (_startPosition.position - t_node.transform.position).magnitude;
                            t_startMinDistance = t_magnitude;
                            _startNode = t_node.GetComponent<Node>();
                            t_magnitude = (_endPosition.position - t_node.transform.position).magnitude;
                            t_endMinDistance = t_magnitude;
                            _endNode = t_node.GetComponent<Node>();
                        }
                        else
                        {
                            float t_magnitude = (_startPosition.position - t_node.transform.position).magnitude;
                            if (t_magnitude < t_startMinDistance)
                            {
                                t_startMinDistance = t_magnitude;
                                _startNode = t_node.GetComponent<Node>();
                            }
                            t_magnitude = (_endPosition.position - t_node.transform.position).magnitude;
                            if (t_magnitude < t_endMinDistance)
                            {
                                t_endMinDistance = t_magnitude;
                                _endNode = t_node.GetComponent<Node>();
                            }
                        }
                    }
                    t_node.GetComponent<Node>().SetIconToNode();
                    _nodes.Add(t_node.GetComponent<Node>());
                }
            }
            _startNode.SetStartNode = true;
            _startNode.SetIconToNode();
            _endNode.SetEndNode = true;
            _endNode.SetIconToNode();
        }

        protected void CreateGraph()
        {
            float t_maxDistance = Mathf.Sqrt(Mathf.Pow(_distance.x, 2) + Mathf.Pow(_distance.y, 2));
            Debug.Log("Iniciando creacion de conecciones");
            for (int i = 0 ; i < _nodes.Count; i++)
            {
                if (_nodes[i].State == NodeState.HABILITADO)
                {
                    Debug.Log("Nodo Habilitado");
                    for (int j = 0; j < _nodes.Count; j++)
                    {
                        if (_nodes[i] != _nodes[j] && _nodes[j].State == NodeState.HABILITADO)
                        {
                            Vector3 _directionAndMagnitude = _nodes[j].transform.position - _nodes[i].transform.position;
                            Debug.Log("Ambos nodos son diferentes y habilitados");
                            if (_directionAndMagnitude.magnitude <= t_maxDistance)
                            {
                                Debug.Log("Los nodos están en el rango de distancia");
                                bool t_allreadyConnected = false;
                                if (_nodes[i].Connections.Count > 0)
                                {
                                    foreach (Connection connection in _nodes[i].Connections)
                                    {
                                        if (connection.NodeA == _nodes[j] || connection.NodeB == _nodes[j])
                                        {
                                            t_allreadyConnected = true;
                                        }
                                    }
                                }
                                if (!t_allreadyConnected)
                                {
                                    Debug.Log("Los nodos aún no están conectados");
                                    if (!Physics.Raycast(_nodes[i].transform.position, _directionAndMagnitude.normalized, out _hit, _directionAndMagnitude.magnitude, LayerMask.GetMask("Obstacle")))
                                    {
                                        if (!Physics.Raycast(_nodes[j].transform.position, (_directionAndMagnitude * -1).normalized, out _hit, _directionAndMagnitude.magnitude, LayerMask.GetMask("Obstacle")))
                                        {
                                            Debug.Log("Creando coneción");
                                            GameObject t_connection = Instantiate(_connection);
                                            t_connection.transform.parent = _connectionParent;
                                            t_connection.GetComponent<Connection>().NodeA = _nodes[i];
                                            t_connection.GetComponent<Connection>().NodeB = _nodes[j];
                                            t_connection.GetComponent<Connection>().Distance = _directionAndMagnitude.magnitude;
                                            
                                            t_connection.transform.position = _nodes[i].transform.position + (_directionAndMagnitude / 2);
                                            _connections.Add(t_connection.GetComponent<Connection>());
                                            _nodes[i].Connections.Add(t_connection.GetComponent<Connection>());
                                            _nodes[j].Connections.Add(t_connection.GetComponent<Connection>());
                                            Debug.Log(_directionAndMagnitude.normalized);
                                            if (_directionAndMagnitude.normalized == new Vector3(1f, 0f, 0f))
                                            {
                                                t_connection.GetComponent<Connection>().Direction = ConnectionDirection.Horizontal;
                                            }
                                            else if (_directionAndMagnitude.normalized == new Vector3(0f, 0f, 1f))
                                            {
                                                t_connection.GetComponent<Connection>().Direction = ConnectionDirection.Vertical;
                                            }
                                            else 
                                            {
                                                if (_directionAndMagnitude.normalized.x <= 0f)
                                                {
                                                    t_connection.GetComponent<Connection>().Direction = ConnectionDirection.DiagonalSE_NO;
                                                }
                                                    
                                                else
                                                {
                                                    t_connection.GetComponent<Connection>().Direction = ConnectionDirection.DiagonalSO_NE;
                                                }
                                            }
                                            
                                            t_connection.GetComponent<Connection>().SetIconToConnection();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void EliminateRedudantConections()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].State == NodeState.HABILITADO)
                {
                    if (_nodes[i].Connections.Count == 2)
                    {
                        if (_nodes[i].Connections[0].Direction == _nodes[i].Connections[1].Direction && _nodes[i].Connections[0].Direction != ConnectionDirection.Modified)
                        {
                            FuseConnections(_nodes[i].Connections[0], _nodes[i].Connections[1], _nodes[i]);
                            _nodes[i].State = NodeState.DESHABILITADO;
                            _nodes[i].SetIconToNode();
                        }
                    }
                    else if (_nodes[i].Connections.Count == 8)
                    {
                        SearchForSameDirectionInConnections(_nodes[i], ConnectionDirection.Horizontal);
                        SearchForSameDirectionInConnections(_nodes[i], ConnectionDirection.Vertical);
                        SearchForSameDirectionInConnections(_nodes[i], ConnectionDirection.DiagonalSE_NO);
                        SearchForSameDirectionInConnections(_nodes[i], ConnectionDirection.DiagonalSO_NE);
                        _nodes[i].State = NodeState.DESHABILITADO;
                        _nodes[i].SetIconToNode();
                    }
                }
            }
        }

        protected void FuseConnections(Connection p_first , Connection p_second, Node p_node)
        {
            p_second.OtherNode(p_node).Connections.Remove(p_second);
            Vector3 _directionAndMagnitude = p_second.OtherNode(p_node).transform.position - p_first.OtherNode(p_node).transform.position;
            p_first.Distance = _directionAndMagnitude.magnitude;
            p_first.transform.position = p_first.OtherNode(p_node).transform.position + (_directionAndMagnitude / 2);
            p_second.OtherNode(p_node).Connections.Add(p_first);
            if (p_first.AmINodeA(p_node))
            {
                p_first.NodeA = p_second.OtherNode(p_node);
            }
            else
            {
                p_first.NodeB = p_second.OtherNode(p_node);
            }
            DestroyImmediate(p_second.gameObject);
            p_first.SetIconToConnection();
        }

        protected void SearchForSameDirectionInConnections(Node p_node, ConnectionDirection p_direction)
        {
            Connection t_first = null;
            Connection t_second = null;
            for (int j = 0; j < p_node.Connections.Count; j++)
            {
                if (p_node.Connections[j].Direction == p_direction)
                {
                    if (t_first == null)
                    {
                        t_first = p_node.Connections[j];
                    }
                    else
                    {
                        t_second = p_node.Connections[j];
                    }
                }
            }
            FuseConnections(t_first, t_second, p_node);
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

