using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Health;
using Messaging;
using Messaging.Messages;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [field:SerializeField] public virtual Image HealthFillImage { get; set; }
    [field:SerializeField] public virtual TMP_Text HealthText { get; set; }

    protected FirstPersonBasicController playerController;
    protected HealthController playerHealthController;
    
    protected virtual void Awake()
    {
        playerController = FindObjectOfType<FirstPersonBasicController>();
        if(playerController == null) return;
        playerHealthController = playerController.GetComponent<HealthController>();
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    protected virtual void OnEnable()
    {
        MessageSystem.MessageManager.RegisterForChannel<HealthChangedMessage>(MessageChannels.UI, HealthChangedMessageHandler);
    }
    
    protected virtual void OnDisable()
    {
        MessageSystem.MessageManager.UnregisterForChannel<HealthChangedMessage>(MessageChannels.UI, HealthChangedMessageHandler);
    }
    
    public virtual void UpdateHealthBar()
    {
        HealthFillImage.fillAmount = playerHealthController.CurrentHealth / playerHealthController.MaxHealth;
        HealthText.text = $"{playerHealthController.CurrentHealth} / {playerHealthController.MaxHealth}";
    }
    
    public virtual void HealthChangedMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if(!message.Message<HealthChangedMessage>().HasValue) return;
        var data = message.Message<HealthChangedMessage>().GetValueOrDefault();
        if (data.Target != playerController.gameObject) return;
        switch (data.Operation)
        {
            case HealthChangedOperation.Damaged:
            case HealthChangedOperation.Healed:
                HealthFillImage.fillAmount = playerHealthController.CurrentHealth / playerHealthController.MaxHealth;
                break;
            case HealthChangedOperation.Died:
                HealthFillImage.fillAmount = 0f;
                break;
        }
        UpdateHealthBar();
    }
}
