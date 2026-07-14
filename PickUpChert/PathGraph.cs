using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx.InternalUtil;
using UnityEngine;

namespace PickUpChert {
    public class PathGraph : MonoBehaviour {
        //startcopy
        [Serializable]
        public class Node : IComparable<Node> {
            public Vector3 _pos;
            public float _radius = 1;
            public string _conversationFileName;
            public float _baseSpeed = 1;
            public List<int> _connectedNodes;
            public Collider _constraintForNearestNode;

            [HideInInspector] public string _constraintNameForNearestNode;

            internal float _cost;

            public int CompareTo(Node other) {
                return other._pos.magnitude.CompareTo(_pos.magnitude); // inverse order for priority queue
            }
        }
        [SerializeField] public List<Node> _nodes;

        [HideInInspector] public List<string> _nodesSerialized;

        void OnValidate() {
            if (_nodes != null) {
                foreach (var node in _nodes) {
                    if(node._constraintForNearestNode != null) {
                        node._constraintNameForNearestNode = node._constraintForNearestNode.name;
                    }
                }
                _nodesSerialized = _nodes.Select(n => JsonUtility.ToJson(n)).ToList();
            }
        }

        void OnDrawGizmos() {
            foreach(var node in _nodes) {
                if(!string.IsNullOrEmpty(node._conversationFileName)) {
                    Gizmos.color = Color.green;
                }
                else {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawSphere(transform.TransformPoint(node._pos), node._radius);
                Gizmos.color = Color.magenta;
                foreach (var connectedNodeIndex in node._connectedNodes) {
                    var connectedNode = _nodes[connectedNodeIndex];
                    var direction = (transform.TransformPoint(connectedNode._pos) - transform.TransformPoint(node._pos)).normalized;
                    Gizmos.DrawLine(transform.TransformPoint(node._pos), transform.TransformPoint(connectedNode._pos));

                    float arrowHeadLength = 2f;
                    float arrowHeadAngle = 30f;
                    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                    Gizmos.DrawRay(transform.TransformPoint(connectedNode._pos), right * arrowHeadLength);

                    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                    Gizmos.DrawRay(transform.TransformPoint(connectedNode._pos), left * arrowHeadLength);
                }
            }
        }
        //endcopy

        void Awake() {
            if(_nodes == null && _nodesSerialized != null) {
                _nodes = _nodesSerialized.Select(s => JsonUtility.FromJson<Node>(s)).ToList();
                foreach (var node in _nodes) {
                    if (!string.IsNullOrEmpty(node._constraintNameForNearestNode)) {
                        var constraint = transform.Find(node._constraintNameForNearestNode);
                        if (constraint != null) {
                            node._constraintForNearestNode = constraint.GetComponent<Collider>();
                        }
                    }
                }
            }
        }

        public static (PathGraph, Node) SearchPathGraphFromSectors(SectorDetector sectorDetector) {
            var pathGraphs = new HashSet<PathGraph>();
            foreach (var sector in sectorDetector._sectorList) {
                foreach (var pathGraph in sector.GetComponentsInChildren<PathGraph>()) {
                    pathGraphs.Add(pathGraph);
                }
            }

            var pgAndNearestNode = pathGraphs.Select(pg => (pg, pg.NearestNode(sectorDetector.transform.position)))
                .OrderBy(pgAndNode => (pgAndNode.Item1.transform.TransformPoint(pgAndNode.Item2._pos) - sectorDetector.transform.position).magnitude)
                .FirstOrDefault();
            return pgAndNearestNode;
        }

        public List<Node> ComputePath(Vector3 fromPos, Vector3 targetPos) {
            var fromNode = NearestNode(fromPos);
            var targetNode = NearestNode(targetPos);
            PickUpChert.Log($"Computing path from {fromNode._pos} to {targetNode._pos}");

            // Implementation for pathfinding algorithm would go here

            foreach (var node in _nodes) {
                node._cost = float.MaxValue;
            }
            fromNode._cost = 0;

            var prev = new Dictionary<Node, Node>();
        
            var q = new PriorityQueue<Node>();
            q.Enqueue(fromNode);
            while(q.Count > 0) {
                var currentNode = q.Dequeue();
                PickUpChert.Log($"Visiting node {currentNode._pos} with cost {currentNode._cost}");
                if (currentNode == targetNode) {
                    break;
                }
                foreach (var connectedNodeIndex in currentNode._connectedNodes) {
                    var connectedNode = _nodes[connectedNodeIndex];
                    float newCost = currentNode._cost + (transform.TransformPoint(currentNode._pos) - transform.TransformPoint(connectedNode._pos)).magnitude;
                    if (newCost < connectedNode._cost) {
                        connectedNode._cost = newCost;
                        q.Enqueue(connectedNode);
                        prev[connectedNode] = currentNode;
                    }
                    PickUpChert.Log($"Checking connected node {connectedNode._pos} with new cost {newCost}");
                }
            }

            // Reconstruct the path
            var path = new List<Node>();
            var current = targetNode;
            while (prev.ContainsKey(current)) {
                path.Add(current);
                current = prev[current];
            }
            if(!path.Contains(fromNode)) {
                path.Add(fromNode);
            }
            path.Reverse();
            PickUpChert.Log($"path: {string.Join(" -> ", path.Select(n => n._pos.ToString()))}");
            return path;
        }

        public Node NearestNode(Vector3 pos) {
            return _nodes.OrderBy(n => (transform.TransformPoint(n._pos) - pos).magnitude).FirstOrDefault(n => n._constraintForNearestNode == null || n._constraintForNearestNode.ClosestPoint(pos) == pos); // surprisingly, Vector3 == is approximately equal (1e-5)
        }
    }
}
