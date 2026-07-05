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
                    Gabbro.Initialize();
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
                        new Vector3(-5.381f, -3.9314f, 3.7078f),
                        new Vector3(4.5668f, -3.8204f, 3.2332f),
                        new Vector3(-0.0396f, -3.7802f, -6.2688f),
                    };
                    for(var i = 0; i < objs.Length; ++i) {
                    //foreach (var obj in objs) {
                        var obj = objs[i];
                        var pos = positions[i];

                        obj.transform.parent = ship.transform;
                        obj.transform.localPosition = pos;
                        obj.transform.localEulerAngles = Vector3.zero;
                        var collider = obj.AddComponent<SphereCollider>();
                        collider.isTrigger = true;
                        collider.radius = 1f;
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
