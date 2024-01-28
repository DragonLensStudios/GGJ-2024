using System;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace DLS.Actor
{
    // This class contains general information describing an actor (player or enemies).
    // It is mostly used for AI detection logic and determining if an actor is friend or foe
    public class Actor : MonoBehaviour
    {
        [Tooltip("Represents the affiliation (or team) of the actor. Actors of the same affiliation are friendly to each other")]
        public int Affiliation;

        [Tooltip("Represents point where other actors will aim when they attack this actor")]
        public Transform AimPoint;

        void Start()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Actors, new ActorsChangedMessage(this, DataOperation.Add));
        }

        void OnDestroy()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Actors, new ActorsChangedMessage(this, DataOperation.Remove));
        }
    }
}