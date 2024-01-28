using System;
using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using FPS.Scripts.UI;//TODO: Convert this to use the new not use FPS references.
using UnityEngine;
using DisplayMessage = DLS.Messaging.Messages.DisplayMessage;

namespace DLS.UI
{
    //TODO: Convert this to use the new not use FPS references.
    public class DisplayMessageManager : MonoBehaviour
    {
        public UITable DisplayMessageRect;
        public NotificationToast MessagePrefab;

        List<(float timestamp, float delay, string message, NotificationToast notification)> m_PendingMessages;

        void Awake()
        {
            m_PendingMessages = new List<(float, float, string, NotificationToast)>();
        }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<DisplayMessage>(MessageChannels.UI, DisplayMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<DisplayMessage>(MessageChannels.UI, DisplayMessageHandler);
        }
        
        private void DisplayMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<DisplayMessage>().HasValue) return;
            var data = message.Message<DisplayMessage>().GetValueOrDefault();
            NotificationToast notification = Instantiate(MessagePrefab, DisplayMessageRect.transform).GetComponent<NotificationToast>();
            m_PendingMessages.Add((UnityEngine.Time.time, data.Delay, data.Message, notification));
        }

        void Update()
        {
            foreach (var message in m_PendingMessages)
            {
                if (UnityEngine.Time.time - message.timestamp > message.delay)
                {
                    message.Item4.Initialize(message.message);
                    DisplayMessage(message.notification);
                }
            }

            // Clear deprecated messages
            m_PendingMessages.RemoveAll(x => x.notification.Initialized);
        }

        void DisplayMessage(NotificationToast notification)
        {
            DisplayMessageRect.UpdateTable(notification.gameObject);
        }
    }
}