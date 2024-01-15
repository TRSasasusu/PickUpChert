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
            }
        }
    }
}
