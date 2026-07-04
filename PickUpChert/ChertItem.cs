using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace PickUpChert {
    public class ChertItem : OWItem {
        public static ChertItem Instance;

        public bool Playing { get; private set; } = true;
        public static bool Brought {
            get {
                return BringChert.Instance != null && BringChert.Instance.ChertSocket && BringChert.Instance.ChertSocket.transform.childCount > 0;
            }
        }
        public bool _inDream;

        bool _prevBrought;

        public override void Awake() {
            base.Awake();
            Instance = this;

            _localDropOffset = new Vector3(0, -0.3f, 0.5f);
        }

        public override string GetDisplayName() {
            return TextTranslation.Translate("Chert");
        }

        public void StopDrum() {
            Playing = false;
            PickUpChert.Locomotion.ChertStopPlaying();
        }

        public void PlayDrum() {
            Playing = true;
            PickUpChert.Locomotion.ChertStartPlaying();
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
                //var pressAction = OWInput.IsNewlyPressed(InputLibrary.toolActionSecondary);
                var pressAction = OWInput.IsNewlyPressed(InputLibrary.toolOptionDown);
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

                if(!_prevBrought) {
                    Observable.NextFrame().Subscribe(_ => {
                        PickUpChert.Locomotion.ChertEndOnFloor();
                    }).AddTo(this);
                    _prevBrought = true;
                }
            }
            else {
                if(_prevBrought) {
                    Observable.NextFrame().Subscribe(_ => {
                        PickUpChert.Locomotion.ChertSitDown();
                    }).AddTo(this);
                    _prevBrought = false;
                }
            }
        }
    }
}
