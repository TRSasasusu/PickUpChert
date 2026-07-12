using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace PickUpChert {
    public class MovingConversation : MonoBehaviour {
        public static MovingConversation Instance { get; private set; }

        Dictionary<string, bool> _doneConversations = new Dictionary<string, bool>();

        DialogueBoxVer2 _dialogue;
        Coroutine _displaying;
        bool _displayPaused;

        void Start() {
            Instance = this;
            _dialogue = GetComponent<DialogueBoxVer2>();
        }

        public void DisplayDialogue(string filename, List<Dictionary<string, object>> items = null) {
            if(_doneConversations.ContainsKey(filename)) {
                return;
            }
            _doneConversations[filename] = true;

            if(items == null) {
                items = ChertPickUpConversation.Instance.GetMovingConversationItem(filename);
            }

            _displaying = StartCoroutine(DisplayDialogueSeqentially(filename, items));
        }

        public void PauseDiaplaying() {
            _displayPaused = true;
            _dialogue._turnOnNameField = false;

            //_dialogue.ResetAllText();
            _dialogue._buttonPromptElement.gameObject.SetActive(true);
        }

        public void ResumeDiaplaying() {
            _displayPaused = false;
        }

        IEnumerator DisplayDialogueSeqentially(string filename, List<Dictionary<string, object>> items) {
            foreach(var item in items) {
                var name = (string)item["name"];
                var text = (string)item["text"];
                float time;
                switch(item["time"]) {
                    case long obj:
                        time = obj;
                        break;
                    case double obj:
                        time = (float)obj;
                        break;
                    case int obj:
                        time = obj;
                        break;
                    case float obj:
                        time = obj;
                        break;
                    default:
                        time = 3;
                        PickUpChert.Log("Cannot cast the time!!");
                        break;
                }

                UpdateText($"PickUpChert_{filename}{text}", name);
                yield return new WaitForSeconds(time);
                yield return new WaitUntil(() => !_displayPaused);
                yield return null;
            }
            EndText();
        }

        void UpdateText(string text, string name) {
            _dialogue._potentialOptions = null;
            _dialogue.ResetAllText();
            //_dialogue.SetVisible(true);
            if(name != null) {
                _dialogue._turnOnNameField = true;
                _dialogue.SetNameField(TextTranslation.Translate(name));
                _dialogue.SetNameFieldVisible(true);
            }
            else {
                _dialogue._turnOnNameField = false;
                _dialogue.SetNameFieldVisible(false);
            }
            _dialogue.SetMainFieldDialogueText(TextTranslation.Translate(text));
            _dialogue._buttonPromptElement.gameObject.SetActive(false);
            if(_dialogue._mainFieldTextEffect != null) {
                _dialogue._mainFieldTextEffect.StartTextEffect();
            }

            BringChert.Instance.ChertTraveler.OnStartConversation();
            if(ModifyObjects.Gabbro.IsActivated) {
                ModifyObjects.Gabbro._travelerController.OnStartConversation();
                //PickUpChert.Locomotion.GabbroStopPlaying();
            }
        }

        void EndText() {
            _dialogue.SetVisible(false);
            _dialogue._turnOnNameField = false;
            _dialogue._buttonPromptElement.gameObject.SetActive(true);

            BringChert.Instance.ChertTraveler.OnEndConversation();
            if(ModifyObjects.Gabbro.IsActivated) {
                ModifyObjects.Gabbro._travelerController.OnEndConversation();
                //PickUpChert.Locomotion.GabbroStartPlaying();
            }
        }
    }
}
