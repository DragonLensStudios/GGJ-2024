using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using GEmojiSharp;
using Messaging;
using Messaging.Messages;
using TMPro;
using UnityEngine;

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