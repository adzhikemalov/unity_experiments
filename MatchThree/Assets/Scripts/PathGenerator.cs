using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.PathGeneration
{
    public class PathGenerator:MonoBehaviour
    {
        [SerializeField]
        private GameObject _pathPrefab;
        private readonly int _minPerSector = 3;
        private readonly int _maxPerSector = 5;
        private readonly int _amountOfSectors = 10;
        private Dictionary<int, int> _path;
        private Dictionary<int, List<Node>> _nodes;
        private Dictionary<Node, List<Connection>> _connections;
        private List<GameObject> _gameObjects;
        private void Start()
        {
            GeneratePath();
        }

        [ContextMenu("Regenerate path")]
        private void GeneratePath()
        {
            ClearPreviousPath();
            GenerateMainObjects();
            GenerateConnections();
        }

        private void Update()
        {
            foreach (var nodeConnection in _connections)
            {
                foreach (var connection in nodeConnection.Value)
                {
                    Debug.DrawLine(connection.StartNode.position, connection.EndNode.position);
                }
            }
        }

        private void GenerateConnections()
        {
            _connections = new Dictionary<Node, List<Connection>>();
            for (var i = 0; i < _nodes.Count-1; i++)
            {
                foreach (var node in _nodes[i])
                {
                    var possibleNodes = GetPossibleNodes(node);
                    foreach (var possibleNode in possibleNodes)
                    {
                       CreateConnection(node, possibleNode);
                    }
                }
            }
        }

        private void CreateConnection(Node node, Node possibleNode)
        {
            var connection = new Connection(node, possibleNode);
            if (!_connections.TryGetValue(node, out var connections))
            {
                connections = new List<Connection>();
            }
            connections.Add(connection);
            _connections[node] = connections;
            possibleNode.Connected = true;
        }

        private List<Node> GetPossibleNodes(Node node)
        {
            var result = new List<Node>();
            var sector = node.Id.x + 1;
            if (node.Id.y - 1 >= 0 && node.Id.y - 1 < _nodes[sector].Count)
                result.Add(_nodes[sector][node.Id.y - 1]);
            if (node.Id.y+1<_nodes[sector].Count)    
                result.Add(_nodes[sector][node.Id.y + 1]);
            if (node.Id.y < _nodes[sector].Count)
                result.Add(_nodes[sector][node.Id.y]);   
            return result;
        }

        private void GenerateMainObjects()
        {
            _gameObjects = new List<GameObject>();
            _path = new Dictionary<int, int>();
            _nodes = new Dictionary<int, List<Node>>();
            for (var i = 0; i < _amountOfSectors; i++)
            {
                _path[i] = Random.Range(_minPerSector, _maxPerSector+1);   
            }
            foreach (var sectorCount in _path)
            {
                _nodes[sectorCount.Key] = new List<Node>();
                for (var i = 0; i < sectorCount.Value; i++)
                {
                    var nodePosition =  new Vector3(i*2, sectorCount.Key*2);
                    _gameObjects.Add(Instantiate(_pathPrefab, nodePosition, Quaternion.identity));
                    _nodes[sectorCount.Key].Add(new Node{Id = new int2(sectorCount.Key, i), position = nodePosition});
                }
            }
        }

        private void ClearPreviousPath()
        {
            if (_gameObjects != null)
            {
                foreach (var go in _gameObjects)
                {
                    Destroy(go);    
                }
            }
        }
    }

    public class Node
    {
        public int2 Id;
        public Vector3 position;
    }

    public class Connection
    {
        public Node StartNode { get; }
        public Node EndNode { get; }

        public Connection(Node startNode, Node endNode)
        {
            StartNode = startNode;
            EndNode = endNode;
        }
    }
}