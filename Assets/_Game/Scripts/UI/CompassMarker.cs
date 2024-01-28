using System;
using DLS.Enemy;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace DLS.UI
{
    public class CompassMarker : MonoBehaviour
    {
        [Tooltip("Main marker image")] public Image MainImage;

        [Tooltip("Canvas group for the marker")]
        public CanvasGroup CanvasGroup;

        [Header("Enemy element")] [Tooltip("Default color for the marker")]
        public Color DefaultColor;

        [Tooltip("Alternative color for the marker")]
        public Color AltColor;

        [Header("Direction element")] [Tooltip("Use this marker as a magnetic direction")]
        public bool IsDirection;

        [Tooltip("Text content for the direction")]
        public TMPro.TextMeshProUGUI TextContent;

        GameObject enemy;

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<TargetDetectionMessage>(MessageChannels.AI, TargetDetectionMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<TargetDetectionMessage>(MessageChannels.AI, TargetDetectionMessageHandler);

        }

        private void TargetDetectionMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<TargetDetectionMessage>().HasValue) return;
            var data = message.Message<TargetDetectionMessage>().GetValueOrDefault();
            if(data.Target != enemy) return;
            switch (data.DetectionType)
            {
                case DetectionType.Detected:
                    DetectTarget();
                    break;
                case DetectionType.Lost:
                    LostTarget();
                    break;
            }
        }

        public void Initialize(CompassElement compassElement, string textDirection)
        {
            if (IsDirection && TextContent)
            {
                TextContent.text = textDirection;
            }
            else
            {
                enemy = compassElement.gameObject;

                if (enemy != null)
                {
                    LostTarget();
                }
            }
        }

        public void DetectTarget()
        {
            MainImage.color = AltColor;
        }

        public void LostTarget()
        {
            MainImage.color = DefaultColor;
        }
    }
}