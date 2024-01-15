using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine;

namespace PickUpChert {
    public class PickUpChert : ModBehaviour {
        public static PickUpChert Instance;

        public static void Log(string text, MessageType messageType = MessageType.Message) {
            Instance.ModHelper.Console.WriteLine(text, messageType);
        }

        private void Awake() {
            Instance = this;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void Start() {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"{nameof(PickUpChert)} is loaded!", MessageType.Success);


            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
                var bringChert = new BringChert();
                bringChert.Initialize();
            };
        }
    }
}