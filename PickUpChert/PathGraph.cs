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

            internal float _cost;

            public int CompareTo(Node other) {
                return other._pos.sqrMagnitude.CompareTo(_pos.sqrMagnitude); // inverse order for priority queue
            }
        }

        public List<Node> _nodes;

        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            foreach(var node in _nodes) {
                Gizmos.DrawSphere(node._pos + transform.position, node._radius);
                foreach (var connectedNodeIndex in node._connectedNodes) {
                    var connectedNode = _nodes[connectedNodeIndex];
                    Gizmos.DrawLine(node._pos + transform.position, connectedNode._pos + transform.position);
                }
            }
        }
        //endcopy

        public List<Node> ComputePath(Vector3 fromPos, Vector3 targetPos) {
            var fromNode = NearestNode(fromPos);
            var targetNode = NearestNode(targetPos);
            // Implementation for pathfinding algorithm would go here

            foreach(var node in _nodes) {
                node._cost = float.MaxValue;
            }

            var prev = new Dictionary<Node, Node>();
        
            var q = new PriorityQueue<Node>();
            q.Enqueue(fromNode);
            while(q.Count > 0) {
                var currentNode = q.Dequeue();
                if (currentNode == targetNode) {
                    break;
                }
                foreach (var connectedNodeIndex in currentNode._connectedNodes) {
                    var connectedNode = _nodes[connectedNodeIndex];
                    float newCost = currentNode._cost + (currentNode._pos - connectedNode._pos).sqrMagnitude;
                    if (newCost < connectedNode._cost) {
                        connectedNode._cost = newCost;
                        q.Enqueue(connectedNode);
                        prev[connectedNode] = currentNode;
                    }
                }
            }

            // Reconstruct the path
            var path = new List<Node>();
            var current = targetNode;
            while (prev.ContainsKey(current)) {
                path.Add(current);
                current = prev[current];
            }
            path.Add(fromNode);
            path.Reverse();
            return path;
        }

        public Node NearestNode(Vector3 pos) {
            return _nodes.OrderBy(n => (n._pos + transform.position - pos).sqrMagnitude).FirstOrDefault();
        }
    }
}
