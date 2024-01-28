using DLS.Weapons;
using JetBrains.Annotations;
using UnityEngine;

namespace DLS.Messaging.Messages
{
    public struct AttackMessage
    {
        public GameObject Attacker { get; }
        public GameObject Target { get; }
        public float? Damage { get; }
        
        [CanBeNull] public WeaponController Weapon { get; }
        
        public AttackMessage(GameObject attacker, GameObject target, float? damage = null, [CanBeNull] WeaponController weapon = null)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
            Weapon = weapon;
        }
    }
}