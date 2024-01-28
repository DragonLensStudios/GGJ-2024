using DLS.Enums;
using UnityEngine;

namespace DLS.Messaging.Messages
{
    public struct TargetDetectionMessage
    {
        public DetectionType DetectionType { get; }
        public GameObject Target { get; }
        
        public TargetDetectionMessage(DetectionType detectionType, GameObject target)
        {
            DetectionType = detectionType;
            Target = target;
        }
    }
}