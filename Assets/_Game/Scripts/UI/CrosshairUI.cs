using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using DLS.Player;
using DLS.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace DLS.UI
{
    public class CrosshairUI : MonoBehaviour
    {
        [field:SerializeField] public Image CrosshairImage { get; set; }
        [field:SerializeField] public Sprite NullCrosshairSprite { get; set; }
        [field:SerializeField] public float CrosshairUpdateSharpness { get; set; } = 5f;

        protected bool wasPointingAtTarget;
        protected RectTransform crosshairRectTransform;
        protected CrosshairData crosshairDataDefault;
        protected CrosshairData crosshairDataTarget;
        protected CrosshairData currentCrosshair;
        
        protected FPSController playerController;
        
        protected virtual void Awake()
        {
            playerController = FindObjectOfType<FPSController>();
            OnWeaponChanged(playerController.GetActiveWeapon());
        }

        protected virtual void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<WeaponMessage>(MessageChannels.UI, WeaponMessageHandler);
        }

        protected virtual void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<WeaponMessage>(MessageChannels.UI, WeaponMessageHandler);
        }
        
        public virtual void UpdateCrosshairPointingAtEnemy(bool force)
        {
            if (crosshairDataDefault.CrosshairSprite == null)
                return;

            if ((force || !wasPointingAtTarget) && playerController.IsPointingAtTarget)
            {
                currentCrosshair = crosshairDataTarget;
                CrosshairImage.sprite = currentCrosshair.CrosshairSprite;
                crosshairRectTransform.sizeDelta = currentCrosshair.CrosshairSize * Vector2.one;
            }
            else if ((force || wasPointingAtTarget) && !playerController.IsPointingAtTarget)
            {
                currentCrosshair = crosshairDataDefault;
                CrosshairImage.sprite = currentCrosshair.CrosshairSprite;
                crosshairRectTransform.sizeDelta = currentCrosshair.CrosshairSize * Vector2.one;
            }

            CrosshairImage.color = Color.Lerp(CrosshairImage.color, currentCrosshair.CrosshairColor,
                UnityEngine.Time.deltaTime * CrosshairUpdateSharpness);
            crosshairRectTransform.sizeDelta = Mathf.Lerp(crosshairRectTransform.sizeDelta.x,
                currentCrosshair.CrosshairSize,
                UnityEngine.Time.deltaTime * CrosshairUpdateSharpness) * Vector2.one;
        }

        void OnWeaponChanged(WeaponController newWeapon)
        {
            if (newWeapon)
            {
                CrosshairImage.enabled = true;
                crosshairDataDefault = newWeapon.CrosshairDataDefault;
                crosshairDataTarget = newWeapon.CrosshairDataTargetInSight;
                crosshairRectTransform = CrosshairImage.GetComponent<RectTransform>();
            }
            else
            {
                if (NullCrosshairSprite)
                {
                    CrosshairImage.sprite = NullCrosshairSprite;
                }
                else
                {
                    CrosshairImage.enabled = false;
                }
            }

            UpdateCrosshairPointingAtEnemy(true);
        }
        
        private void WeaponMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<WeaponMessage>().HasValue) return;
            var data = message.Message<WeaponMessage>().GetValueOrDefault();
            switch(data.Operation)
            {
                case WeaponChangedOperation.Equipped:
                case WeaponChangedOperation.Switched:
                    OnWeaponChanged(data.Weapon);
                    break;
            }
        }
    }
}