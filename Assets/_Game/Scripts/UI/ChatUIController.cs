using System;
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
        
        protected Camera MainCamera;


        private void Awake()
        {
            MainCamera = Camera.main;
        }

        protected void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<AddChatMessage>(MessageChannels.UI, AddChatMessageHandler);
        }
        protected void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<AddChatMessage>(MessageChannels.UI, AddChatMessageHandler);
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
                if (StreamerViewSettingsRoot.activeSelf)
                {
                    GameViewSettingsRoot.SetActive(true);
                    StreamerViewSettingsRoot.SetActive(false);
                }
                StreamerViewUI.SetActive(false);
                GameViewUI.SetActive(true);
                MainCamera.targetTexture = null;

            }
            else
            {
                if (GameViewSettingsRoot.activeSelf)
                {
                    StreamerViewSettingsRoot.SetActive(true);
                    GameViewSettingsRoot.SetActive(false);
                }
                StreamerViewUI.SetActive(true);
                GameViewUI.SetActive(false);
                MainCamera.targetTexture = StreamerViewRenderTexture;
            }
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

            if (GameViewUI.activeSelf)
            {
                DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
                displayMessage.Message = $"{sender}: {message}";
                displayMessage.DelayBeforeDisplay = 0.25f;
                EventManager.Broadcast(displayMessage);
            }
        }
    }
}