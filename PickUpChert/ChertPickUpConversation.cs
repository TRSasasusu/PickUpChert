using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class ChertPickUpConversation : MonoBehaviour {
        public static ChertPickUpConversation Instance;

        enum InitialPickUpState {
            BEFORE_PICKUP,
            AFTER_PICKUP,
            END_INITIAL_PICKUP,
        }
        InitialPickUpState _initialPickUpState = InitialPickUpState.BEFORE_PICKUP;

        static TextAsset _initialPickUpXML;

        public static void Initialize() {
            var path = PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + $"assets/dialogues/initial_pick_up.xml";
            _initialPickUpXML = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(path));
        }

        public bool OnStartConversation() {
            if(_initialPickUpState == InitialPickUpState.AFTER_PICKUP) {
                BringChert.Instance.ChertDialogueTree.SetTextXml(_initialPickUpXML);
                _initialPickUpState = InitialPickUpState.END_INITIAL_PICKUP;
                return false;
            }
            return true;
        }

        void Awake() {
            Instance = this;
        }

        void Update() {
            if(BringChert.Instance == null || !BringChert.Instance.SectorDetector) {
                return;
            }
            if(_initialPickUpState == InitialPickUpState.BEFORE_PICKUP) {
                if(ChertItem.Instance.Brought) {
                    _initialPickUpState = InitialPickUpState.AFTER_PICKUP;
                }
                else {
                    return;
                }
            }

            var sector = BringChert.Instance.SectorDetector.GetLastEnteredSector();
        }
    }
}
