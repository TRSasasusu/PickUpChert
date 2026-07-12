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
    }
}