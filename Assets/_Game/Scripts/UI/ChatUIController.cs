using System;
using DLS.Chat;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DLS.UI
{
    
    public class ChatUIController : MonoBehaviour
    {
        [field: SerializeField] public GameObject StreamerViewUI { get; set; }
        [field: SerializeField] public GameObject StreamerViewSettingsRoot { get; set; }
        [field: SerializeField] public GameObject GameViewUI { get; set; }
        [field: SerializeField] public GameObject GameViewSettingsRoot { get; set; }
        [field: SerializeField] public RenderTexture StreamerViewRenderTexture { get; set; }
        [field:SerializeField] public GameObject ChatMessagePrefab { get; set; }
        [field:SerializeField] public GameObject ChatMessageContainer { get; set; }
        [field:SerializeField] public Scrollbar ChatScrollbar { get; set; }
        
        [field: SerializeField] public TMP_Text ViewerCountText { get; set; }
        
        [field: SerializeField] public TMP_Text SubscriberCountText { get; set; }
        
        protected int CurrentViewers { get; set; }
        protected int CurrentSubscribers { get; set; }
        
        protected Camera MainCamera;


        private void Awake()
        {
            MainCamera = Camera.main;
        }

        protected void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<AddChatMessage>(MessageChannels.UI, AddChatMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<AddUserMessage>(MessageChannels.UI, AddUserMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<AddSubscriberMessage>(MessageChannels.UI, AddSubscriberMessageHandler);
        }
        protected void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<AddChatMessage>(MessageChannels.UI, AddChatMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<AddUserMessage>(MessageChannels.UI, AddUserMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<AddSubscriberMessage>(MessageChannels.UI, AddSubscriberMessageHandler);
        }

        private void Update()
        {
            //TODO: replace with new input system
            if (UnityEngine.Input.GetButtonDown(GameConstants.k_ButtonNameToggleView))
            {
                ToggleView();
            }
        }
        
        private void ToggleView()
        {
            if(StreamerViewUI.activeSelf)
            {
                GameViewSettingsRoot.SetActive(StreamerViewSettingsRoot.activeSelf);
                StreamerViewUI.SetActive(false);
                GameViewUI.SetActive(true);
                MainCamera.targetTexture = null;
            }
            else
            {
                StreamerViewSettingsRoot.SetActive(GameViewSettingsRoot.activeSelf);
                StreamerViewUI.SetActive(true);
                GameViewUI.SetActive(false);
                MainCamera.targetTexture = StreamerViewRenderTexture;
            }
        }
        
        public virtual void AddUserMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AddUserMessage>().HasValue) return;
            var data = message.Message<AddUserMessage>().GetValueOrDefault();
            AddUserMessage(data.User);
        }
        
        public void AddUserMessage(ViewerUser user)
        {
            //TODO:Refactor to not use hardcoded string for guest.
            if (user.UserType == UserType.Subscriber && !user.Username.Equals("Guest"))
            {
                var chatMessage = Instantiate(ChatMessagePrefab, ChatMessageContainer.transform);
                var chatText = chatMessage.GetComponent<TMP_Text>();
                chatText.text = $"{user.Username} has joined the chat!";
            }
            CurrentViewers++;
            ViewerCountText.text = $"{CurrentViewers} Viewers";
            SubscriberCountText.text = $"{CurrentSubscribers} Subscribers";
            ChatScrollbar.value = 0;
            
            if (GameViewUI.activeSelf)
            {
                DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
                //TODO:Refactor to not use hardcoded string for guest.
                if (user.UserType == UserType.Subscriber && !user.Username.Equals("Guest"))
                {
                    displayMessage.Message = $"{user.Username} has joined the chat!";
                    displayMessage.DelayBeforeDisplay = 0.0f;
                    EventManager.Broadcast(displayMessage);
                }
            }
        }

        public virtual void AddChatMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AddChatMessage>().HasValue) return;
            var data = message.Message<AddChatMessage>().GetValueOrDefault();
            AddChatMessage(data.User, data.Message);
        }
        
        public void AddChatMessage(ViewerUser user, string message)
        {
            var chatMessage = Instantiate(ChatMessagePrefab, ChatMessageContainer.transform);
            var chatText = chatMessage.GetComponent<TMP_Text>();
            chatText.text = $"{user.Username}: {message}";
            ChatScrollbar.value = 0;

            if (GameViewUI.activeSelf)
            {
                DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
                displayMessage.Message = $"{user.Username}: {message}";
                displayMessage.DelayBeforeDisplay = 0f;
                EventManager.Broadcast(displayMessage);
            }
        }
        
        private void AddSubscriberMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AddSubscriberMessage>().HasValue) return;
            var data = message.Message<AddSubscriberMessage>().GetValueOrDefault();
            AddSubscriberMessage(data.User);
        }
        
        public void AddSubscriberMessage(ViewerUser user)
        {
            CurrentSubscribers++;
            SubscriberCountText.text = $"{CurrentSubscribers} Subscribers";
            if (GameViewUI.activeSelf)
            {
                if(user.UserType == UserType.Subscriber && !user.Username.Equals("Guest"))
                {
                    DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
                    displayMessage.Message = $"{user.Username} has subscribed!";
                    displayMessage.DelayBeforeDisplay = 0.0f;
                    EventManager.Broadcast(displayMessage);
                }

            }
        }
    }
}