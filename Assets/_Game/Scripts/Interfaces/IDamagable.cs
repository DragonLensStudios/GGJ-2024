
using UnityEngine;

namespace DLS.Interfaces
{
    public interface IDamagable
    {
        GameObject GameObject { get; }
        float DamageMultiplier { get; set; }
        float SensibilityToSelfDamage { get; set; }
        void TakeDamage(float damage, bool isExplosionDamage, GameObject source, GameObject target = null);
        void Kill(GameObject source, GameObject target);
    }
}
