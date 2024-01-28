using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace DLS.Weapons
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [field:SerializeField] public virtual GameObject Owner { get; set; }
        [field:SerializeField] public virtual Vector3 InitialPosition { get; set; }
        [field:SerializeField] public virtual Vector3 InitialDirection { get; set; }
        [field:SerializeField] public virtual Vector3 InheritedMuzzleVelocity { get; set; }
        [field:SerializeField] public virtual float InitialCharge { get; set; }
        
        public virtual void Shoot(WeaponController controller, GameObject source, GameObject target = null)
        {
            Owner = controller.Owner;
            InitialPosition = controller.transform.position;
            InitialDirection = controller.transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitialCharge = controller.CurrentCharge;
            
            MessageSystem.MessageManager.SendImmediate(Enums.MessageChannels.Weapons, new ProjectileShootMessage(this, source, target));
        }
        
    }
}