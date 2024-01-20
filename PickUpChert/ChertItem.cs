using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickUpChert {
    public class ChertItem : OWItem {
        bool _playing = true;

        public override string GetDisplayName() {
            return "Chert";
        }

        void Update() {
            if(BringChert.Instance == null || !BringChert.Instance.ChertSocket || BringChert.Instance.StopDrumPrompt == null || BringChert.Instance.PlayDrumPrompt == null) {
                return;
            }

            BringChert.Instance.StopDrumPrompt.SetVisibility(false);
            BringChert.Instance.PlayDrumPrompt.SetVisibility(false);
            if(BringChert.Instance.ChertSocket.transform.childCount > 0) {
                var pressAction = OWInput.IsNewlyPressed(InputLibrary.toolActionSecondary);
                if(_playing) {
                    BringChert.Instance.StopDrumPrompt.SetVisibility(true);
                    if(pressAction) {
                        _playing = false;
                        BringChert.Instance.SignalDrums._active = false;
                        BringChert.Instance.SignalDrums.GetOWAudioSource().FadeOut(0.5f, OWAudioSource.FadeOutCompleteAction.STOP, 0f);
                        BringChert.Instance.ChertTraveler._animator.SetTrigger("Talking");
                    }
                }
                else {
                    BringChert.Instance.PlayDrumPrompt.SetVisibility(true);
                    if(pressAction) {
                        _playing = true;
                        BringChert.Instance.SignalDrums._active = true;
                        BringChert.Instance.SignalDrums.GetOWAudioSource().FadeIn(0.5f, false, false, 1f);
                        BringChert.Instance.SignalDrums.GetOWAudioSource().timeSamples = 0;
                        BringChert.Instance.ChertTraveler._animator.SetTrigger("Playing");
                    }
                }
                //BringChert.Instance.StopDrumPrompt
            }
        }
    }
}
