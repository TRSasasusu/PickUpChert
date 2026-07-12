using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using NewHorizons.Utility;
using System.IO;
using IEnumerator = System.Collections.IEnumerator;

namespace PickUpChert {
    public class ChertPickUpConversation : MonoBehaviour {
        public static ChertPickUpConversation Instance;

        public class ConversationData {
            public Dictionary<string, TextAsset> _eventXMLDict = new Dictionary<string, TextAsset>();
            public Dictionary<string, TextAsset> _sectorXMLDict = new Dictionary<string, TextAsset>();
            public Dictionary<string, TextAsset> _triggerXMLDict = new Dictionary<string, TextAsset>();
        }
        public static Dictionary<string, ConversationData> _conversationDataDict;

        static Dictionary<string, List<Dictionary<string, object>>> _movingConversationJsonDict;

        HashSet<ConversationTrigger> _conversationTriggers = new HashSet<ConversationTrigger>();

        public static void Initialize() {
            _conversationDataDict = new Dictionary<string, ConversationData>();

            foreach(var dialogueFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/dialogues/outerwilds", "*.xml", System.IO.SearchOption.AllDirectories)) {
                PickUpChert.Log(dialogueFilePath);
                var splitPath = dialogueFilePath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(dialogueFilePath);

                var idxOfOuterWilds = splitPath.FindIndex(s => s == "outerwilds");
                var characterName = splitPath[idxOfOuterWilds + 1];
                if(!_conversationDataDict.ContainsKey(characterName)) {
                    _conversationDataDict[characterName] = new ConversationData();
                }
                var categoryName = splitPath[idxOfOuterWilds + 2];

                var textAsset = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueFilePath));
                var splitPathAfterCategory = splitPath.Skip(idxOfOuterWilds + 1 + 2);
                var splitPathAfterCategoryWithoutExtension = splitPathAfterCategory.Take(splitPathAfterCategory.Count() - 1).Append(nameWithoutExtension);
                var key = $"{string.Join("/", splitPathAfterCategoryWithoutExtension)}";
                switch (categoryName) {
                    case "event":
                        _conversationDataDict[characterName]._eventXMLDict[key] = textAsset;
                        break;
                    case "sector":
                        _conversationDataDict[characterName]._sectorXMLDict[key] = textAsset;
                        break;
                    case "trigger":
                        _conversationDataDict[characterName]._triggerXMLDict[key] = textAsset;
                        break;
                }
            }

            //{
            //    var path = PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + $"assets/dialogues/initial_pick_up.xml";
            //    _initialPickUpXML = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(path));
            //}

            //_sectorXMLDict = new Dictionary<string, TextAsset>();
            //foreach(var dialogueFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/dialogues/outerwilds/sector", "*.xml", System.IO.SearchOption.AllDirectories)) {
            //    PickUpChert.Log(dialogueFilePath);
            //    var sectorName = "Sector_" + System.IO.Path.GetFileNameWithoutExtension(dialogueFilePath);
            //    _sectorXMLDict[sectorName] = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueFilePath));
            //}

            ////foreach(var dialogueSettingFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/dialogues/outerwilds/sector", "*.json", System.IO.SearchOption.AllDirectories)) {
            ////    PickUpChert.Log(dialogueSettingFilePath);
            ////    var sectorName = "Sector_" + System.IO.Path.GetFileNameWithoutExtension(dialogueSettingFilePath);
            ////    var dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueSettingFilePath));
            ////    if(dict.ContainsKey("common")) {
            ////        foreach(var targetSectorBodyName in dict["common"]) {
            ////            _sectorXMLDict.Add("Sector_" + targetSectorBodyName, _sectorXMLDict[sectorName]);
            ////        }
            ////    }
            ////}

