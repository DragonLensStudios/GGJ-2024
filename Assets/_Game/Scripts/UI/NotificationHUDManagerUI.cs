using System;
using Enums;
using Messaging;
using Messaging.Messages;
using Unity.FPS.UI;
using UnityEngine;
using WeaponController = Weapons.WeaponController;

namespace UI
{
    public class NotificationHUDManagerUI : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying notifications")]
        public RectTransform NotificationPanel;

        [Tooltip("Prefab for the notifications")]
        public GameObject NotificationPrefab;

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<ObjectiveUpdateMessage>(MessageChannels.Gameplay, ObjectiveUpdateMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<WeaponMessage>(MessageChannels.Weapons, WeaponMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<ObjectiveUpdateMessage>(MessageChannels.Gameplay, ObjectiveUpdateMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<WeaponMessage>(MessageChannels.Weapons, WeaponMessageHandler);

        }

        private void WeaponMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<WeaponMessage>().HasValue) return;
            var data = message.Message<WeaponMessage>().GetValueOrDefault();
            if (data.Operation == WeaponChangedOperation.PickedUp)
            {
                if (data.Weapon != null)
                {
                    OnPickupWeapon(data.Weapon, data.Index.GetValueOrDefault());
                }
            }
        }

        private void ObjectiveUpdateMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<ObjectiveUpdateMessage>().HasValue) return;
            var data = message.Message<ObjectiveUpdateMessage>().GetValueOrDefault();
            if (!string.IsNullOrEmpty(data.NotificationText))
            {
                CreateNotification(data.NotificationText);
            }
        }

        void OnPickupWeapon(WeaponController weaponController, int index)
        {
            if (index != 0)
            {
                CreateNotification("Picked up weapon : " + weaponController.WeaponName);
            }
        }

        public void CreateNotification(string text)
        {
            GameObject notificationInstance = Instantiate(NotificationPrefab, NotificationPanel);
            notificationInstance.transform.SetSiblingIndex(0);

            NotificationToast toast = notificationInstance.GetComponent<NotificationToast>();
            if (toast)
            {
                toast.Initialize(text);
            }
        }
    }
}