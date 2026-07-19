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
            bool travelerEnter = false;
            foreach (var traveler in Traveler.GetAllTravelers) {
                if (hitObj.gameObject == traveler.gameObject) {
                    traveler.CompleteEnteringShip();
                    travelerEnter = true;
                    //if (__instance._isPlayerInShip) {
                    //    __instance.CloseHatch();
                    //    Locator.GetShipBody().GetComponentInChildren<ShipTractorBeamSwitch>().DeactivateTractorBeam();
                    //}
                    //return false;
                }
            }
            if(travelerEnter) {
                if(__instance._isPlayerInShip && Traveler.GetAllTravelers.All(x => !x.IsActivated || x.IsInShip)) {
                    __instance.CloseHatch();
                    Locator.GetShipBody().GetComponentInChildren<ShipTractorBeamSwitch>().DeactivateTractorBeam();
                }
                return false;
            }

            if(hitObj.CompareTag("PlayerDetector")) {
                if(Traveler.GetAllTravelers.All(x => !x.IsActivated || x.IsInShip)) {
                //if(!ModifyObjects.Gabbro.IsActivated || ModifyObjects.Gabbro.IsInShip) {
                    __instance.CloseHatch();
                }
                __instance._isPlayerInShip = true;
                GlobalMessenger.FireEvent("EnterShip");
                ModifyObjects.Hatchling.CompleteEnteringShip();
                //if(ModifyObjects.Gabbro.IsActivated && !ModifyObjects.Gabbro.IsInShip) {
                //    ModifyObjects.Gabbro.GoToShip();
                //}
                foreach(var traveler in Traveler.GetAllTravelers) {
                    if(traveler.IsActivated && !traveler.IsInShip) {
                        traveler.GoToShip();
                    }
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipTractorBeamSwitch), nameof(ShipTractorBeamSwitch.OnEnterShip))]
        public static bool ShipTractorBeamSwitch_OnEnterShip_Prefix(ShipTractorBeamSwitch __instance) {
            __instance._isPlayerInShip = true;
            if(Traveler.GetAllTravelers.All(x => !x.IsActivated || x.IsInShip)) {
            //if (!ModifyObjects.Gabbro.IsActivated || ModifyObjects.Gabbro.IsInShip) {
                return true;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HatchController), nameof(HatchController.OnExit))]
        public static bool HatchController_OnExit_Prefix(HatchController __instance, GameObject hitObj) {
            foreach(var traveler in Traveler.GetAllTravelers) {
                if(hitObj.gameObject == traveler.gameObject) {
                    traveler.CompleteExitingShip();
                    return false;
                }
            }
            if(hitObj.CompareTag("PlayerDetector")) {
                foreach (var traveler in Traveler.GetAllTravelers) {
                    if (traveler.IsActivated) {
                        traveler.GoToCenterOfShipToExit();
                    }
                }
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipTractorBeamSwitch), nameof(ShipTractorBeamSwitch.OnTriggerExit))]
        public static bool ShipTractorBeamSwitch_OnTriggerExit_Prefix(ShipTractorBeamSwitch __instance, Collider hitCollider) {
            bool travelerExit = false;
            foreach (var traveler in Traveler.GetAllTravelers) {
                if (hitCollider.gameObject == traveler.gameObject) {
                    //PickUpChert.Log("gabbro exit the tractor beam volume");
                    traveler.CompleteExitingShipBeamVolume();
                    travelerExit = true;
                }
            }
            if(travelerExit) {
                if (!ModifyObjects.Hatchling.IsInsideShipBeamVolume && Traveler.GetAllTravelers.All(x => !x.IsActivated || (!x.IsInsideShipBeamVolume && !x.IsInShip))) {
                    __instance.ActivateTractorBeam();
                }
                return false;
            }

            if(!__instance._isPlayerInShip && __instance._functional && hitCollider.CompareTag("PlayerDetector")) {
                if(Traveler.GetAllTravelers.All(x => !x.IsActivated || (!x.IsInsideShipBeamVolume && !x.IsInShip))) {
                //if(!ModifyObjects.Gabbro.IsActivated || !ModifyObjects.Gabbro.IsInsideShipBeamVolume) {
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
            foreach (var traveler in Traveler.GetAllTravelers) {
                if (traveler.IsActivated) {
                    traveler.StartSitting();
                    traveler.LookAt(__instance.transform, Vector3.zero);
                }
            }
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Campfire), nameof(Campfire.StopRoasting))]
        //public static void Campfire_StopRoasting_Postfix() {
        //    if(ModifyObjects.Gabbro.IsActivated) {
        //        ModifyObjects.Gabbro.StopSitting();
        //    }
        //}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSpawner), nameof(PlayerSpawner.FixedUpdate))]
        public static void PlayerSpawner_FixedUpdate_Prefix(PlayerSpawner __instance) {
            if(__instance._debugWarpNextUpdate) {
                foreach (var traveler in Traveler.GetAllTravelers) {
                    if (traveler.IsActivated) {
                        var owRigidbody = traveler.GetComponent<OWRigidbody>();
                        var offset = owRigidbody.GetPosition() - __instance._playerBody.GetPosition();
                        owRigidbody.WarpToPositionRotation(__instance._debugWarpPoint.transform.position, __instance._debugWarpPoint.transform.rotation);
                        owRigidbody.SetVelocity(__instance._debugWarpPoint.GetPointVelocity());
                        if (!PlayerState.IsInsideShip()) {
                            __instance._debugWarpPoint.AddObjectToTriggerVolumes(owRigidbody.gameObject);
                        }
                    }
                }
            }
        }
    }
}
