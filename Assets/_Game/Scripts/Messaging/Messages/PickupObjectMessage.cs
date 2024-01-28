using UnityEngine;

namespace DLS.Messaging.Messages
{
    public struct PickupObjectMessage
    {
        public GameObject PickupObject { get; }
        
        public PickupObjectMessage(GameObject pickupObject)
        {
            PickupObject = pickupObject;
        }
    }
}