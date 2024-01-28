using System;
using Enums;
using Messaging;
using Messaging.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    
    public class ChatUIController : MonoBehaviour
    {
        [field:SerializeField] public GameObject ChatMessagePrefab { get; set; }
        [field:SerializeField] public GameObject ChatMessageContainer { get; set; }
        [field:SerializeField] public Scrollbar ChatScrollbar { get; set; }

        protected void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<AddChatMessage>(MessageChannels.UI, AddChatMessageHandler);
        }
        protected void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<AddChatMessage>(MessageChannels.UI, AddChatMessageHandler);
        }
        
        public virtual void AddChatMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AddChatMessage>().HasValue) return;
            var data = message.Message<AddChatMessage>().GetValueOrDefault();
            AddChatMessage(data.Sender, data.Message);
        }
        
        public void AddChatMessage(string sender, string message)
        {
            var chatMessage = Instantiate(ChatMessagePrefab, ChatMessageContainer.transform);
            var chatText = chatMessage.GetComponent<TMP_Text>();
            chatText.text = $"{sender}: {message}";
            ChatScrollbar.value = 0;
        }
    }
}