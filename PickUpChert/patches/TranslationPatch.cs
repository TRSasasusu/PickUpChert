using HarmonyLib;
using Newtonsoft.Json;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickUpChert {
    [HarmonyPatch]
    public static class TranslationPatch {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.SetLanguage))]
        public static void TextTranslation_SetLanguage_Postfix(TextTranslation.Language lang, TextTranslation __instance) {
            var path = PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + $"assets/translations/{lang.GetName().ToLower()}.json";
            PickUpChert.Log($"path to json file: {path}");
            if (!File.Exists(path)) {
                PickUpChert.Log($"this json file: {path} is not found, so English is used.");
                path = PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + $"assets/translations/english.json";
                if (!File.Exists(path)) {
                    PickUpChert.Log($"Even English translation file is not found!", OWML.Common.MessageType.Error);
                    return;
                }
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(path));

            foreach(var (key, value) in dict) {
                __instance.m_table.theTable[key] = value;
            }
        }
    }
}
