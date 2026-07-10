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
                return other._pos.magnitude.CompareTo(_pos.magnitude); // inverse order for priority queue
            }
        }
        [SerializeField] public List<Node> _nodes;
        [HideInInspector] public List<string> _nodesSerialized;
        void OnValidate() {
            if (_nodes != null) {
                _nodesSerialized = _nodes.Select(n => JsonUtility.ToJson(n)).ToList();
            }
        }
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            foreach(var node in _nodes) {
                Gizmos.DrawSphere(transform.TransformPoint(node._pos), node._radius);
                foreach (var connectedNodeIndex in node._connectedNodes) {
                    var connectedNode = _nodes[connectedNodeIndex];
                    Gizmos.DrawLine(transform.TransformPoint(node._pos), transform.TransformPoint(connectedNode._pos));
                }
            }
        }
        //endcopy
    }
}