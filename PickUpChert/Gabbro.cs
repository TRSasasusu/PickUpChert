using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace PickUpChert {
    public class Gabbro : Traveler {
        bool _standUp;
        string _conversationCondition;

        public override void ConversationStart() {
            if(_standUp) {
                _conversationCondition = "PUC_GABBRO_AFTER_START";
                DialogueConditionManager.SharedInstance.SetConditionState(_conversationCondition, true);
            }
        }

        public override void ConversationEnd() {
            if(!_standUp) {
                _standUp = true;
                PickUpChert.Locomotion.GabbroStandUp();
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => {
                    ModifyObjects.Gabbro.GoToShip();
                }).AddTo(this);
            }
        }

        protected override void Update() {
            base.Update();
        }
    }
}
