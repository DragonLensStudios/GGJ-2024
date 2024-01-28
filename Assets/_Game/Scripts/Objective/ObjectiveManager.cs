using System;
using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using UnityEngine;

namespace Objective
{
    public class ObjectiveManager : MonoBehaviour
    {
        List<Objective> m_Objectives = new List<Objective>();
        bool m_ObjectivesCompleted = false;
        
        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<ObjectiveMessage>(MessageChannels.Objective, ObjectiveMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<ObjectiveMessage>(MessageChannels.Objective, ObjectiveMessageHandler);
        }

        private void ObjectiveMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<ObjectiveMessage>().HasValue) return;
            var data = message.Message<ObjectiveMessage>().GetValueOrDefault();
            switch (data.Status)
            {
                case ObjectiveStatus.Created:
                    RegisterObjective(data.Objective);
                    break;
                case ObjectiveStatus.Updated:
                    break;
                case ObjectiveStatus.Completed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void RegisterObjective(Objective objective) => m_Objectives.Add(objective);

        void Update()
        {
            if (m_Objectives.Count == 0 || m_ObjectivesCompleted)
                return;

            for (int i = 0; i < m_Objectives.Count; i++)
            {
                // pass every objectives to check if they have been completed
                if (m_Objectives[i].IsBlocking())
                {
                    // break the loop as soon as we find one uncompleted objective
                    return;
                }
            }

            m_ObjectivesCompleted = true;
            EventManager.Broadcast(Events.AllObjectivesCompletedEvent);
        }
        
    }
}