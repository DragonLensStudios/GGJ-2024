using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using DLS.Player;
using DLS.Weapons;
using UnityEngine;

namespace DLS.UI
{
    public class WeaponHUDUI : MonoBehaviour
    {
        [field:Tooltip("UI panel containing the layoutGroup for displaying weapon ammo")]
        [field:SerializeField] public virtual RectTransform AmmoPanel { get; set; }
    
        [field:Tooltip("Prefab for displaying weapon ammo")]
        [field:SerializeField] public virtual GameObject AmmoCounterPrefab { get; set; }

        protected FPSController playerController;
        protected List<AmmoCounter> ammoCounters = new ();

        private void Awake()
        {
            playerController = FindObjectOfType<FPSController>();

            WeaponController activeWeapon = playerController.GetActiveWeapon();
            if (activeWeapon)
            {
                AddWeapon(activeWeapon, playerController.ActiveWeaponIndex);
                ChangeWeapon(activeWeapon);
            }
        }

        public virtual void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<WeaponMessage>(MessageChannels.Weapons, WeaponMessageHandler);
        }

        public virtual void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<WeaponMessage>(MessageChannels.Weapons, WeaponMessageHandler);
        }
    
        public virtual void WeaponMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<WeaponMessage>().HasValue) return;
            var data = message.Message<WeaponMessage>().GetValueOrDefault();
            switch (data.Operation)
            {
                case WeaponChangedOperation.Added:
                    AddWeapon(data.Weapon, data.Index.GetValueOrDefault());
                    break;
                case WeaponChangedOperation.Removed:
                    RemoveWeapon(data.Weapon, data.Index.GetValueOrDefault());
                    break;
                case WeaponChangedOperation.Switched:
                    ChangeWeapon(data.Weapon);
                    break;
            }
        }

        protected virtual void AddWeapon(WeaponController newWeapon, int weaponIndex)
        {
            GameObject ammoCounterInstance = Instantiate(AmmoCounterPrefab, AmmoPanel);
            AmmoCounter newAmmoCounter = ammoCounterInstance.GetComponent<AmmoCounter>();

            newAmmoCounter.Initialize(newWeapon, weaponIndex);

            ammoCounters.Add(newAmmoCounter);
        }
    
        protected virtual void RemoveWeapon(WeaponController newWeapon, int weaponIndex)
        {
            int foundCounterIndex = -1;
            for (int i = 0; i < ammoCounters.Count; i++)
            {
                if (ammoCounters[i].WeaponCounterIndex == weaponIndex)
                {
                    foundCounterIndex = i;
                    Destroy(ammoCounters[i].gameObject);
                }
            }

            if (foundCounterIndex != -1)
            {
                ammoCounters.RemoveAt(foundCounterIndex);
            }
        }
    
        protected virtual void ChangeWeapon(WeaponController newWeapon)
        {
            foreach (var ammoCounter in ammoCounters)
            {
                // ammoCounter.UpdateAmmoText();
            }
        }
    }
}
