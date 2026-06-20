using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class Traveler : MonoBehaviour {
        Stack<PathProbe> _stackedPathProbes;
        bool _goingToShip;

        void Awake() {
            _stackedPathProbes = new Stack<PathProbe>();
        }

        public void AddStackedPathProbe(PathProbe probe) {
            _stackedPathProbes.Push(probe);
        }

        public void ReachProbe(PathProbe probe) {
            if (probe) {
                if(_goingToShip) {
                    GoToShip();
                }
                else {
                    PickUpChert.Locomotion.GabbroMoveTo(probe._moveTarget, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                }
            }
        }

        public void GoToShip() {
            if(_stackedPathProbes.Count > 0) {
                PathProbe probe = _stackedPathProbes.Pop();
                if (probe) {
                    PickUpChert.Locomotion.GabbroMoveTo(probe.transform, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                }
            }
            else {
                PickUpChert.Locomotion.GabbroMoveTo(Locator.GetShipTransform(), 0.5f, 3f, new Vector3(0, -0.5f, 0));
            }
        }
    }
}
