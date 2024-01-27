using UnityEngine;
using Weapons;

namespace Messaging.Messages
{
    public struct ProjectileShootMessage
    {
        public ProjectileBase Projectile { get; }
        public GameObject Source { get; }
        public GameObject Target { get; }
        
        public ProjectileShootMessage(ProjectileBase projectile, GameObject source, GameObject target = null)
        {
            Projectile = projectile;
            Source = source;
            Target = target;
        }
    }
}