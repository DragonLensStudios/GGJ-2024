using System;
using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using FPS.Scripts.Game;
using UnityEngine;

namespace DLS.Chat
{
    public class ChatController : MonoBehaviour
    {
        [field: SerializeField] public List<ChatMessage> ChatMessages { get; set; } = new();
        
        [field: SerializeField] public GameObject StreamerViewUI { get; set; }
        [field: SerializeField] public GameObject GameViewUI { get; set; }
        
        [field: SerializeField] public Camera MainCamera { get; set; }
        
        [field: SerializeField] public RenderTexture StreamerViewRenderTexture { get; set; }

        private void Awake()
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }
        }

        private void Start()
        {
            var chatMessage = new ChatMessage("Todd", "I Like Eggs");
            ChatMessages.Add(chatMessage);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AddChatMessage(chatMessage.Sender, chatMessage.Message));
        }

        private void Update()
        {
            if (UnityEngine.Input.GetButtonDown(GameConstants.k_ButtonNameToggleView))
            {
                ToggleView();
            }
        }

        private void ToggleView()
        {
            if(StreamerViewUI.activeSelf)
            {
                StreamerViewUI.SetActive(false);
                GameViewUI.SetActive(true);
                MainCamera.targetTexture = null;
            }
            else
            {
                StreamerViewUI.SetActive(true);
                GameViewUI.SetActive(false);
                MainCamera.targetTexture = StreamerViewRenderTexture;
            }
        }
    }
}