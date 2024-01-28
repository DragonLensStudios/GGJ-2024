using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using FPS.Scripts.UI;
using UnityEngine;
using WeaponController = DLS.Weapons.WeaponController;

namespace DLS.UI
{
    public class NotificationHUDManagerUI : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying notifications")]
        public RectTransform NotificationPanel;

        [Tooltip("Prefab for the notifications")]
        public GameObject NotificationPrefab;

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<ObjectiveMessage>(MessageChannels.Gameplay, ObjectiveUpdateMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<WeaponMessage>(MessageChannels.Weapons, WeaponMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<ObjectiveMessage>(MessageChannels.Gameplay, ObjectiveUpdateMessageHandler);
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
            if(!message.Message<ObjectiveMessage>().HasValue) return;
            var data = message.Message<ObjectiveMessage>().GetValueOrDefault();
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