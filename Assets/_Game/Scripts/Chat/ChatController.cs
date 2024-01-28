using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace DLS.Chat
{
    public class ChatController : MonoBehaviour
    {
        [field: SerializeField] public List<ChatMessage> ChatMessages { get; set; } = new();

        private void Start()
        {
            var chatMessage = new ChatMessage("Todd", "I Like Eggs");
            ChatMessages.Add(chatMessage);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AddChatMessage(chatMessage.Sender, chatMessage.Message));
        }
    
    }
}