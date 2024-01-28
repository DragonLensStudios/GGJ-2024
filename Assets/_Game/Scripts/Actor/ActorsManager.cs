using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace DLS.Actor
{
    public class ActorsManager : MonoBehaviour
    {
        public List<Actor> Actors { get; private set; }
        public GameObject Player { get; private set; }

        public void SetPlayer(GameObject player) => Player = player;

        void Awake()
        {
            Actors = new List<Actor>();
        }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<ActorsChangedMessage>(MessageChannels.Actors, ActorsChangedMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<ActorsChangedMessage>(MessageChannels.Actors, ActorsChangedMessageHandler);
        }

        private void ActorsChangedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<ActorsChangedMessage>().HasValue) return;
            var data = message.Message<ActorsChangedMessage>().GetValueOrDefault();
            switch (data.Operation)
            {
                case DataOperation.Add:
                    if (!Actors.Contains(data.Actor))
                    {
                        Actors.Add(data.Actor);
                    }
                    break;
                case DataOperation.Remove:
                    if (Actors.Contains(data.Actor))
                    {
                        Actors.Remove(data.Actor);
                    }
                    break;
            }
        }
    }
}