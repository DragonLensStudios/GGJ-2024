using DLS.Enums;
using DLS.Interfaces;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace Damage
{
    public class Damageable : MonoBehaviour, IDamagable
    {
        [field:SerializeField] public float DamageMultiplier { get; set; }
        [field:Range(0, 1)]
        [field:SerializeField] public float SensibilityToSelfDamage { get; set; }
        public virtual void TakeDamage(float damage, bool isExplosionDamage, GameObject source, GameObject target = null)
        {
            if(target != gameObject) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Health, new HealthChangedMessage(HealthChangedOperation.Damaged, damage, source, target));
        }

        public virtual void Kill(GameObject source, GameObject target)
        {
            if(target != gameObject) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Health, new HealthChangedMessage(HealthChangedOperation.Died, 0, source, gameObject));
        }
    }
}