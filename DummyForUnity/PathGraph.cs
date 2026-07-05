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
            public int CompareTo(Node other) {
                return _pos.sqrMagnitude.CompareTo(other._pos.sqrMagnitude);
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
    }
}