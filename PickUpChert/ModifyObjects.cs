using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace PickUpChert {
    public class ModifyObjects {
        const string SHORTCUT_CHERT_PATH = "TimberHearth_Body/Sector_TH/shortcut_chert";
        const string GABBRO_PATH = "GabbroIsland_Body/Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/Traveller_HEA_Gabbro_ANIM_IdleFlute";

        Coroutine _initializeBody;

        public static Traveler Gabbro { get; private set; }

        public void Initialize() {
            _initializeBody = PickUpChert.Instance.StartCoroutine(InitializeBody());
        }

        public void DestroyResources() {
            if(_initializeBody != null) {
                PickUpChert.Instance.StopCoroutine(_initializeBody);
                _initializeBody = null;
            }
        }

        IEnumerator InitializeBody() {
            GameObject shortcutChert = null;
            while(true) {
                shortcutChert = GameObject.Find(SHORTCUT_CHERT_PATH);
                if(shortcutChert) {
                    shortcutChert.AddComponent<ShortCutChert>();
                    break;
                }
                yield return null;
            }

            GameObject gabbro = null;
            while(true) {
                gabbro = GameObject.Find(GABBRO_PATH);
                if(gabbro) {
                    PickUpChert.Locomotion.GabbroInitialize(gabbro);
                    Gabbro = gabbro.AddComponent<Traveler>();
                    break;
                }
                yield return null;
            }
        }
    }
}
