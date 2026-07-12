using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public class ConversationTrigger : MonoBehaviour {
        [SerializeField] public string _conversationFileName;
        [SerializeField] public string _movingConversationFileName;

        void OnTriggerEnter(Collider other) {
            ChertPickUpConversation.Instance.EnterTrigger(this);
        }

        void OnTriggerExit(Collider other) {
            ChertPickUpConversation.Instance.ExitTrigger(this);
        }
    }
}
