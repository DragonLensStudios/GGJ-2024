using Enums;
using Messaging;
using Messaging.Messages;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ScoringSystem : MonoBehaviour
{
    [field: SerializeField] public long PlayerScore { get; set; } = 0;
    protected void OnEnable()
    {
        MessageSystem.MessageManager.RegisterForChannel<ScoreMessage>(MessageChannels.UI, ScoreMessageHandler);
    }

    protected void OnDisable()
    {
        MessageSystem.MessageManager.UnregisterForChannel<ScoreMessage>(MessageChannels.UI, ScoreMessageHandler);
    }

    public virtual void ScoreMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if(!message.Message<ScoreMessage>().HasValue) return;
        var data = message.Message<ScoreMessage>().GetValueOrDefault();
        switch (data.Operation)
        {
            case MathOperation.Add:
                PlayerScore += data.Value;
                break;
            case MathOperation.Subtract:
                PlayerScore -= data.Value;
                break;
            case MathOperation.Set:
                PlayerScore = data.Value;
                break;
            case MathOperation.Multiply:
                PlayerScore *= data.Value;
                break;
            case MathOperation.Divide:
                if(data.Value != 0)
                {
                    PlayerScore /= data.Value;
                }
                break;
        }
    }
}