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

        static AssetBundle _assetBundle;

        public GameObject Chert { get; private set; }
        public ChertTravelerController ChertTraveler { get; private set; }
        public Transform ChertSocket { get; private set; }
        public SectorCullGroup Sector_Lakebed { get; private set; }
        public Mesh Drum { get; private set; }
        public Mesh DrumStick { get; private set; }
        public ShipCockpitController ShipCockpitController { get; private set; }
        public AudioSignal SignalDrums { get; private set; }
        public ScreenPrompt StopDrumPrompt { get; private set; }
        public ScreenPrompt PlayDrumPrompt { get; private set; }
        public PlayerSectorDetector SectorDetector { get; private set; }
        public CharacterDialogueTree ChertDialogueTree { get; private set; }

        public BringChert() {
            Instance = this;
        }

        public void Initialize() {
            PickUpChert.Instance.StartCoroutine(InitializeBody());
        }

        IEnumerator InitializeBody() {
            //if(_assetBundle != null) {
            //    _assetBundle.Unload(true);
            //    _assetBundle = null;
            //}

            while(true) {
                yield return null;
                Chert = GameObject.Find(PATH_CHERT);
                if(Chert) {
                    break;
                }
            }
            Sector_Lakebed = Chert.transform.parent.GetComponent<SectorCullGroup>();

            var conversationZone = Chert.transform.Find("ConversationZone_Chert");
            conversationZone.transform.localPosition = new Vector3(0.009f, 0.363f, 0.355f);

            Chert.AddComponent<ChertItem>();

            var sphereCollider = Chert.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            //sphereCollider.enabled = false;
            sphereCollider.radius = 0.75f;
            sphereCollider.center = new Vector3(0, 0.5f, -1f);

            var owCollider = Chert.AddComponent<OWCollider>();

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

            GameObject shipCockpitController;
            while(true) {
                yield return null;
                shipCockpitController = GameObject.Find("Ship_Body/Module_Cockpit/Systems_Cockpit/ShipCockpitController");
                if(shipCockpitController) {
                    ShipCockpitController = shipCockpitController.GetComponent<ShipCockpitController>();
                    break;
                }
            }

            GameObject signalDrums;
            while(true) {
                yield return null;
                signalDrums = GameObject.Find("CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Volumes_Lakebed/Signal_Drums");
                if(signalDrums) {
                    SignalDrums = signalDrums.GetComponent<AudioSignal>();
                    break;
                }
            }
            SignalDrums.transform.parent = Chert.transform;

            StopDrumPrompt = new ScreenPrompt(InputLibrary.toolActionSecondary, TextTranslation.Translate("Stop Drums") + "   <CMD>", 0, ScreenPrompt.DisplayState.Normal, false);
            PlayDrumPrompt = new ScreenPrompt(InputLibrary.toolActionSecondary, TextTranslation.Translate("Play Drums") + "   <CMD>", 0, ScreenPrompt.DisplayState.Normal, false);
            Locator.GetPromptManager().AddScreenPrompt(StopDrumPrompt, PromptPosition.UpperRight, false);
            Locator.GetPromptManager().AddScreenPrompt(PlayDrumPrompt, PromptPosition.UpperRight, false);
            StopDrumPrompt.SetVisibility(false);
            PlayDrumPrompt.SetVisibility(false);

            ChertTraveler = Chert.GetComponent<ChertTravelerController>();

            GameObject sectorDetector;
            while(true) {
                yield return null;
                sectorDetector = GameObject.Find("Player_Body/PlayerDetector");
                if(sectorDetector) {
                    SectorDetector = sectorDetector.GetComponent<PlayerSectorDetector>();
                    break;
                }
            }

            ChertDialogueTree = Chert.transform.Find("ConversationZone_Chert").GetComponent<CharacterDialogueTree>();
            Chert.AddComponent<ChertPickUpConversation>();
        }
    }
}
