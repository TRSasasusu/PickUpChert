using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class Riebeck : Traveler {
        bool _standUp;


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
            PickUpChert.Locomotion.RiebeckSitting();
        }

        protected override void StandUp() {
            PickUpChert.Locomotion.RiebeckStandUp();
        }

        public override void LookAt(Transform target, Vector3 offset) {
            PickUpChert.Locomotion.RiebeckLookAt(target, offset);
        }
    }
}
