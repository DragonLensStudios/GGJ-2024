using Enums;
using WeaponController = Weapons.WeaponController;

namespace Messaging.Messages
{
    public struct WeaponMessage
    {
        public WeaponChangedOperation Operation { get; }
        public WeaponController Weapon { get; }
        public int? Index { get; }
        public WeaponSwitchState? WeaponSwitchState { get; }

        public WeaponMessage(WeaponChangedOperation operation, WeaponController weapon, int? index = null, WeaponSwitchState? weaponSwitchState = null)
        {
            Operation = operation;
            Weapon = weapon;
            Index = index;
            WeaponSwitchState = weaponSwitchState;
        }
    }
}