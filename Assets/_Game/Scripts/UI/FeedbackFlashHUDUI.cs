using Enums;
using Health;
using Messaging;
using Messaging.Messages;
using Player;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FeedbackFlashHUDUI : MonoBehaviour
    {
        [Header("References")] [Tooltip("Image component of the flash")]
        public Image FlashImage;

        [Tooltip("CanvasGroup to fade the damage flash, used when recieving damage end healing")]
        public CanvasGroup FlashCanvasGroup;

        [Tooltip("CanvasGroup to fade the critical health vignette")]
        public CanvasGroup VignetteCanvasGroup;

        [Header("Damage")] [Tooltip("Color of the damage flash")]
        public Color DamageFlashColor;

        [Tooltip("Duration of the damage flash")]
        public float DamageFlashDuration;

        [Tooltip("Max alpha of the damage flash")]
        public float DamageFlashMaxAlpha = 1f;

        [Header("Critical health")] [Tooltip("Max alpha of the critical vignette")]
        public float CriticaHealthVignetteMaxAlpha = .8f;

        [Tooltip("Frequency at which the vignette will pulse when at critical health")]
        public float PulsatingVignetteFrequency = 4f;

        [Header("Heal")] [Tooltip("Color of the heal flash")]
        public Color HealFlashColor;

        [Tooltip("Duration of the heal flash")]
        public float HealFlashDuration;

        [Tooltip("Max alpha of the heal flash")]
        public float HealFlashMaxAlpha = 1f;

        bool m_FlashActive;
        float m_LastTimeFlashStarted = Mathf.NegativeInfinity;
        protected FPSController playerController;
        protected HealthController m_PlayerHealth;
        GameFlowManager m_GameFlowManager;

        void Start()
        {
            // Subscribe to player damage events
            playerController = FindObjectOfType<FPSController>();
            m_PlayerHealth = playerController.GetComponent<HealthController>();
            m_GameFlowManager = FindObjectOfType<GameFlowManager>();
        }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<HealthChangedMessage>(MessageChannels.Health, HealthChangedMessageHandler);
            
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<HealthChangedMessage>(MessageChannels.Health, HealthChangedMessageHandler);
        }

        private void HealthChangedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<HealthChangedMessage>().HasValue) return;
            var data = message.Message<HealthChangedMessage>().GetValueOrDefault();
            if (data.Target != playerController.gameObject) return;
            switch (data.Operation)
            {
                case HealthChangedOperation.None:
                    break;
                case HealthChangedOperation.Damaged:
                    OnTakeDamage(data.Value, data.Source);
                    break;
                case HealthChangedOperation.Healed:
                    OnHealed(data.Value);
                    break;
                case HealthChangedOperation.Died:
                    break;

            }
        }

        void Update()
        {
            if (m_PlayerHealth.IsCritical())
            {
                VignetteCanvasGroup.gameObject.SetActive(true);
                float vignetteAlpha =
                    (1 - (m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth /
                          m_PlayerHealth.CriticalHealthRatio)) * CriticaHealthVignetteMaxAlpha;

                if (m_GameFlowManager.GameIsEnding)
                    VignetteCanvasGroup.alpha = vignetteAlpha;
                else
                    VignetteCanvasGroup.alpha =
                        ((Mathf.Sin(UnityEngine.Time.time * PulsatingVignetteFrequency) / 2) + 0.5f) * vignetteAlpha;
            }
            else
            {
                VignetteCanvasGroup.gameObject.SetActive(false);
            }


            if (m_FlashActive)
            {
                float normalizedTimeSinceDamage = (UnityEngine.Time.time - m_LastTimeFlashStarted) / DamageFlashDuration;

                if (normalizedTimeSinceDamage < 1f)
                {
                    float flashAmount = DamageFlashMaxAlpha * (1f - normalizedTimeSinceDamage);
                    FlashCanvasGroup.alpha = flashAmount;
                }
                else
                {
                    FlashCanvasGroup.gameObject.SetActive(false);
                    m_FlashActive = false;
                }
            }
        }

        void ResetFlash()
        {
            m_LastTimeFlashStarted = UnityEngine.Time.time;
            m_FlashActive = true;
            FlashCanvasGroup.alpha = 0f;
            FlashCanvasGroup.gameObject.SetActive(true);
        }

        void OnTakeDamage(float dmg, GameObject damageSource)
        {
            ResetFlash();
            FlashImage.color = DamageFlashColor;
        }

        void OnHealed(float amount)
        {
            ResetFlash();
            FlashImage.color = HealFlashColor;
        }
    }
}