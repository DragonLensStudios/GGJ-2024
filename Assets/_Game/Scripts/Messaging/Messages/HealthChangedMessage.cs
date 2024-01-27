using Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace Messaging.Messages
{
    public struct HealthChangedMessage
    {
        public HealthChangedOperation Operation { get; }
        public float Value { get; }
        [CanBeNull] public GameObject Source { get; }
        [CanBeNull] public GameObject Target { get; }
        
        public HealthChangedMessage(HealthChangedOperation operation, float value, GameObject source = null, GameObject target = null)
        {
            Operation = operation;
            Value = value;
            Source = source;
            Target = target;
        }
    }
}