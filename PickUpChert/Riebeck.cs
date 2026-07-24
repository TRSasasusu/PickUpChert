using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class Riebeck : Traveler {
        bool _standUp;
        Animator _animator;
        bool _movedToShipPos;
        Tween _movedToShipTweenPos;
        Tween _movedToShipTweenRot;

        public override void Initialize() {
            base.Initialize();
            _animator = GetComponentInChildren<Animator>(true);

            _shipSuspendedPos = new Vector3(-2.278f, 0.8706f, -0.8103f);
            _shipSuspendedRot = new Vector3(0, 90, 0);
        }


        public override TextAsset ConversationStart(IEnumerable<ConversationTrigger> triggers, IEnumerable<Sector> sectors) {
            if(!_standUp) {
                return ChertPickUpConversation._conversationDataDict["Riebeck"]._eventXMLDict["before_standup"];
            }
            return null;
        }

        public override void ConversationEnd(IEnumerable<ConversationTrigger> triggers, IEnumerable<Sector> sectors) {
            base.ConversationEnd(triggers, sectors);
            if(!_standUp) {
                _standUp = true;
                IsActivated = true;
                PickUpChert.Locomotion.RiebeckStandUp();
            }
        }

        protected override void MoveTo(Transform target, float radius, float speed, Vector3 offset) {
            PickUpChert.Locomotion.RiebeckMoveTo(target, radius, speed * 0.7f, offset);
        }

        protected override void MoveStop() {
            PickUpChert.Locomotion.RiebeckMoveStop();
        }

        protected override void Sitting() {
            PickUpChert.Locomotion.RiebeckSitting(!IsInShip);
        }

        protected override void StandUp() {
            PickUpChert.Locomotion.RiebeckStandUp();
        }

        public override void LookAt(Transform target, Vector3 offset) {
            PickUpChert.Locomotion.RiebeckLookAt(target, offset);
        }

        public override void DisableRigidbody(Transform newParent) {
            PickUpChert.Locomotion.RiebeckDisableRigidbody(newParent);
        }

        public override void EnableRigidbody() {
            PickUpChert.Locomotion.RiebeckEnableRigidbody();
        }

        //public override void SuspendInShipPosition() {
        //    base.SuspendInShipPosition();

        //    if (_movedToShipTweenPos != null) {
        //        _movedToShipTweenPos.Kill();
        //        _movedToShipTweenPos = null;
        //    }
        //    if (_movedToShipTweenRot != null) {
        //        _movedToShipTweenRot.Kill();
        //        _movedToShipTweenRot = null;
        //    }
        //    _movedToShipTweenPos = transform.DOLocalMove(new Vector3(-2.278f, 0.8706f, -0.8103f), 0.5f).SetUpdate(UpdateType.Fixed).OnComplete(() => {
        //        PickUpChert.Locomotion.RiebeckSitting(false);
        //        _movedToShipPos = true;
        //    }).SetLink(gameObject);
        //    _movedToShipTweenRot = transform.DOLocalRotate(new Vector3(0, 90, 0), 0.5f).SetUpdate(UpdateType.Fixed).SetLink(gameObject);
        //}

        //protected override void FixedUpdate() {
        //    base.FixedUpdate();
        //    if(!SuspendedInShip) {
        //        _movedToShipPos = false;
        //        if(_movedToShipTweenPos != null) {
        //            _movedToShipTweenPos.Kill();
        //            _movedToShipTweenPos = null;
        //        }
        //        if (_movedToShipTweenRot == null) {
        //            _movedToShipTweenRot.Kill();
        //            _movedToShipTweenRot = null;
        //        }
        //        return;
        //    }
        //    if (_movedToShipPos) {
        //        transform.localPosition = new Vector3(-2.278f, 0.8706f, -0.8103f);
        //        transform.localEulerAngles = new Vector3(0, 90, 0);
        //    }
        //}
    }
}
