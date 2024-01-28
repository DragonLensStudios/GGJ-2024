using System;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using UnityEngine;

namespace Objective
{
    public class ObjectivePickupItem : Objective
    {
        [Tooltip("Item to pickup to complete the objective")]
        public GameObject ItemToPickup;

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<PickupObjectMessage>(MessageChannels.Items, PickupObjectMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<PickupObjectMessage>(MessageChannels.Items, PickupObjectMessageHandler);
        }

        private void PickupObjectMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PickupObjectMessage>().HasValue) return;
            var data = message.Message<PickupObjectMessage>().GetValueOrDefault();
            if (IsCompleted || ItemToPickup != data.PickupObject)
                return;

            // this will trigger the objective completion
            // it works even if the player can't pickup the item (i.e. objective pickup healthpack while at full heath)
            CompleteObjective(string.Empty, string.Empty, "Objective complete : " + Title);

            if (gameObject)
            {
                Destroy(gameObject);
            }
        }
    }
}