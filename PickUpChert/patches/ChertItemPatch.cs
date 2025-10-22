using HarmonyLib;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    [HarmonyPatch]
    public static class ChertItemPatch {
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(ItemTool), nameof(ItemTool.MoveItemToCarrySocket))]
        //public static bool ItemTool_MoveItemToCarrySocket_Prefix(OWItem item, ItemTool __instance) {

        //}
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OWItem), nameof(OWItem.MoveAndChildToTransform))]
        public static void OWItem_MoveAndChildToTransform_Prefix(ref Transform socketTransform, OWItem __instance) {
            if(socketTransform.parent.name != "ItemCarryTool") {
                return;
            }
            if (__instance.name == "Traveller_HEA_Chert") {
                socketTransform = BringChert.Instance.ChertSocket;
                for (var i = BringChert.Instance.Sector_Lakebed._staticRenderers.Count - 1; i >= 0; i--) {
                    var name = BringChert.Instance.Sector_Lakebed._staticRenderers[i].name;
                    if (name == "NewDrum:polySurface1" || name == "NewDrum:polySurface2" || name == "Chert_Skin_02:Chert_Mesh:Traveller_HEA_Chert 1" || name == "Chert_DrumStick_Geo1") {
                        BringChert.Instance.Sector_Lakebed._staticRenderers.RemoveAt(i); // remove Chert parts from a disabled renderer queue of the sector
                    }
                }
            }
            else if(ChertItem.Instance && ChertItem.Instance.Brought) {
                socketTransform = BringChert.Instance.ChertRightHand; // set socket as the right hand of Chert
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OWItem), nameof(OWItem.MoveAndChildToTransform))]
        public static void OWItem_MoveAndChildToTransform_Postfix(ref Transform socketTransform, OWItem __instance) {
            if(socketTransform == BringChert.Instance.ChertRightHand) {
                if(__instance._type == ItemType.DreamLantern) {
                    __instance.transform.localPosition = new Vector3(0.4f, -0.2f, 0.9f);
                    __instance.transform.localEulerAngles = new Vector3(0, 90, 250);
                    foreach(var renderer in __instance.transform.GetComponentsInChildren<MeshRenderer>(true)) {
                        if(renderer.name.Contains("ViewModelPrepass")) { // dream lantern has special parts displayed above any objects including Chert, so disable them.
                            renderer.gameObject.SetActive(false);
                        }
                    }
                }
                else {
                    __instance.transform.localPosition = new Vector3(0.2f, -0.1f, 0);
                    __instance.transform.localEulerAngles = new Vector3(330, 280, 0);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StreamingRenderMeshHandle), nameof(StreamingRenderMeshHandle.UnloadMesh))]
        public static bool StreamingSkinnedMeshHandle_UnloadMesh_Prefix(StreamingRenderMeshHandle __instance) {
            if (__instance.name == "NewDrum:polySurface1" || __instance.name == "NewDrum:polySurface2" || __instance.name == "Chert_DrumStick_Geo1") {
                PickUpChert.Log($"unload is avoided on {__instance.name}");
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StreamingSkinnedMeshHandle), nameof(StreamingSkinnedMeshHandle.UnloadMesh))]
        public static bool StreamingSkinnedMeshHandle_UnloadMesh_Prefix(StreamingSkinnedMeshHandle __instance) {
            if (__instance.name == "Chert_Skin_02:Chert_Mesh:Traveller_HEA_Chert 1") {
                PickUpChert.Log($"unload is avoided on {__instance.name}");
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StreamingAssetBundle), nameof(StreamingAssetBundle.UnloadImmediate))]
        public static void StreamingAssetBundle_UnloadImmediate_Prefix(StreamingAssetBundle __instance) {
            if (__instance._assetBundleName == "hourglasstwins/meshes/characters") {
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
            if (__instance.name == "Traveller_HEA_Chert") {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.Update))]
        public static void PlayerTool_Update_Prefix(PlayerTool __instance) {
            if (!(__instance is ItemTool)) {
                return;
            }
            if (BringChert.Instance == null || !BringChert.Instance.ChertSocket || !BringChert.Instance.ShipCockpitController) {
                return;
            }

            if (BringChert.Instance.ChertSocket.transform.childCount > 0 && BringChert.Instance.ShipCockpitController._playerAtFlightConsole) { // keep holding Chert when player is in cockpit
                __instance._isPuttingAway = false;
                __instance._isEquipped = false;
                __instance.enabled = false;
                __instance._moveSpring.ResetVelocity();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TravelerAudioManager), nameof(TravelerAudioManager.OnUnpause))]
        public static void TravelerAudioManager_OnUnpause_Postfix() {
            if(!ChertItem.Instance || BringChert.Instance == null || !BringChert.Instance.SignalDrums) {
                return;
            }
            if(!ChertItem.Instance.Playing) {
                BringChert.Instance.SignalDrums.GetOWAudioSource().Stop();
            }
        }

        // this method prevents to play drums on ending talk when stopping the playing
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OWAudioSource), nameof(OWAudioSource.FadeIn))]
        public static bool OWAudioSource_FadeIn_Prefix(OWAudioSource __instance) {
            if(!ChertItem.Instance || BringChert.Instance == null || !BringChert.Instance.SignalDrums) {
                return true;
            }
            if(__instance != BringChert.Instance.SignalDrums.GetOWAudioSource()) {
                return true;
            }
            if(!ChertItem.Instance.Playing) {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TravelerController), nameof(TravelerController.OnUnpause))]
        public static bool TravelerController_OnUnpause_Prefix(TravelerController __instance) {
            if(!(__instance is ChertTravelerController)) {
                return true;
            }
            if(!ChertItem.Instance || BringChert.Instance == null || !BringChert.Instance.SignalDrums) {
                return true;
            }
            if(!ChertItem.Instance.Playing) {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChertDialogueSwapper), nameof(ChertDialogueSwapper.OnStartConversation))]
        public static bool ChertDialogueSwapper_OnStartConversation_Prefix() {
            if(ChertPickUpConversation.Instance) {
                return ChertPickUpConversation.Instance.OnStartConversation();
            }
            return true;
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemTool), nameof(ItemTool.UpdateInteract))]
        public static void ItemTool_UpdateInteract_Prefix(FirstPersonManipulator firstPersonManipulator, ItemTool __instance) {
            if(!ChertItem.Instance || BringChert.Instance == null || !BringChert.Instance.SignalDrums) {
                return;
            }
            if(!ChertItem.Instance.Brought) {
                return;
            }
            if(__instance._heldItem != ChertItem.Instance) {
                return;
            }

            var focusedOWItem = firstPersonManipulator.GetFocusedOWItem();
            var focusedItemSocket = firstPersonManipulator.GetFocusedItemSocket();
            if(focusedOWItem || (focusedItemSocket && focusedItemSocket.IsSocketOccupied())) {
                __instance._heldItem = null;
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemTool), nameof(ItemTool.UpdateInteract))]
        public static void ItemTool_UpdateInteract_Postfix(ItemTool __instance) {
            if(!__instance._heldItem && ChertItem.Instance && ChertItem.Instance.Brought) {
                __instance._heldItem = ChertItem.Instance;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DialogueNode), nameof(DialogueNode.GetNextPage))]
        public static void DialogueNode_GetNextPage(string mainText, List<DialogueOption> options) {
            PickUpChert.Log(mainText);
        }
    }
}
