using HarmonyLib;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PickUpChert {
    public class PickUpChert : ModBehaviour {
        public static PickUpChert Instance;
        public INewHorizons NewHorizons;
        public static class NHAssembly {
            public static MethodInfo _streamingHandlerSetUpStreaming;
            public static MethodInfo _detailBuilderFixComponent;
            public static Type _dreamLanternControllerFixer;
            public static Type _addPhysics;
        }

        public RuntimeAnimatorController _riebeckAnimatorController;

        public static void Log(string text, MessageType messageType = MessageType.Message) {
            Instance.ModHelper.Console.WriteLine(text, messageType);
        }

        public static string ReadAndRemoveByteOrderMarkFromPath(string path) {
            // this code is from https://github.com/xen-42/outer-wilds-localization-utility/blob/6cf4eb784c06237820d318b4ce22ac30da4acac1/LocalizationUtility/Patches/TextTranslationPatches.cs#L198-L209
            byte[] bytes = File.ReadAllBytes(path);
            byte[] preamble1 = Encoding.UTF8.GetPreamble();
            byte[] preamble2 = Encoding.Unicode.GetPreamble();
            byte[] preamble3 = Encoding.BigEndianUnicode.GetPreamble();
            if (bytes.StartsWith(preamble1))
                return Encoding.UTF8.GetString(bytes, preamble1.Length, bytes.Length - preamble1.Length);
            if (bytes.StartsWith(preamble2))
                return Encoding.Unicode.GetString(bytes, preamble2.Length, bytes.Length - preamble2.Length);
            return bytes.StartsWith(preamble3) ? Encoding.BigEndianUnicode.GetString(bytes, preamble3.Length, bytes.Length - preamble3.Length) : Encoding.UTF8.GetString(bytes);
        }

        private void Awake() {
            Instance = this;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void Start() {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"{nameof(PickUpChert)} is loaded!", MessageType.Success);

            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);


            //var newHorizons = ModHelper.Interaction.TryGetMod("xen.NewHorizons");
            //var type = newHorizons.GetType();
            //Log($"{type}");
            //Log($"{type.Assembly}");
            //foreach(var t in type.Assembly.GetTypes()) {
            //    Log(t.FullName);
            //}
            //Log("hogehoge");
            //foreach(var t in type.Assembly.GetTypes().First(x => x.FullName.Contains("DetailBuilder")).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)) {
            //    Log(t.Name);
            //}
            //NHAssembly._streamingHandlerSetUpStreaming = type.Assembly.GetTypes().First(x => x.FullName.Contains("StreamingHandler")).GetMethods().FirstOrDefault(x => x.Name == "SetUpStreaming" && x.GetParameters()[0].ParameterType == typeof(GameObject));
            //NHAssembly._detailBuilderFixComponent = type.Assembly.GetTypes().First(x => x.FullName.Contains("DetailBuilder")).GetMethod("FixComponent", BindingFlags.NonPublic | BindingFlags.Static);
            //NHAssembly._dreamLanternControllerFixer = type.Assembly.GetTypes().First(x => x.FullName.Contains("DreamLanternControllerFixer"));
            //NHAssembly._addPhysics = type.Assembly.GetTypes().First(x => x.FullName.Contains("AddPhysics"));
            //Log(NHAssembly._streamingHandlerSetUpStreaming.Name);
            //Log(NHAssembly._detailBuilderFixComponent.Name);
            //Log(NHAssembly._dreamLanternControllerFixer.Name);
            //Log(NHAssembly._addPhysics.Name);
            //Log($"{type.Namespace}");
            //var streaminghandlertype = Type.GetType("NewHorizons.Handlers.StreamingHandler, NewHorizons.dll");
            //Log($"{streaminghandlertype}");
            ////foreach(var assembly in Assembly.GetExecutingAssembly().GetTypes()) {
            ////    Log(assembly.FullName);
            ////}

            //var bundle = ModHelper.Assets.LoadBundle("assets/assetbundles/pickupchert");

            BringChert bringChert = null;
            ModifyObjects modifyObjects = null;
            TitleController titleController = null;
            titleController = new TitleController();
            titleController.OnSceneLoad();
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if(loadScene == OWScene.TitleScreen) {
                    if(titleController != null) {
                        titleController.DestroyResources();
                    }
                    titleController = new TitleController();
                    titleController.OnSceneLoad();
                }
                if (loadScene != OWScene.SolarSystem) return;
                ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);

                if(bringChert != null) {
                    bringChert.DestroyResources();
                }
                bringChert = new BringChert();
                bringChert.Initialize();

                if(modifyObjects != null) {
                    modifyObjects.DestroyResources();
                }
                modifyObjects = new ModifyObjects();
                modifyObjects.Initialize();


                //_riebeckAnimatorController = bundle.LoadAsset<RuntimeAnimatorController>("Assets/MyAssets/Animators/riebeck.controller");
                //Log($"{_riebeckAnimatorController}");
            };

            ChertPickUpConversation.Initialize();
        }
    }
}