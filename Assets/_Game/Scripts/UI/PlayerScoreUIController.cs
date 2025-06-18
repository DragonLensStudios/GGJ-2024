using System;
using DLS.Enums;
using DLS.Messaging;
using Enums;
using Messaging.Messages;
using TMPro;
using UnityEngine;

public class PlayerScoreUIController : MonoBehaviour
{
    [field: SerializeField] public long PlayerScore { get; set; }
    [field: SerializeField] public TMP_Text ScoreText { get; set; }
    private void OnEnable()
    {
        MessageSystem.MessageManager.RegisterForChannel<ScoreMessage>(MessageChannels.UI, HandleScoreMessage);
    }

    private void HandleScoreMessage(MessageSystem.IMessageEnvelope message)
    {
        if (!message.Message<ScoreMessage>().HasValue) return;
        var data = message.Message<ScoreMessage>().GetValueOrDefault();
        switch (data.Operation)
        {
            case MathOperation.Add:
                PlayerScore += data.Value;
                break;
            case MathOperation.Subtract:
                PlayerScore -= data.Value;
                break;
            
        }

        if (ScoreText != null)
        {
            ScoreText.text = $"Score: {PlayerScore}";
        }
    }

    private void OnDisable()
    {
        MessageSystem.MessageManager.UnregisterForChannel<ScoreMessage>(MessageChannels.UI, HandleScoreMessage);
    }
}
