using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class OtherConversation {
        public TextAsset ConversationStart(string characterName, IEnumerable<ConversationTrigger> triggers, IEnumerable<Sector> sectors) {
            if(characterName == "Slate") {
                if(!ChertItem.Brought) {
                    return null;
                }
                return ChertPickUpConversation._conversationDataDict["Slate"]._eventXMLDict["Slate"];
            }
            return null;
        }

        public void ConversationEnd(string characterName, IEnumerable<ConversationTrigger> triggers, IEnumerable<Sector> sectors) {

        }
    }
}
