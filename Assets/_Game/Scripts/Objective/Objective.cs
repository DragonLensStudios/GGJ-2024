using System;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace Objective
{
    public abstract class Objective : MonoBehaviour
    {
        [Tooltip("Name of the objective that will be shown on screen")]
        public string Title;

        [Tooltip("Short text explaining the objective that will be shown on screen")]
        public string Description;

        [Tooltip("Whether the objective is required to win or not")]
        public bool IsOptional;

        [Tooltip("Delay before the objective becomes visible")]
        public float DelayVisible;

        public bool IsCompleted { get; private set; }
        public bool IsBlocking() => !(IsOptional || IsCompleted);

        protected virtual void Start()
        {
            MessageSystem.MessageManager.BroadcastImmediate(new ObjectiveMessage(this, ObjectiveStatus.Created, Title));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new DisplayMessage(Title, 0.0f));
        }

        public void UpdateObjective(string descriptionText, string counterText, string notificationText)
        {
            
            MessageSystem.MessageManager.BroadcastImmediate(new ObjectiveMessage(this, ObjectiveStatus.Updated, descriptionText, counterText, notificationText));
        }

        public void CompleteObjective(string descriptionText, string counterText, string notificationText)
        {
            IsCompleted = true;

            MessageSystem.MessageManager.BroadcastImmediate(new ObjectiveMessage(this, ObjectiveStatus.Completed, descriptionText, counterText, notificationText));
        }
    }
}