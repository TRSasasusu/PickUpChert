using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class ChertForConversation : MonoBehaviour {
        public static ChertForConversation Instance { get; private set; }

        bool _hasPickedUp = false;
        bool _hasConversationAfterPickedUp = false;

        void Awake() {
            Instance = this;
        }

        void Update() {
            if(ChertItem.Brought) {
                _hasPickedUp = true;
            }
        }

        public TextAsset ConversationStart(IEnumerable<ConversationTrigger> triggers, IEnumerable<Sector> sectors) {
            foreach (var trigger in triggers) {
                if (ChertPickUpConversation._conversationDataDict["Chert"]._triggerXMLDict.ContainsKey(trigger._conversationFileName)) {
                    return ChertPickUpConversation._conversationDataDict["Chert"]._triggerXMLDict[trigger._conversationFileName];
                }
            }
            foreach (var sector in sectors) {
                if(ChertPickUpConversation._conversationDataDict["Chert"]._sectorXMLDict.ContainsKey(sector.name)) {
                    return ChertPickUpConversation._conversationDataDict["Chert"]._sectorXMLDict[sector.name];
                }
            }

            if (_hasPickedUp) {
                return ChertPickUpConversation._conversationDataDict["Chert"]._eventXMLDict["initial_pick_up"];
            }
            return null;
        }

        public void ConversationEnd(IEnumerable<ConversationTrigger> triggers, IEnumerable<Sector> sectors) {
        }
    }
}
