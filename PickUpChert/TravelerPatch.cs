using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace PickUpChert {
    [HarmonyPatch]
    public static class TravelerPatch {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HatchController), nameof(HatchController.OnEntry))]
        public static bool HatchController_OnEntry_Prefix(HatchController __instance, GameObject hitObj) {
            if(hitObj.gameObject == ModifyObjects.Gabbro.gameObject) {
                ModifyObjects.Gabbro.CompleteEnteringShip();
                if(__instance._isPlayerInShip) {
                    __instance.CloseHatch();
                    Locator.GetShipBody().GetComponentInChildren<ShipTractorBeamSwitch>().DeactivateTractorBeam();
                }
                return false;
            }
            if(hitObj.CompareTag("PlayerDetector")) {
                if(!ModifyObjects.Gabbro.IsActivated || ModifyObjects.Gabbro.IsInShip) {
                    __instance.CloseHatch();
                }
                __instance._isPlayerInShip = true;
                GlobalMessenger.FireEvent("EnterShip");
                ModifyObjects.Hatchling.CompleteEnteringShip();
                if(ModifyObjects.Gabbro.IsActivated && !ModifyObjects.Gabbro.IsInShip) {
                    ModifyObjects.Gabbro.GoToShip();
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipTractorBeamSwitch), nameof(ShipTractorBeamSwitch.OnEnterShip))]
        public static bool ShipTractorBeamSwitch_OnEnterShip_Prefix(ShipTractorBeamSwitch __instance) {
            __instance._isPlayerInShip = true;
            if (!ModifyObjects.Gabbro.IsActivated || ModifyObjects.Gabbro.IsInShip) {
                return true;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HatchController), nameof(HatchController.OnExit))]
        public static bool HatchController_OnExit_Prefix(HatchController __instance, GameObject hitObj) {
            if(hitObj.gameObject == ModifyObjects.Gabbro.gameObject) {
                ModifyObjects.Gabbro.CompleteExitingShip();
                return false;
            }
            if(hitObj.CompareTag("PlayerDetector")) {
                if(ModifyObjects.Gabbro.IsActivated) {
                    ModifyObjects.Gabbro.GoToCenterOfShipToExit();
                }
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipTractorBeamSwitch), nameof(ShipTractorBeamSwitch.OnTriggerExit))]
        public static bool ShipTractorBeamSwitch_OnTriggerExit_Prefix(ShipTractorBeamSwitch __instance, Collider hitCollider) {
            if(hitCollider.gameObject == ModifyObjects.Gabbro.gameObject) {
                PickUpChert.Log("gabbro exit the tractor beam volume");
                ModifyObjects.Gabbro.CompleteExitingShipBeamVolume();
                if(!ModifyObjects.Hatchling.IsInsideShipBeamVolume) {
                    __instance.ActivateTractorBeam();
                }
                return false;
            }
            if(!__instance._isPlayerInShip && __instance._functional && hitCollider.CompareTag("PlayerDetector")) {
                if(!ModifyObjects.Gabbro.IsActivated || !ModifyObjects.Gabbro.IsInsideShipBeamVolume) {
                    PickUpChert.Log("tractor beam is activated");
                    __instance.ActivateTractorBeam();
                }
                ModifyObjects.Hatchling.CompleteExitingShipBeamVolume();
                return false;
            }
            return true;
        }

        // ## campfire
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Campfire), nameof(Campfire.StartRoasting))]
        public static void Campfire_StartRoasting_Postfix(Campfire __instance) {
            if(ModifyObjects.Gabbro.IsActivated) {
                ModifyObjects.Gabbro.StartSitting();
                PickUpChert.Locomotion.GabbroLookAt(__instance.transform, Vector3.zero);
            }
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Campfire), nameof(Campfire.StopRoasting))]
        //public static void Campfire_StopRoasting_Postfix() {
        //    if(ModifyObjects.Gabbro.IsActivated) {
        //        ModifyObjects.Gabbro.StopSitting();
        //    }
        //}
    }
}
