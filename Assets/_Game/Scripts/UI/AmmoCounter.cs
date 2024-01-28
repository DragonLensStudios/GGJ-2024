using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using DLS.Player;
using FPS.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeaponController = DLS.Weapons.WeaponController;

namespace DLS.UI
{
    [RequireComponent(typeof(FillBarColorChange))]
    public class AmmoCounter : MonoBehaviour
    {
        [Tooltip("CanvasGroup to fade the ammo UI")]
        public CanvasGroup CanvasGroup;

        [Tooltip("Image for the weapon icon")] public Image WeaponImage;

        [Tooltip("Image component for the background")]
        public Image AmmoBackgroundImage;

        [Tooltip("Image component to display fill ratio")]
        public Image AmmoFillImage;

        [Tooltip("Text for Weapon index")] 
        public TextMeshProUGUI WeaponIndexText;

        [Tooltip("Text for Bullet Counter")] 
        public TextMeshProUGUI BulletCounter;

        [Tooltip("Reload Text for Weapons with physical bullets")]
        public RectTransform Reload;

        [Header("Selection")] [Range(0, 1)] [Tooltip("Opacity when weapon not selected")]
        public float UnselectedOpacity = 0.5f;

        [Tooltip("Scale when weapon not selected")]
        public Vector3 UnselectedScale = Vector3.one * 0.8f;

        [Tooltip("Root for the control keys")] public GameObject ControlKeysRoot;

        [Header("Feedback")] [Tooltip("Component to animate the color when empty or full")]
        public FillBarColorChange FillBarColorChange;

        [Tooltip("Sharpness for the fill ratio movements")]
        public float AmmoFillMovementSharpness = 20f;

        public int WeaponCounterIndex { get; set; }

        protected FPSController playerController;
        protected WeaponController weapon;

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<AmmoPickupMessage>(MessageChannels.Weapons, AmmoPickupMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<AmmoPickupMessage>(MessageChannels.Weapons, AmmoPickupMessageHandler);
        }
        
        private void AmmoPickupMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AmmoPickupMessage>().HasValue) return;
            var data = message.Message<AmmoPickupMessage>().GetValueOrDefault();
            if (data.Weapon == weapon)
            {
                BulletCounter.text = weapon.GetCarriedPhysicalBullets().ToString();
            }
        }

        public void Initialize(WeaponController weapon, int weaponIndex)
        {
            this.weapon = weapon;
            WeaponCounterIndex = weaponIndex;
            WeaponImage.sprite = weapon.WeaponIcon;
            if (!weapon.HasPhysicalBullets)
                BulletCounter.transform.parent.gameObject.SetActive(false);
            else
                BulletCounter.text = weapon.GetCarriedPhysicalBullets().ToString();

            Reload.gameObject.SetActive(false);
            playerController = FindObjectOfType<FPSController>();

            WeaponIndexText.text = (WeaponCounterIndex + 1).ToString();

            FillBarColorChange.Initialize(1f, this.weapon.GetAmmoNeededToShoot());
        }

        void Update()
        {
            float currenFillRatio = weapon.CurrentAmmoRatio;
            AmmoFillImage.fillAmount = Mathf.Lerp(AmmoFillImage.fillAmount, currenFillRatio,
                UnityEngine.Time.deltaTime * AmmoFillMovementSharpness);

            BulletCounter.text = weapon.GetCarriedPhysicalBullets().ToString();

            bool isActiveWeapon = weapon == playerController.GetActiveWeapon();

            CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, isActiveWeapon ? 1f : UnselectedOpacity,
                UnityEngine.Time.deltaTime * 10);
            transform.localScale = Vector3.Lerp(transform.localScale, isActiveWeapon ? Vector3.one : UnselectedScale,
                UnityEngine.Time.deltaTime * 10);
            ControlKeysRoot.SetActive(!isActiveWeapon);

            FillBarColorChange.UpdateVisual(currenFillRatio);

            Reload.gameObject.SetActive(weapon.GetCarriedPhysicalBullets() > 0 && weapon.GetCurrentAmmo() == 0 && weapon.IsWeaponActive);
        }
    }
}