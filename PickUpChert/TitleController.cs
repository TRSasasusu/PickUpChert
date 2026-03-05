using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace PickUpChert {
    public class TitleController {
        const string ASSETBUNDLE_NAME = "hourglasstwins/meshes/characters";
        const string CHERT_PATH = "Scene/Background/PlanetPivot/chert";

        StreamingAssetBundleState _chertAssetBundleState;
        Coroutine _loadChert;

        public void DestroyResources() {
            if(_loadChert != null) {
                PickUpChert.Instance.StopCoroutine(_loadChert);
                _loadChert = null;
            }
        }

        public void OnSceneLoad() {
            PickUpChert.Log("title controller is called!");
            _loadChert = PickUpChert.Instance.StartCoroutine(LoadChert());
        }

        IEnumerator LoadChert() {
            while(true) {
                if(StreamingManager.isStreamingEnabled) {
                    break;
                }
                yield return null;
            }

            StreamingManager.LoadStreamingAssets(ASSETBUNDLE_NAME, 0);
            _chertAssetBundleState = StreamingManager.GetStreamingAssetBundleState(ASSETBUNDLE_NAME);

            while (true) {
                if(_chertAssetBundleState.isLoaded) {
                    break;
                }
                yield return null;
            }

            GameObject chert = null;
            while(true) {
                chert = GameObject.Find(CHERT_PATH);
                if(chert) {
                    break;
                }
                yield return null;
            }

            var names = _chertAssetBundleState._streamingAssetBundle._assetBundle.GetAllAssetNames();
            foreach(var name in names) {
                PickUpChert.Log(name);
            }
            var streamingMeshAssetBundle = (StreamingMeshAssetBundle)_chertAssetBundleState._streamingAssetBundle;

            var streamingRenderMeshHandles = chert.GetComponentsInChildren<StreamingRenderMeshHandle>();
            foreach(var handle in streamingRenderMeshHandles) {
                PickUpChert.Log(handle.name);
                if(handle.name == "Chert_DrumStick_Geo1") {
                    foreach (var op in streamingMeshAssetBundle._loadAssetOperations) {
                        if(op.asset.name == "Chert_DrumStick_Geo1") {
                            handle.LoadMesh(op.asset as Mesh);
                            break;
                        }
                    }
                }
                else if(handle.name == "NewDrum:polySurface2") {
                    foreach (var op in streamingMeshAssetBundle._loadAssetOperations) {
                        if(op.asset.name == "NewDrum_polySurface2") {
                            handle.LoadMesh(op.asset as Mesh);
                            break;
                        }
                    }
                }
            }
            var streamingSkinnedMeshHandle = chert.GetComponentInChildren<StreamingSkinnedMeshHandle>();
            PickUpChert.Log(streamingSkinnedMeshHandle.name);
            foreach(var op in streamingMeshAssetBundle._loadAssetOperations) {
                if(op.asset.name == "Chert_Skin_02_Chert_Mesh_Traveller_HEA_Chert") {
                    streamingSkinnedMeshHandle.LoadMesh(op.asset as Mesh);
                    break;
                }
            }
            //streamingMeshAssetBundle._loadAssetOperations[0].asset.name

            //((StreamingMeshAssetBundle)_chertAssetBundleState._streamingAssetBundle).l

            //var assetbundle = _chertAssetBundleState._streamingAssetBundle._assetBundle;
            //var prefabReq = assetbundle.LoadAllAssetsAsync<GameObject>();
            //yield return prefabReq;
            //var prefabs = prefabReq.allAssets as GameObject[];
            //foreach (var prefab in prefabs) {
            //    GameObject.Instantiate(prefab);
            //}
        }
    }
}
