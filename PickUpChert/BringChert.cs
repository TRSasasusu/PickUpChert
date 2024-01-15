using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace PickUpChert {
    public class BringChert {
        const string PATH_CHERT = "CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Traveller_HEA_Chert";

        public static BringChert Instance;

        public GameObject _chert;
        public Transform ChertSocket { get; private set; }

        public BringChert() {
            Instance = this;
        }

        public void Initialize() {
            PickUpChert.Instance.StartCoroutine(InitializeBody());
        }

        IEnumerator InitializeBody() {
            while(true) {
                yield return null;
                _chert = GameObject.Find(PATH_CHERT);
                if(_chert) {
                    break;
                }
            }

            var conversationZone = _chert.transform.Find("ConversationZone_Chert");
            conversationZone.transform.localPosition = new Vector3(0.009f, 0.363f, 0.355f);

            _chert.AddComponent<ChertItem>();

            var sphereCollider = _chert.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            //sphereCollider.enabled = false;
            sphereCollider.radius = 0.75f;
            sphereCollider.center = new Vector3(0, 0.5f, -1f);

            var owCollider = _chert.AddComponent<OWCollider>();

            GameObject defaultItemSocket;
            while(true) {
                yield return null;
                defaultItemSocket = GameObject.Find("Player_Body/PlayerCamera/ItemCarryTool/ItemSocket");
                if(defaultItemSocket) {
                    break;
                }
            }
            ChertSocket = GameObject.Instantiate(defaultItemSocket).transform;
            ChertSocket.gameObject.name = "ChertSocket";
            ChertSocket.parent = defaultItemSocket.transform.parent;
            ChertSocket.localPosition = new Vector3(0.185f, -0.32f, 0.37f);
            ChertSocket.localEulerAngles = new Vector3(0, 350, 15);
        }
    }
}
