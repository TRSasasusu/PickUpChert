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
        const string GABBRO_INITIAL_PROBE_PATH = "GabbroIsland_Body/Sector_GabbroIsland/PUCPathProbesGabbroIsland/AfterHammock";

        Coroutine _initializeBody;

        public static Hatchling Hatchling { get; private set; }
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

            while(true) {
                var playerBody = Locator.GetPlayerBody();
                if (playerBody) {
                    Hatchling = playerBody.gameObject.AddComponent<Hatchling>();
                    break;
                }
                yield return null;
            }

            GameObject originalGabbro = null;
            while (true) {
                originalGabbro = GameObject.Find(GABBRO_PATH);
                if (originalGabbro) {
                    PickUpChert.Locomotion.GabbroInitialize(originalGabbro);
                    break;
                }
                yield return null;
            }

            while (true) {
                if(PickUpChert.Locomotion.GabbroIsInitialized()) {
                    var gabbro = PickUpChert.Locomotion.GetGabbro();
                    Gabbro = gabbro.AddComponent<Gabbro>();
                    break;
                }
                yield return null;
            }

            GameObject gabborInitialProbe = null;
            while(true) {
                gabborInitialProbe = GameObject.Find(GABBRO_INITIAL_PROBE_PATH);
                if (gabborInitialProbe) {
                    Gabbro.AddStackedPathProbe(gabborInitialProbe.GetComponent<PathProbe>());
                    break;
                }
                yield return null;
            }

            while(true) {
                var dialogue = GameObject.FindWithTag("DialogueGui").GetRequiredComponent<DialogueBoxVer2>();
                if(dialogue != null) {
                    dialogue.gameObject.AddComponent<MovingConversation>();
                    break;
                }
                yield return null;
            }

            while (true) {
                var ship = Locator.GetShipBody();
                if (ship != null) {
                    var objs = new GameObject[] {
                        new GameObject("probe_0"),
                        new GameObject("probe_1"),
                        new GameObject("probe_2"),
                    };
                    var positions = new Vector3[] {
                        new Vector3(1, 0, 0),
                        new Vector3(-1, 0, 0),
                        new Vector3(0, 0, 1),
                    };
                    foreach (var obj in objs) {
                        obj.transform.parent = ship.transform;
                        obj.transform.localPosition = new Vector3(1, 0, 0);
                        obj.transform.localEulerAngles = Vector3.zero;
                        var collider = obj.AddComponent<SphereCollider>();
                        collider.isTrigger = true;
                        //collider.enabled = false; // false now!! so I need to enable it when going to ship
                        var probe = obj.AddComponent<PathProbe>();
                        //probe._isStackedForShip = true;
                    }
                    break;
                }
                yield return null;
            }
        }
    }
}
