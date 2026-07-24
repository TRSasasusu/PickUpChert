using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace PickUpChert {
    public class Gabbro : Traveler {
        bool _standUp;

        public override TextAsset ConversationStart(IEnumerable<ConversationTrigger> triggers, IEnumerable<Sector> sectors) {
            if(!_standUp) {
                return ChertPickUpConversation._conversationDataDict["Gabbro"]._eventXMLDict["before_standup"];
            }

            DialogueConditionManager.SharedInstance.SetConditionState("wo_chert", !ChertItem.Brought);

            foreach (var trigger in triggers) {
                if (ChertPickUpConversation._conversationDataDict["Gabbro"]._triggerXMLDict.ContainsKey(trigger._conversationFileName)) {
                    return ChertPickUpConversation._conversationDataDict["Gabbro"]._triggerXMLDict[trigger._conversationFileName];
                }
            }
            foreach (var sector in sectors) {
                if(ChertPickUpConversation._conversationDataDict["Gabbro"]._sectorXMLDict.ContainsKey(sector.name)) {
                    return ChertPickUpConversation._conversationDataDict["Gabbro"]._sectorXMLDict[sector.name];
                }
            }

            if(ChertItem.Brought) {
                return ChertPickUpConversation._conversationDataDict["Gabbro"]._eventXMLDict["after_standup"];
            }
            return ChertPickUpConversation._conversationDataDict["Gabbro"]._eventXMLDict["after_standup_wo_chert"];
        }

        public override void ConversationEnd(IEnumerable<ConversationTrigger> triggers, IEnumerable<Sector> sectors) {
            if(!_standUp) {
                _standUp = true;
                PickUpChert.Locomotion.GabbroStandUp();
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => {
                    ModifyObjects.Gabbro.GoToShip();
                }).AddTo(this);
            }
        }

        protected override void MoveTo(Transform target, float radius, float speed, Vector3 offset) {
            PickUpChert.Locomotion.GabbroMoveTo(target, radius, speed, offset);
        }

        protected override void MoveStop() {
            PickUpChert.Locomotion.GabbroMoveStop();
        }

        protected override void Sitting() {
            PickUpChert.Locomotion.GabbroSitting();
        }

        protected override void StandUp() {
            PickUpChert.Locomotion.GabbroStandUp();
        }

        public override void LookAt(Transform target, Vector3 offset) {
            PickUpChert.Locomotion.GabbroLookAt(target, offset);
        }

        public override void DisableRigidbody(Transform newParent) {
            PickUpChert.Locomotion.GabbroDisableRigidbody(newParent);
        }

        public override void EnableRigidbody() {
            PickUpChert.Locomotion.GabbroEnableRigidbody();
        }

        protected override void Update() {
            base.Update();
        }
    }
}
