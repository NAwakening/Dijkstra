using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace NAwakening.Dijkstra
{
    #region Structs

    [System.Serializable]
    public struct Route
    {
        //[SerializeField] public List<Node> visitedNodes;
        [SerializeField] public List<int> idsVisitedNodes;
        [SerializeField] public float distance;
    }

    #endregion

    public class Dijkstra : MonoBehaviour
    {
        #region References

        [SerializeField, HideInInspector] protected GameObject _node;
        [SerializeField, HideInInspector] protected Transform _startPosition;
        [SerializeField, HideInInspector] protected Transform _endPosition;
        [SerializeField, HideInInspector] protected Transform _nodeParent;
        [SerializeField, HideInInspector] protected GameObject _connection;
        [SerializeField, HideInInspector] protected Transform _connectionParent;
        [SerializeField] protected NPC_SO _behaviour;

        #endregion

        #region Parameters

        [SerializeField, HideInInspector] protected Vector2 _mapDimensions;
        [SerializeField] protected Vector2 _numberOfNodes;
        [SerializeField] protected float _speed;

        #endregion

        #region RuntimeVariables

        [SerializeField, HideInInspector] protected List<Node> _nodes;
        protected Vector2 _distance;
        protected Node _startNode;
        protected Node _endNode;
        [SerializeField, HideInInspector] protected List<Connection> _connections;
        protected RaycastHit _hit;
        [SerializeField, HideInInspector] protected List<Route> _routes;
        [SerializeField, HideInInspector] protected List<Route> _succesfullRoutes;
        [SerializeField, HideInInspector] protected Route _initialRoute;
        [SerializeField] protected Route _bestRoute;

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
            _initialRoute.idsVisitedNodes.Add(_startNode.GetNodeID);
            _routes.Add(_initialRoute);
            SearchNeighbours(_startNode, _initialRoute);
            Debug.Log("Unsuccesful Routes: " + _routes.Count);
            Debug.Log("Succesful Routes: " + _succesfullRoutes.Count);
            Debug.Log("Total Routes: " + (_routes.Count + _succesfullRoutes.Count));
            AssignBestRouteToAgent();
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
            for (int i = 0 ; i < _nodes.Count; i++)
            {
                if (_nodes[i].State == NodeState.HABILITADO)
                {
                    for (int j = 0; j < _nodes.Count; j++)
                    {
                        if (_nodes[i] != _nodes[j] && _nodes[j].State == NodeState.HABILITADO)
                        {
                            Vector3 t_directionAndMagnitude = _nodes[j].transform.position - _nodes[i].transform.position;
                            if (IsInRange(t_directionAndMagnitude, t_maxDistance))
                            {
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
                                    if (!Physics.Raycast(_nodes[i].transform.position, t_directionAndMagnitude.normalized, out _hit, t_directionAndMagnitude.magnitude, LayerMask.GetMask("Obstacle")))
                                    {
                                        if (!Physics.Raycast(_nodes[j].transform.position, (t_directionAndMagnitude * -1).normalized, out _hit, t_directionAndMagnitude.magnitude, LayerMask.GetMask("Obstacle")))
                                        {
                                            GameObject t_connection = Instantiate(_connection);
                                            t_connection.transform.parent = _connectionParent;
                                            t_connection.GetComponent<Connection>().NodeA = _nodes[i];
                                            t_connection.GetComponent<Connection>().NodeB = _nodes[j];
                                            t_connection.GetComponent<Connection>().Distance = t_directionAndMagnitude.magnitude;
                                            
                                            t_connection.transform.position = _nodes[i].transform.position + (t_directionAndMagnitude / 2);
                                            _connections.Add(t_connection.GetComponent<Connection>());
                                            _nodes[i].Connections.Add(t_connection.GetComponent<Connection>());
                                            _nodes[j].Connections.Add(t_connection.GetComponent<Connection>());
                                            if (t_directionAndMagnitude.normalized == new Vector3(1f, 0f, 0f) || t_directionAndMagnitude.normalized == new Vector3(-1f, 0f, 0f))
                                            {
                                                t_connection.GetComponent<Connection>().Direction = ConnectionDirection.Horizontal;
                                            }
                                            else if (t_directionAndMagnitude.normalized == new Vector3(0f, 0f, 1f) || t_directionAndMagnitude.normalized == new Vector3(0f, 0f, -1f))
                                            {
                                                t_connection.GetComponent<Connection>().Direction = ConnectionDirection.Vertical;
                                            }
                                            else 
                                            {
                                                if (t_directionAndMagnitude.normalized.x <= 0f)
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

        protected bool IsInRange(Vector3 distance, float maxDistance)
        {
            if (distance.normalized == new Vector3(0f, 0f, 1f))
            {
                if (distance.magnitude <= _distance.y)
                {
                    return true;
                }
                else 
                { 
                    return false; 
                }
            }
            else if (distance.normalized == new Vector3(1f, 0f, 0f))
            {
                if (distance.magnitude <= _distance.x)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (distance.magnitude <= maxDistance)
                {
                    return true;
                }
                else 
                { 
                    return false; 
                }
            }
        }

        protected void EliminateRedudantConections()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].State == NodeState.HABILITADO && _nodes[i] != _endNode && _nodes[i] != _startNode)
                {
                    if (_nodes[i].Connections.Count == 2)
                    {
                        if (_nodes[i].Connections[0].Direction == _nodes[i].Connections[1].Direction)
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
                        break;
                    }
                }
            }
            FuseConnections(t_first, t_second, p_node);
        }

        protected void SearchNeighbours(Node p_previousNode, Route p_allreadyVisitedNode)
        {
            for(int i = 0; i < p_previousNode.Connections.Count; i++)
            {
                if (!p_allreadyVisitedNode.idsVisitedNodes.Contains(p_previousNode.Connections[i].OtherNode(p_previousNode).GetNodeID))
                {
                    Route currentRoute = new Route();
                    currentRoute.idsVisitedNodes = new List<int>(p_allreadyVisitedNode.idsVisitedNodes);
                    currentRoute.idsVisitedNodes.Add(p_previousNode.Connections[i].OtherNode(p_previousNode).GetNodeID);
                    currentRoute.distance = p_allreadyVisitedNode.distance + p_previousNode.Connections[i].Distance;
                    if (p_previousNode.Connections[i].OtherNode(p_previousNode) == _endNode)
                    {
                        _succesfullRoutes.Add(currentRoute);
                        continue;
                    }
                    _routes.Add(currentRoute);
                    SearchNeighbours(p_previousNode.Connections[i].OtherNode(p_previousNode), currentRoute);
                }
            }
        }

        protected void AssignBestRouteToAgent()
        {
            float minDistance = Mathf.Infinity;
            int minIndex = 0;
            for (int i = 0; i < _succesfullRoutes.Count; i++)
            {
                if (_succesfullRoutes[i].distance < minDistance)
                {
                    minIndex = i;
                    minDistance = _succesfullRoutes[i].distance;
                }
            }
            _bestRoute = _succesfullRoutes[minIndex];
            for (int i = 1; i < _succesfullRoutes[minIndex].idsVisitedNodes.Count; i++)
            {
                Behaviour currentMove = new Behaviour();
                currentMove.stateMechanic = StateMechanic.MOVE;
                currentMove.velocity = _speed;
                currentMove.destinyDirection = ((GameObject)EditorUtility.InstanceIDToObject(_succesfullRoutes[minIndex].idsVisitedNodes[i])).transform.position;
                _behaviour.behaviour.Add(currentMove);
            }
            Behaviour finalBehaviour = new Behaviour();
            finalBehaviour.stateMechanic = StateMechanic.STOP;
            finalBehaviour.velocity = -1;
            _behaviour.behaviour.Add(finalBehaviour);

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

            _initialRoute.idsVisitedNodes.Clear();
            _routes.Clear();
            _succesfullRoutes.Clear();
            _behaviour.behaviour.Clear();
        }

        #endregion
    }
}

