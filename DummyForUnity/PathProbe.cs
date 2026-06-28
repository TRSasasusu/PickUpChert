using UnityEngine;
namespace PickUpChert {
    public class PathProbe : MonoBehaviour {
        [SerializeField] public Transform _moveTarget;
        [SerializeField] public string _conversationFileName;
        [SerializeField] public float _baseSpeed = 1;
        [SerializeField] public bool _isStackedForShip;
        [SerializeField] public bool _stopPlaying;
    }
}