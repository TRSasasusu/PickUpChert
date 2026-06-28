using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class PathProbe : MonoBehaviour {
        [SerializeField] public Transform _moveTarget;
        [SerializeField] public string _conversationFileName;
        [SerializeField] public float _baseSpeed = 1;
        [SerializeField] public bool _isStackedForShip;
        [SerializeField] public bool _stopPlaying;

        public static PathProbe _nearestProbeToShip;

        void OnTriggerStay(Collider other) {
            var traveler = other.GetComponent<Traveler>();
            if(traveler) {
                traveler.ReachProbe(this);
            }
        }
    }
}
