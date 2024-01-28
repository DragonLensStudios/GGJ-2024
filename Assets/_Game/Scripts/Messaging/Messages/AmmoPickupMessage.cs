using DLS.Weapons;

namespace DLS.Messaging.Messages
{
    public struct AmmoPickupMessage
    {
        public WeaponController Weapon { get; }
        public int? AmmoCount { get; }
        
        public AmmoPickupMessage(WeaponController weapon, int? ammoCount = null)
        {
            Weapon = weapon;
            AmmoCount = ammoCount;
        }
    }
}