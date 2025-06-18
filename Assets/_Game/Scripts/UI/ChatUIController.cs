using System;
using System.Collections;
using DLS.Chat;
using DLS.Enums;
using DLS.Managers;
using DLS.Messaging;
using DLS.Messaging.Messages;
using Enums;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using Messaging.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DLS.UI
{
    public class ChatUIController : MonoBehaviour
    {
        [field: SerializeField] public RawImage StreamerViewImage { get; set; }
        [field: SerializeField] public GameObject StreamerViewUI { get; set; }
        [field: SerializeField] public GameObject StreamerViewSettingsRoot { get; set; }
        [field: SerializeField] public GameObject GameViewUI { get; set; }
        [field: SerializeField] public GameObject GameViewSettingsRoot { get; set; }
        [field: SerializeField] public RenderTexture StreamerViewRenderTexture { get; set; }
        [field: SerializeField] public GameObject ChatMessagePrefab { get; set; }
        [field: SerializeField] public GameObject ChatMessageContainer { get; set; }
        [field: SerializeField] public Scrollbar ChatScrollbar { get; set; }
        [field: SerializeField] public TMP_Text ViewerCountText { get; set; }
        [field: SerializeField] public TMP_Text SubscriberCountText { get; set; }
        
        // NEW: Separate cameras
        [field: SerializeField] public Camera GameCamera { get; set; }      // Your main camera (child of player)
        [field: SerializeField] public Camera StreamerOverlayCamera { get; set; } // New camera for streamer UI only
        
        protected int CurrentViewers { get; set; }
        protected int CurrentSubscribers { get; set; }
        protected bool isStreamerViewActive = false;

        private void Awake()
        {
            SetupCameras();
            SetupRenderTexture();
        }

        private void Start()
        {
            // Default to Game View
            SwitchToGameView();
        }

        private void SetupCameras()
        {
            // Find or assign the main game camera (child of player)
            if (GameCamera == null)
            {
                GameCamera = Camera.main;
            }

            // Create streamer overlay camera if it doesn't exist
            if (StreamerOverlayCamera == null)
            {
                GameObject streamerCamObj = new GameObject("StreamerOverlayCamera");
                StreamerOverlayCamera = streamerCamObj.AddComponent<Camera>();
            }

            // Setup streamer overlay camera
            StreamerOverlayCamera.clearFlags = CameraClearFlags.SolidColor;
            StreamerOverlayCamera.backgroundColor = Color.black;
            StreamerOverlayCamera.cullingMask = LayerMask.GetMask("UI"); // Only render UI layer
            StreamerOverlayCamera.depth = 1; // Higher than game camera
            StreamerOverlayCamera.enabled = false; // Start disabled
        }

        private void SetupRenderTexture()
        {
            if (StreamerViewRenderTexture != null)
            {
                if (!StreamerViewRenderTexture.IsCreated())
                {
                    StreamerViewRenderTexture.Create();
                }

                // Always assign the texture to the RawImage
                if (StreamerViewImage != null)
                {
                    StreamerViewImage.texture = StreamerViewRenderTexture;
                }
            }
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
            if (UnityEngine.Input.GetButtonDown(GameConstants.k_ButtonNameToggleView))
            {
                ToggleView();
            }
        }

        private void ToggleView()
        {
            isStreamerViewActive = !isStreamerViewActive;
            
            if (isStreamerViewActive)
            {
                SwitchToStreamerView();
            }
            else
            {
                SwitchToGameView();
            }
        }

        private void SwitchToStreamerView()
        {
            // Game camera renders to texture (continues following player)
            GameCamera.targetTexture = StreamerViewRenderTexture;
            
            // Enable streamer overlay camera to render UI
            StreamerOverlayCamera.enabled = true;
            
            // Switch UI
            GameViewUI.SetActive(false);
            StreamerViewUI.SetActive(true);
            
            // Set UI layer for streamer overlay camera
            SetUILayer(StreamerViewUI, "UI");
            
            Debug.Log("Switched to Streamer View");
        }

        private void SwitchToGameView()
        {
            // Game camera renders directly to screen
            GameCamera.targetTexture = null;
            
            // Disable streamer overlay camera
            StreamerOverlayCamera.enabled = false;
            
            // Switch UI
            StreamerViewUI.SetActive(false);
            GameViewUI.SetActive(true);
            
            // Set UI layer back to default
            SetUILayer(GameViewUI, "UI");
            
            Debug.Log("Switched to Game View");
        }

        private void SetUILayer(GameObject uiRoot, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            SetLayerRecursively(uiRoot, layer);
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        // All your existing message handling methods remain the same...
        public virtual void AddUserMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AddUserMessage>().HasValue) return;
            var data = message.Message<AddUserMessage>().GetValueOrDefault();
            AddUserMessage(data.User);
        }
        
        public void AddUserMessage(ViewerUser user)
        {
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
        
        public void PauseTime()
        {
            TimeManager.Instance.IsPaused = true;
        }
        
        public void ResetTime()
        {
            TimeManager.Instance.Reset();
        }
    }
}