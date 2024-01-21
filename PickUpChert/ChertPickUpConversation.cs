using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class ChertPickUpConversation : MonoBehaviour {
        public static ChertPickUpConversation Instance;

        static readonly Dictionary<string, string> SECTOR_DIALOGUE_DICT = new Dictionary<string, string>() {
            {"Sector_City", "sunless_city"},
        };

        enum InitialPickUpState {
            BEFORE_PICKUP,
            AFTER_PICKUP,
            END_INITIAL_PICKUP,
        }
        InitialPickUpState _initialPickUpState = InitialPickUpState.BEFORE_PICKUP;

        static TextAsset _initialPickUpXML;
        static Dictionary<string, TextAsset> _sectorXMLDict;

        Sector _currentSector;

        public static void Initialize() {
            {
                var path = PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + $"assets/dialogues/initial_pick_up.xml";
                _initialPickUpXML = new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(path));
            }

            _sectorXMLDict = new Dictionary<string, TextAsset>();
            foreach(var (sectorName, dialogueFileName) in SECTOR_DIALOGUE_DICT) {
                var path = PickUpChert.Instance.ModHelper.Manifest.ModFolderPath + $"assets/dialogues/{dialogueFileName}.xml";
                _sectorXMLDict.Add(sectorName, new TextAsset(PickUpChert.ReadAndRemoveByteOrderMarkFromPath(path)));
            }
        }

        public bool OnStartConversation() {
            if(_initialPickUpState == InitialPickUpState.AFTER_PICKUP) {
                BringChert.Instance.ChertDialogueTree.SetTextXml(_initialPickUpXML);
                _initialPickUpState = InitialPickUpState.END_INITIAL_PICKUP;
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

        void Update() {
            if(BringChert.Instance == null || !BringChert.Instance.SectorDetector) {
                return;
            }
            if(_initialPickUpState == InitialPickUpState.BEFORE_PICKUP) {
                if(ChertItem.Instance.Brought) {
                    _initialPickUpState = InitialPickUpState.AFTER_PICKUP;
                }
            }

            _currentSector = BringChert.Instance.SectorDetector.GetLastEnteredSector();
        }
    }
}
