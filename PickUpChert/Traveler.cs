using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class Traveler : MonoBehaviour {
        public bool IsActivated { get; private set; }
        public bool IsInShip { get; private set; }
        public bool IsInsideShipBeamVolume { get; private set; }

        Stack<PathProbe> _stackedPathProbes = new Stack<PathProbe>();
        bool _goingToShip;
        PathProbe _currentProbe;
        bool _stop;

        public void AddStackedPathProbe(PathProbe probe) {
            _stackedPathProbes.Push(probe);
        }

        public void ReachProbe(PathProbe probe) {
            if (probe) {
                IsActivated = true;
                if(_goingToShip) {
                    GoToShip();
                    return;
                }

                if(probe._moveTarget) {
                    PickUpChert.Locomotion.GabbroMoveTo(probe._moveTarget, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                    _currentProbe = probe;
                }

                if(probe._isStackedForShip) {
                    _stackedPathProbes.Push(probe);
                }
            }
        }

        public void GoToShip() {
            IsActivated = true;
            _goingToShip = true;
            if (_stackedPathProbes.Count > 0) {
                PathProbe probe = _stackedPathProbes.Pop();
                if (probe) {
                    PickUpChert.Locomotion.GabbroMoveTo(probe.transform, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                    _currentProbe = probe;
                }
            }
            else {
                PickUpChert.Locomotion.GabbroMoveTo(Locator.GetShipTransform(), 0.5f, 3f, new Vector3(0, -0.5f, 0));
            }
        }

        public void GoToCenterOfShipToExit() {
            PickUpChert.Locomotion.GabbroMoveTo(Locator.GetShipTransform(), 0.5f, 2f, Vector3.zero);
        }

        public void CompleteEnteringShip() {
            _goingToShip = false;
            IsInShip = true;
            IsInsideShipBeamVolume = true;
            _currentProbe = null;
        }

        public void CompleteExitingShip() {
            _goingToShip = false;
            IsInShip = false;
            _currentProbe = null;
        }

        public void CompleteExitingShipBeamVolume() {
            IsInsideShipBeamVolume = false;
        }

        void Update() {
            if (_goingToShip) {
                return;
            }
            if (_currentProbe) {
                return;
            }
            if (IsInShip) {
                return;
            }
            if(_stop) {
                return;
            }

            if (Vector3.Distance(Locator.GetPlayerTransform().position, transform.position) < 3.0f) {
                return;
            }
            PickUpChert.Locomotion.GabbroMoveTo(Locator.GetPlayerTransform(), 3f, 3f, Vector3.zero);
        }
    }
}
