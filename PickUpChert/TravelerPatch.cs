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
    }
}
