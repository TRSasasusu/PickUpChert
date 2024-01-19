using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    [HarmonyPatch]
    public static class Patch {
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(ItemTool), nameof(ItemTool.MoveItemToCarrySocket))]
        //public static bool ItemTool_MoveItemToCarrySocket_Prefix(OWItem item, ItemTool __instance) {

        //}
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OWItem), nameof(OWItem.MoveAndChildToTransform))]
        public static void OWItem_MoveAndChildToTransform_Prefix(ref Transform socketTransform, OWItem __instance) {
            if(__instance.name == "Traveller_HEA_Chert") {
                socketTransform = BringChert.Instance.ChertSocket;
                for(var i = BringChert.Instance.Sector_Lakebed._staticRenderers.Count - 1; i >= 0; i--) {
                    var name = BringChert.Instance.Sector_Lakebed._staticRenderers[i].name;
                    if(name == "NewDrum:polySurface1" || name == "NewDrum:polySurface2" || name == "Chert_Skin_02:Chert_Mesh:Traveller_HEA_Chert 1" || name == "Chert_DrumStick_Geo1") {
                        BringChert.Instance.Sector_Lakebed._staticRenderers.RemoveAt(i);
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StreamingRenderMeshHandle), nameof(StreamingRenderMeshHandle.UnloadMesh))]
        public static bool StreamingSkinnedMeshHandle_UnloadMesh_Prefix(StreamingRenderMeshHandle __instance) {
            if(__instance.name == "NewDrum:polySurface1" || __instance.name == "NewDrum:polySurface2" || __instance.name == "Chert_DrumStick_Geo1") {
                PickUpChert.Log($"unload is avoided on {__instance.name}");
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StreamingSkinnedMeshHandle), nameof(StreamingSkinnedMeshHandle.UnloadMesh))]
        public static bool StreamingSkinnedMeshHandle_UnloadMesh_Prefix(StreamingSkinnedMeshHandle __instance) {
            if(__instance.name == "Chert_Skin_02:Chert_Mesh:Traveller_HEA_Chert 1") {
                PickUpChert.Log($"unload is avoided on {__instance.name}");
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StreamingAssetBundle), nameof(StreamingAssetBundle.UnloadImmediate))]
        public static void StreamingAssetBundle_UnloadImmediate_Prefix(StreamingAssetBundle __instance) {
            if(__instance._assetBundleName == "hourglasstwins/meshes/characters") {
                PickUpChert.Log($"unload is avoided on {__instance._assetBundleName}");
                __instance._assetBundle.Unload(false); // to avoid unloading them even if they are used
                __instance._assetBundle = null;
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(StreamingMeshAssetBundle), nameof(StreamingMeshAssetBundle.UnloadImmediate)]
        //public static void StreamingMeshAssetBundle_UnloadImmediate_Prefix(StreamingMeshAssetBundle __instance) {
        //    for(var i = __instance._streamingMeshHandles)
        //}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TravelerController), nameof(TravelerController.OnSectorOccupantsUpdated))]
        public static bool TravelerController_OnSectorOccupantsUpdated_Prefix(TravelerController __instance) {
            if(__instance.name == "Traveller_HEA_Chert") {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.Update))]
        public static void PlayerTool_Update_Prefix(ItemTool __instance) {
            if(BringChert.Instance == null || !BringChert.Instance.ChertSocket || !BringChert.Instance.ShipCockpitController) {
                return;
            }

            if(BringChert.Instance.ChertSocket.transform.childCount > 0 && BringChert.Instance.ShipCockpitController._playerAtFlightConsole) {
                __instance._isPuttingAway = false;
                __instance._isEquipped = false;
                __instance.enabled = false;
                __instance._moveSpring.ResetVelocity();
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.UnequipTool))]
        //public static bool ToolModeSwapper_UnequipTool_Prefix() {
        //    return !(BringChert.Instance.ChertSocket.transform.childCount > 0 && BringChert.Instance.ShipCockpitController._playerAtFlightConsole); // it leads to not allowed to cancel cockpit...
        //}

        //[HarmonyPrefix] // only to find sector cull group of chert
        //[HarmonyPatch(typeof(CullGroup), nameof(CullGroup.SetRenderersEnabled))]
        //public static void CullGroup_SetRenderersEnabled_Prefix(CullGroup __instance) {
        //    foreach(var owRenderer in __instance._staticRenderers) {
        //        if(owRenderer.name.Contains("Chert")) {
        //            PickUpChert.Log($"SectorCullGroup found!: {__instance.name} to {owRenderer.name}");
        //        }
        //    }
        //    foreach(var owRenderer in __instance._dynamicRenderers) {
        //        if(owRenderer.name.Contains("Chert")) {
        //            PickUpChert.Log($"SectorCullGroup found!: {__instance.name} to {owRenderer.name}");
        //        }
        //    }
        //}
    }
}
