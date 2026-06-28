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
        HashSet<PathProbe> _setForStackedPathProbes = new HashSet<PathProbe>();
        bool _goingToShip;
        PathProbe _currentProbe;
        PathProbe _targetProbe;
        bool _stop;

        public void AddStackedPathProbe(PathProbe probe) {
            if(_setForStackedPathProbes.Contains(probe)) {
                return;
            }
            _stackedPathProbes.Push(probe);
            _setForStackedPathProbes.Add(probe);
        }

        public void ReachProbe(PathProbe probe) {
            if (probe != null) {
                IsActivated = true;
                if(_goingToShip) {
                    if(!string.IsNullOrEmpty(probe._conversationFileName)) {
                        MovingConversation.Instance.DisplayDialogue(probe._conversationFileName, ChertPickUpConversation.Instance.GetMovingConversationItem(probe._conversationFileName));
                    }
                    if(probe == _targetProbe) {
                        _currentProbe = probe;
                        GoToShip();
                    }
                    return;
                }

                if(probe._moveTarget) {
                    PickUpChert.Locomotion.GabbroMoveTo(probe._moveTarget, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                    _targetProbe = probe._moveTarget.GetComponent<PathProbe>(); // maybe null
                }
                else {
                    PickUpChert.Locomotion.GabbroMoveStop();
                }

                if(_currentProbe != probe) {
                    if(probe._stopPlaying) {
                        PickUpChert.Locomotion.GabbroStopPlaying();
                    }
                    else {
                        PickUpChert.Locomotion.GabbroStartPlaying();
                    }

                    if(!string.IsNullOrEmpty(probe._conversationFileName)) {
                        MovingConversation.Instance.DisplayDialogue(probe._conversationFileName, ChertPickUpConversation.Instance.GetMovingConversationItem(probe._conversationFileName));
                    }
                    _currentProbe = probe;
                }

                if(probe._isStackedForShip) {
                    AddStackedPathProbe(probe);
                }
            }
        }

        public void GoToShip() {
            IsActivated = true;
            _goingToShip = true;
            if (_stackedPathProbes.Count > 0) {
                PathProbe probe = _stackedPathProbes.Pop();
                _setForStackedPathProbes.Remove(probe);
                if (probe) {
                    PickUpChert.Locomotion.GabbroMoveTo(probe.transform, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                    _targetProbe = probe;
                }
            }
            else {
                PickUpChert.Locomotion.GabbroMoveTo(Locator.GetShipTransform(), 0.5f, 3f, new Vector3(0, -0.5f, 0));
                _targetProbe = null;
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
            _targetProbe = null;
        }

        public void CompleteExitingShip() {
            _goingToShip = false;
            IsInShip = false;
            _currentProbe = null;
            _targetProbe = null;
        }

        public void CompleteExitingShipBeamVolume() {
            if(!IsInShip) {
                IsInsideShipBeamVolume = false;
            }
        }

        void Update() {
            if (_goingToShip) {
                return;
            }
            if (_currentProbe) {
                return;
            }
            if(_targetProbe) {
                return;
            }
            if (IsInShip) {
                return;
            }
            if(_stop) {
                return;
            }

            if (Vector3.Distance(Locator.GetPlayerTransform().position, transform.position) < 6.0f) {
                return;
            }
            PickUpChert.Locomotion.GabbroMoveTo(Locator.GetPlayerTransform(), 2f, 3f, Vector3.zero);
        }
    }
}