            //_triggerXMLDict = new Dictionary<string, TextAsset>();
            //foreach(var dialogueFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/dialogues/outerwilds/trigger", "*.xml", System.IO.SearchOption.AllDirectories)) {
            //    PickUpChert.Log(dialogueFilePath);
            //    var dirName = new DirectoryInfo(dialogueFilePath).Parent.Name;
            //    var fileName = System.IO.Path.GetFileNameWithoutExtension(dialogueFilePath);
            //    var triggerName = $"PUCTriggers{dirName}/{fileName}";
            //    _triggerXMLDict[triggerName] = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueFilePath));
            //}

            //_conversationXMLDict = new Dictionary<string, TextAsset>();
            //foreach(var dialogueFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/dialogues/outerwilds/conversation", "*.xml", System.IO.SearchOption.AllDirectories)) {
            //    PickUpChert.Log(dialogueFilePath);
            //    var conversationName = System.IO.Path.GetFileNameWithoutExtension(dialogueFilePath);
            //    _conversationXMLDict[conversationName] = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueFilePath));
            //}

            _movingConversationJsonDict = new Dictionary<string, List<Dictionary<string, object>>>();
            foreach(var dialogueFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/moving_conversations/outerwilds", "*.json", System.IO.SearchOption.AllDirectories)) {
                PickUpChert.Log(dialogueFilePath);
                var dirName = new DirectoryInfo(dialogueFilePath).Parent.Name;
                var fileName = System.IO.Path.GetFileNameWithoutExtension(dialogueFilePath);
                var name = $"{dirName}/{fileName}";
                var dict = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueFilePath));
                _movingConversationJsonDict[name] = dict["texts"];
            }
        }

        public List<Dictionary<string, object>> GetMovingConversationItem(string filename) {
            return _movingConversationJsonDict[filename];
        }

        public bool OnChertStartConversation() {
            var textAsset = ConversationStartCharacter("Chert");
            if (textAsset != null) {
                BringChert.Instance.ChertDialogueTree.SetTextXml(textAsset);
                return false;
            }
            return true;
        }

        TextAsset _backupXml;
        bool _chertSpeaking;
        string _speakingCharacterName;
        public void StartConversationPrefix(CharacterDialogueTree dialogueTree) {
            MovingConversation.Instance.PauseDiaplaying();

            _chertSpeaking = false;

            var textAsset = ConversationStartCharacter(dialogueTree._characterName);
            if (textAsset == null) {
                return;
            }

            BringChert.Instance.ChertTraveler.OnStartConversation();

            _backupXml = dialogueTree._xmlCharacterDialogueAsset;
            _speakingCharacterName = dialogueTree._characterName;
            ConversationStartCharacter(_speakingCharacterName);
            dialogueTree.SetTextXml(textAsset);
        }
        public void InputDialogueOptionPostfix(CharacterDialogueTree __instance, ref bool __result) {
            PickUpChert.Log($"InputDialoueOption __result: {__result}");
            if(!__result) {
                if(ChertItem.Brought) {
                    BringChert.Instance.ChertTraveler.OnEndConversation();
                }

                if(_backupXml != null) {
                    __instance.SetTextXml(_backupXml);
                    _backupXml = null;

                    ConversationEndCharacter(_speakingCharacterName);
                    _speakingCharacterName = null;
                }

                MovingConversation.Instance.ResumeDiaplaying();
            }
        }
        public void DisplayDialogueBox2Postfix(CharacterDialogueTree dialogueTree, ref DialogueBoxVer2 __result) {
            if (_chertSpeaking) {
                __result.SetNameField(TextTranslation.Translate("Chert"));
            }
        }
        public void SetMainFieldDialogueTextPrefix(DialogueBoxVer2 __instance, ref string richText) {
            if(_backupXml == null) {
                return;
            }

            //PickUpChert.Log(__result._mainFieldTextEffect._strToDisplay);
            PickUpChert.Log(richText);
            //if(__result._mainFieldTextEffect._strToDisplay.StartsWith("<Chert/>")) {

            if(TextTranslation.s_theTable.GetLanguage() == TextTranslation.Language.JAPANESE) {
                richText += "<i></i>"; // magic to use dynamic font in japanese, which has many characters
            }

            if(richText.StartsWith("<Chert/>")) {
                //__result._mainFieldTextEffect._strToDisplay = __result._mainFieldTextEffect._strToDisplay.Substring("<Chert/>".Length);
                richText = richText.Substring("<Chert/>".Length);
                Observable.NextFrame().Subscribe(_ => {
                    Locator.GetToolModeSwapper().EquipToolMode(ToolMode.Item);
                }).AddTo(this);
                _chertSpeaking = true;
            }
            else {
                Locator.GetToolModeSwapper().UnequipTool();
                _chertSpeaking = false;
            }

            __instance._turnOnNameField = false;
            if (_backupXml != null) {
                __instance._turnOnNameField = true;
                __instance.SetNameFieldVisible(true);
            }
        }

        public void EnterTrigger(ConversationTrigger trigger) {
            _conversationTriggers.Add(trigger);
        }

        public void ExitTrigger(ConversationTrigger trigger) {
            _conversationTriggers.Remove(trigger);
        }

        void Awake() {
            Instance = this;
        }

        //IEnumerator Start() {
        //    yield return null;
        //    PickUpChert.Log(GameObject.Find("PUCTriggersTH/Campsite").name);
        //    foreach (var (triggerName, xml) in _triggerXMLDict) {
        //        while (true) {
        //            var trigger = GameObject.Find(triggerName);
        //            if (trigger) {
        //                PickUpChert.Log("found trigger: " + triggerName);
        //                trigger.OnTriggerEnterAsObservable().Subscribe(other => {
        //                    PickUpChert.Log("entered trigger: " + triggerName);
        //                    if (other.gameObject == Locator._playerBody.gameObject) {
        //                        PickUpChert.Log("player entered trigger: " + triggerName);
        //                        _triggerXML = xml;
        //                    }
        //                }).AddTo(trigger);
        //                trigger.OnTriggerExitAsObservable().Subscribe(other => {
        //                    PickUpChert.Log("exited trigger: " + triggerName);
        //                    if (other.gameObject == Locator._playerBody.gameObject) {
        //                        PickUpChert.Log("player exited trigger: " + triggerName);
        //                        if(_triggerXML == xml) {
        //                            _triggerXML = null;
        //                        }
        //                    }
        //                }).AddTo(trigger);
        //                break;
        //            }
        //            yield return null;
        //        }
        //    }
        //}

        //void Update() {
        //    if(BringChert.Instance == null || !BringChert.Instance.SectorDetector) {
        //        return;
        //    }
        //    if(_initialPickUpState == InitialPickUpState.BEFORE_PICKUP) {
        //        if(ChertItem.Brought) {
        //            _initialPickUpState = InitialPickUpState.AFTER_PICKUP;
        //        }
        //    }

        //    _currentSector = BringChert.Instance.SectorDetector.GetLastEnteredSector();
        //}

        TextAsset ConversationStartCharacter(string characterName) {
            if (characterName == "Chert") {
                return ChertForConversation.Instance.ConversationStart(_conversationTriggers, BringChert.Instance.SectorDetector._sectorList);
            }
            else if (characterName == "Gabbro") {
                return ModifyObjects.Gabbro.ConversationStart(_conversationTriggers, BringChert.Instance.SectorDetector._sectorList);
            }
            return ModifyObjects.OtherConversation.ConversationStart(characterName, _conversationTriggers, BringChert.Instance.SectorDetector._sectorList);
        }

        void ConversationEndCharacter(string characterName) {
            if (characterName == "Chert") {
                ChertForConversation.Instance.ConversationEnd(_conversationTriggers, BringChert.Instance.SectorDetector._sectorList);
            }
            else if(characterName == "Gabbro") {
                ModifyObjects.Gabbro.ConversationEnd(_conversationTriggers, BringChert.Instance.SectorDetector._sectorList);
            }
            else {
                ModifyObjects.OtherConversation.ConversationEnd(characterName, _conversationTriggers, BringChert.Instance.SectorDetector._sectorList);
            }
        }
    }
}
