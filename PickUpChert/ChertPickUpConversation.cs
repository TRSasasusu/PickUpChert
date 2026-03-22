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

        enum InitialPickUpState {
            BEFORE_PICKUP,
            AFTER_PICKUP,
            END_INITIAL_PICKUP,
        }
        InitialPickUpState _initialPickUpState = InitialPickUpState.BEFORE_PICKUP;

        static TextAsset _initialPickUpXML;
        static Dictionary<string, TextAsset> _sectorXMLDict;
        static Dictionary<string, TextAsset> _triggerXMLDict;

        Sector _currentSector;
        TextAsset _triggerXML;

        public static void Initialize() {
            {
                var path = PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + $"assets/dialogues/initial_pick_up.xml";
                _initialPickUpXML = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(path));
            }

            _sectorXMLDict = new Dictionary<string, TextAsset>();
            foreach(var dialogueFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/dialogues/outerwilds/sector", "*.xml", System.IO.SearchOption.AllDirectories)) {
                PickUpChert.Log(dialogueFilePath);
                var sectorName = "Sector_" + System.IO.Path.GetFileNameWithoutExtension(dialogueFilePath);
                _sectorXMLDict[sectorName] = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueFilePath));
            }

            foreach(var dialogueSettingFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/dialogues/outerwilds/sector", "*.json", System.IO.SearchOption.AllDirectories)) {
                PickUpChert.Log(dialogueSettingFilePath);
                var sectorName = "Sector_" + System.IO.Path.GetFileNameWithoutExtension(dialogueSettingFilePath);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueSettingFilePath));
                if(dict.ContainsKey("common")) {
                    foreach(var targetSectorBodyName in dict["common"]) {
                        _sectorXMLDict.Add("Sector_" + targetSectorBodyName, _sectorXMLDict[sectorName]);
                    }
                }
            }

            _triggerXMLDict = new Dictionary<string, TextAsset>();
            foreach(var dialogueFilePath in System.IO.Directory.EnumerateFiles(PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + "assets/dialogues/outerwilds/trigger", "*.xml", System.IO.SearchOption.AllDirectories)) {
                PickUpChert.Log(dialogueFilePath);
                var dirName = new DirectoryInfo(dialogueFilePath).Parent.Name;
                var fileName = System.IO.Path.GetFileNameWithoutExtension(dialogueFilePath);
                var triggerName = $"PUCTriggers{dirName}/{fileName}";
                _triggerXMLDict[triggerName] = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(dialogueFilePath));
            }
        }

        public bool OnStartConversation() {
            if(_initialPickUpState == InitialPickUpState.AFTER_PICKUP) {
                BringChert.Instance.ChertDialogueTree.SetTextXml(_initialPickUpXML);
                _initialPickUpState = InitialPickUpState.END_INITIAL_PICKUP;
                return false;
            }

            if(_triggerXML != null) {
                BringChert.Instance.ChertDialogueTree.SetTextXml(_triggerXML);
                return false;
            }

            if(!_currentSector) {
                return true;
            }

            var sector = _currentSector;
            for(int i = 0; i < 1000; ++i) {
                foreach(var (sectorName, xml) in _sectorXMLDict) {
                    if(sector.name == sectorName) {
                        BringChert.Instance.ChertDialogueTree.SetTextXml(xml);
                        return false;
                    }
                }
                sector = sector._parentSector;
                if(!sector) {
                    break;
                }
            }
            return true;
        }

        void Awake() {
            Instance = this;
        }

        IEnumerator Start() {
            yield return null;
            PickUpChert.Log(GameObject.Find("PUCTriggersTH/Campsite").name);
            foreach (var (triggerName, xml) in _triggerXMLDict) {
                while (true) {
                    var trigger = GameObject.Find(triggerName);
                    if (trigger) {
                        PickUpChert.Log("found trigger: " + triggerName);
                        trigger.OnTriggerEnterAsObservable().Subscribe(other => {
                            PickUpChert.Log("entered trigger: " + triggerName);
                            if (other.gameObject == Locator._playerBody.gameObject) {
                                PickUpChert.Log("player entered trigger: " + triggerName);
                                _triggerXML = xml;
                            }
                        }).AddTo(trigger);
                        trigger.OnTriggerExitAsObservable().Subscribe(other => {
                            PickUpChert.Log("exited trigger: " + triggerName);
                            if (other.gameObject == Locator._playerBody.gameObject) {
                                PickUpChert.Log("player exited trigger: " + triggerName);
                                if(_triggerXML == xml) {
                                    _triggerXML = null;
                                }
                            }
                        }).AddTo(trigger);
                        break;
                    }
                    yield return null;
                }
            }
        }

        void Update() {
            if(BringChert.Instance == null || !BringChert.Instance.SectorDetector) {
                return;
            }
            if(_initialPickUpState == InitialPickUpState.BEFORE_PICKUP) {
                if(ChertItem.Brought) {
                    _initialPickUpState = InitialPickUpState.AFTER_PICKUP;
                }
            }

            _currentSector = BringChert.Instance.SectorDetector.GetLastEnteredSector();
        }
    }
}
