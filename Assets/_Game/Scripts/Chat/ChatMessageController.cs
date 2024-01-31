using System;
using System.Collections;
using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using FPS.Scripts.Game;
using UnityEngine;

namespace DLS.Chat
{
    public class ChatMessageController : MonoBehaviour
    {
        [field: SerializeField] public List<ChatMessage> ChatMessages { get; set; } = new();

        private void Start()
        {
            var chatMessage = new ChatMessage("Todd", "I Like Eggs");
            var chatMessage2 = new ChatMessage("Todd", "I Like I LIKE GREEN EGGS AND HAM ALL DAY LONG");
            var chatMessage3= new ChatMessage("Todd", "I Like I LIKE GREEN EGGS AND HAM ALL DAY LONG");
            var chatMessage4 = new ChatMessage("Todd", "I Like I LIKE GREEN EGGS AND HAM ALL DAY LONG");
            var chatMessage5 = new ChatMessage("Todd", "I Like I LIKE GREEN EGGS AND HAM ALL DAY LONG");
            var chatMessage6 = new ChatMessage("Todd", "I Like I LIKE GREEN EGGS AND HAM ALL DAY LONG");
            var chatMessage7 = new ChatMessage("Todd", "I Like Eggs 7");
            var chatMessage8 = new ChatMessage("Todd", "I Like Eggs 8");
            var chatMessage9 = new ChatMessage("Todd", "I Like Eggs 9");
            var chatMessage10 = new ChatMessage("Todd", "I Like Eggs 10");
            var chatMessage11 = new ChatMessage("Joe", "I Hate Eggs");
            var chatMessage12 = new ChatMessage("Will", "I Hate Eggs also my dude");
            var chatMessage13 = new ChatMessage("Roger", "I Hate Eggs also");
            ChatMessages.Add(chatMessage);
            ChatMessages.Add(chatMessage2);
            ChatMessages.Add(chatMessage3);
            ChatMessages.Add(chatMessage4);
            ChatMessages.Add(chatMessage5);
            ChatMessages.Add(chatMessage6);
            ChatMessages.Add(chatMessage7);
            ChatMessages.Add(chatMessage8);
            ChatMessages.Add(chatMessage9);
            ChatMessages.Add(chatMessage10);
            ChatMessages.Add(chatMessage11);
            ChatMessages.Add(chatMessage12);
            ChatMessages.Add(chatMessage13);
            StartCoroutine(ChatMessageEnumerator());
        }
        
        public IEnumerator ChatMessageEnumerator()
        {
            foreach (var chatMessage in ChatMessages)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AddChatMessage(chatMessage.Sender, chatMessage.Message));
                var randomTimeBetweenMessages = UnityEngine.Random.Range(1, 5);
                yield return new WaitForSeconds(randomTimeBetweenMessages);
            }
        }
       
    }
}