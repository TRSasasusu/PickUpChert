using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace PickUpChert {
    public class Traveler : MonoBehaviour {
        public bool IsActivated { get; private set; }
        public bool IsInShip { get; private set; }
        public bool IsInsideShipBeamVolume { get; private set; }
        public TravelerController _travelerController { get; private set; }

        //Stack<PathProbe> _stackedPathProbes = new Stack<PathProbe>();
        LinkedList<PathProbe> _stackedPathProbes = new LinkedList<PathProbe>();
        HashSet<PathProbe> _setForStackedPathProbes = new HashSet<PathProbe>();
        bool _goingToShip;
        PathProbe _currentProbe;
        PathProbe _targetProbe;
        bool _stop;
        bool _isSitting;
        Coroutine _trackPathToTargetCoroutine;
        SectorDetector _sectorDetector;

        public void Initialize() {
            _travelerController = transform.parent.GetComponentInChildren<TravelerController>(true);
            _sectorDetector = GetComponentInChildren<SectorDetector>(true);
        }

        public void AddStackedPathProbe(PathProbe probe, bool head = true) {
            if(_setForStackedPathProbes.Contains(probe)) {
                return;
            }
            if(head) {
                _stackedPathProbes.AddFirst(probe);
            }
            else {
                _stackedPathProbes.AddLast(probe);
            }
            //_stackedPathProbes.Push(probe);
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
                        GoToShipRecursive();
                    }
                    return;
                }

                if(_currentProbe != probe) {
                    if(probe._moveTarget) {
                        PickUpChert.Locomotion.GabbroMoveTo(probe._moveTarget, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                        _targetProbe = probe._moveTarget.GetComponent<PathProbe>(); // maybe null
                    }
                    else {
                        PickUpChert.Locomotion.GabbroMoveStop();
                    }

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
            var ship = Locator.GetShipBody();
            if(ship == null) {
                return; // no ship!?
            }

            if(_isSitting) {
                StopSitting();
            }

            var (pathGraph, nearestNode) = PathGraph.SearchPathGraphFromSectors(_sectorDetector);
            if(Vector3.Distance(pathGraph.transform.TransformPoint(nearestNode._pos), transform.position) > Vector3.Distance(transform.position, Locator.GetPlayerTransform().position)) {
                StartCoroutine(TrackPathToTarget(null, null, ship.transform, true));
            }
            else {
                _trackPathToTargetCoroutine = StartCoroutine(TrackPathToTarget(pathGraph.ComputePath(nearestNode._pos, ship.transform.position), pathGraph, ship.transform, true));
            }

            //var probes = ship.GetComponentsInChildren<PathProbe>();
            //var probe = probes.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).FirstOrDefault();
            //if(probe != null) {
            //    AddStackedPathProbe(probe, false);
            //}

            //GoToShipRecursive();
        }

        void GoToShipRecursive() {
            IsActivated = true;
            _goingToShip = true;
            if (_stackedPathProbes.Count > 0) {
                PathProbe probe = _stackedPathProbes.First.Value;
                _stackedPathProbes.RemoveFirst();
                _setForStackedPathProbes.Remove(probe);
                if (probe) {
                    PickUpChert.Locomotion.GabbroMoveTo(probe.transform, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                    _targetProbe = probe;
                }
            }
            else {
                PickUpChert.Locomotion.GabbroMoveTo(Locator.GetShipTransform(), 0.5f, 3f, new Vector3(0, -3.7f, 0));
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
            PickUpChert.Locomotion.GabbroMoveStop();
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

        public void StartSitting() {
            _isSitting = true;
            PickUpChert.Locomotion.GabbroMoveStop();
            PickUpChert.Locomotion.GabbroSitting();
        }

        public void StopSitting() {
            _isSitting = false;
            PickUpChert.Locomotion.GabbroStandUp();
        }

        IEnumerator TrackPathToTarget(List<PathGraph.Node> nodes, PathGraph graph, Transform target, bool isTargetShip) {
            if (graph != null) {
                foreach (var node in nodes) {
                    PickUpChert.Locomotion.GabbroMoveTo(graph.transform, node._radius, node._baseSpeed * 3f, node._pos);
                    while ((transform.position - graph.transform.TransformPoint(node._pos)).sqrMagnitude > node._radius * node._radius) {
                        yield return null;
                    }
                }

                var nearestNode = graph.NearestNode(target.position);
                if (nearestNode != nodes.Last()) {
                    _trackPathToTargetCoroutine = StartCoroutine(TrackPathToTarget(graph.ComputePath(nearestNode._pos, target.position), graph, target, isTargetShip));
                    yield break;
                }
            }

            if(!isTargetShip) {
                PickUpChert.Locomotion.GabbroMoveTo(target, 2f, 3f, Vector3.zero);
            }
            else {
                var probes = target.GetComponentsInChildren<PathProbe>();
                var probe = probes.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).FirstOrDefault();
                PickUpChert.Locomotion.GabbroMoveTo(probe.transform, 0.5f, probe._baseSpeed * 3, Vector3.zero);
                while((transform.position - probe.transform.position).sqrMagnitude > 0.5f * 0.5f) {
                    yield return null;
                }
                PickUpChert.Locomotion.GabbroMoveTo(target, 0.5f, 3f, new Vector3(0, -3.7f, 0));
            }
        }

        virtual public void ConversationStart() {

        }

        virtual public void ConversationEnd() {

        }

        virtual protected void Update() {
            if (_goingToShip) {
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
            if(_isSitting) {
                if (Vector3.Distance(Locator.GetPlayerTransform().position, transform.position) < 10.0f) {
                    return;
                }
                StopSitting();
            }

            if (Vector3.Distance(Locator.GetPlayerTransform().position, transform.position) < 6.0f) {
                return;
            }
            //PickUpChert.Locomotion.GabbroMoveTo(Locator.GetPlayerTransform(), 2f, 3f, Vector3.zero);
            //StartCoroutine(TrackPathToTarget(Locator.GetPathGraph().ComputePath(transform.position, Locator.GetPlayerTransform().position), Locator.GetPathGraph(), Locator.GetPlayerTransform())); // TODO
            var (pathGraph, nearestNode) = PathGraph.SearchPathGraphFromSectors(_sectorDetector);
            if(Vector3.Distance(pathGraph.transform.TransformPoint(nearestNode._pos), transform.position) > Vector3.Distance(transform.position, Locator.GetPlayerTransform().position)) {
                //PickUpChert.Locomotion.GabbroMoveTo(Locator.GetPlayerTransform(), 2f, 3f, Vector3.zero);
                _trackPathToTargetCoroutine = StartCoroutine(TrackPathToTarget(null, null, Locator.GetPlayerTransform(), false));
            }
            else {
                _trackPathToTargetCoroutine = StartCoroutine(TrackPathToTarget(pathGraph.ComputePath(nearestNode._pos, Locator.GetPlayerTransform().position), pathGraph, Locator.GetPlayerTransform(), false));
            }
        }
    }
}
