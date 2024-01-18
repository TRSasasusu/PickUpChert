﻿using System;
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

        public GameObject _chert;
        public Transform ChertSocket { get; private set; }
        public SectorCullGroup Sector_Lakebed { get; private set; }
        public Mesh Drum { get; private set; }
        public Mesh DrumStick { get; private set; }

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
                _chert = GameObject.Find(PATH_CHERT);
                if(_chert) {
                    break;
                }
            }
            Sector_Lakebed = _chert.transform.parent.GetComponent<SectorCullGroup>();

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

            //var streamingAssetBundle = new StreamingAssetBundle("hourglasstwins/meshes/characters");
            //streamingAssetBundle.Load();
            //while(true) {
            //    yield return null;
            //    if(streamingAssetBundle._loadBundleOperation.isDone) {
            //        PickUpChert.Log("loading chert assetbundle is done");
            //        break;
            //    }
            //}
            //_assetBundle = streamingAssetBundle._loadBundleOperation.assetBundle;
            //Drum = _assetBundle.LoadAsset<Mesh>("Assets/Scenes/HourglassTwins/Streamingmeshes_characters/NewDrum_polySurface2.asset");
            //DrumStick = _assetBundle.LoadAsset<Mesh>("Assets/Scenes/HourglassTwins/Streamingmeshes_characters/Chert_DrumStick_Geo1.asset");
        }
    }
}
