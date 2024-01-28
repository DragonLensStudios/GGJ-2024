using System;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace Objective
{
    public class ObjectiveKillEnemies : Objective
    {
        [Tooltip("Chose whether you need to kill every enemies or only a minimum amount")]
        public bool MustKillAllEnemies = true;

        [Tooltip("If MustKillAllEnemies is false, this is the amount of enemy kills required")]
        public int KillsToCompleteObjective = 5;

        [Tooltip("Start sending notification about remaining enemies when this amount of enemies is left")]
        public int NotificationEnemiesRemainingThreshold = 3;

        int m_KillTotal;

        protected override void Start()
        {
            base.Start();
            
            // set a title and description specific for this type of objective, if it hasn't one
            if (string.IsNullOrEmpty(Title))
                Title = "Eliminate " + (MustKillAllEnemies ? "all the" : KillsToCompleteObjective.ToString()) +
                        " enemies";

            if (string.IsNullOrEmpty(Description))
                Description = GetUpdatedCounterAmount();
        }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<EnemyKillMessage>(MessageChannels.Objective, EnemyKillMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<EnemyKillMessage>(MessageChannels.Objective, EnemyKillMessageHandler);
        }

        private void EnemyKillMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<EnemyKillMessage>().HasValue) return;
            var data = message.Message<EnemyKillMessage>().GetValueOrDefault();
            if (IsCompleted)
                return;

            m_KillTotal++;

            if (MustKillAllEnemies)
                KillsToCompleteObjective = data.RemainingEnemyCount + m_KillTotal;

            int targetRemaining = MustKillAllEnemies ? data.RemainingEnemyCount : KillsToCompleteObjective - m_KillTotal;

            // update the objective text according to how many enemies remain to kill
            if (targetRemaining == 0)
            {
                CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }
            else if (targetRemaining == 1)
            {
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? "One enemy left"
                    : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
            else
            {
                // create a notification text if needed, if it stays empty, the notification will not be created
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? targetRemaining + " enemies to kill left"
                    : string.Empty;

                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
        }

        string GetUpdatedCounterAmount()
        {
            return m_KillTotal + " / " + KillsToCompleteObjective;
        }
    }
}