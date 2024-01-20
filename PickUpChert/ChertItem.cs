using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickUpChert {
    public class ChertItem : OWItem {
        public static ChertItem Instance;

        public bool Playing { get; private set; } = true;

        public override void Awake() {
            base.Awake();
            Instance = this;
        }

        public override string GetDisplayName() {
            return TextTranslation.Translate("Chert");
        }

        public void StopDrum() {
            Playing = false;
            BringChert.Instance.SignalDrums._active = false;
            BringChert.Instance.SignalDrums.GetOWAudioSource().FadeOut(0.5f, OWAudioSource.FadeOutCompleteAction.STOP, 0f);
            BringChert.Instance.ChertTraveler._animator.SetTrigger("Talking");
        }

        public void PlayDrum() {
            Playing = true;
            BringChert.Instance.SignalDrums._active = true;
            BringChert.Instance.SignalDrums.GetOWAudioSource().FadeIn(0.5f, false, false, 1f);
            BringChert.Instance.SignalDrums.GetOWAudioSource().timeSamples = 0;
            BringChert.Instance.ChertTraveler._animator.SetTrigger("Playing");
        }

        void Update() {
            if(BringChert.Instance == null || !BringChert.Instance.ChertSocket || BringChert.Instance.StopDrumPrompt == null || BringChert.Instance.PlayDrumPrompt == null) {
                return;
            }

            BringChert.Instance.StopDrumPrompt.SetVisibility(false);
            BringChert.Instance.PlayDrumPrompt.SetVisibility(false);

            if(Locator.GetSceneMenuManager() && Locator.GetSceneMenuManager().pauseMenu && Locator.GetSceneMenuManager().pauseMenu._isPaused) {
                return;
            }

            //if(!_playing && BringChert.Instance.SignalDrums.GetOWAudioSource().isPlaying) { // it would be caused by pausing and TravelerAudioManager
            //    StopDrum();
            //}

            if(BringChert.Instance.ChertSocket.transform.childCount > 0) {
                var pressAction = OWInput.IsNewlyPressed(InputLibrary.toolActionSecondary);
                if(Playing) {
                    BringChert.Instance.StopDrumPrompt.SetVisibility(true);
                    if(pressAction) {
                        StopDrum();
                    }
                }
                else {
                    BringChert.Instance.PlayDrumPrompt.SetVisibility(true);
                    if(pressAction) {
                        PlayDrum();
                    }
                }
                //BringChert.Instance.StopDrumPrompt
            }
        }
    }
}
