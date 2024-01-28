using System;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace DLS.Health
{
    public class HealthController : MonoBehaviour
    {
        [field:Tooltip("Maximum amount of health")]
        [field:SerializeField] public virtual float MaxHealth { get; set; } = 10f;
    
        [field:Tooltip("Health ratio at which the critical health vignette starts appearing")]
        [field:SerializeField] public virtual float CriticalHealthRatio { get; set; } = 0.3f;
    
        [field:Tooltip("Current Health")]
        [field:SerializeField] public virtual float CurrentHealth { get; set; }
    
        [field:Tooltip("Is the character invincible?")]
        [field:SerializeField] public virtual bool Invincible { get; set; }
    
        [field:Tooltip("Is the character dead?")]
        [field:SerializeField] public virtual bool IsDead { get; set; }
    
        public virtual bool CanPickup() => CurrentHealth < MaxHealth;
        public virtual float GetRatio() => CurrentHealth / MaxHealth;
        public virtual bool IsCritical() => GetRatio() <= CriticalHealthRatio;
    
        protected virtual void Start()
        {
            CurrentHealth = MaxHealth;
        }

        protected virtual void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<HealthChangedMessage>(MessageChannels.Health, HealthChangedMessageHandler);
        }

        protected virtual void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<HealthChangedMessage>(MessageChannels.Health, HealthChangedMessageHandler);
        }

        public virtual void Heal(float healAmount)
        {
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        }

        public virtual void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible)
                return;

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            HandleDeath();
        }

        public virtual void Kill()
        {
            CurrentHealth = 0f;
            HandleDeath();
        }

        public virtual void HandleDeath()
        {
            if (IsDead)
                return;

            // call OnDie action
            if (CurrentHealth <= 0f)
            {
                IsDead = true;
            }
        }
        
        public virtual void HealthChangedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<HealthChangedMessage>().HasValue) return;
            var data = message.Message<HealthChangedMessage>().GetValueOrDefault();
            if(data.Target != gameObject) return;
            switch (data.Operation)
            {
                case HealthChangedOperation.Healed:
                    Heal(data.Value);
                    break;
                case HealthChangedOperation.Damaged:
                    TakeDamage(data.Value, data.Source);
                    break;
                case HealthChangedOperation.Died:
                    Kill();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
