using Enums;
using Messaging;
using Messaging.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ScoringSystem : MonoBehaviour
{
    [field: SerializeField] public int PlayerScore { get; set; } = 0;
    [field: SerializeField] public int PlayerHighScore { get; set; } = 0;
    [field: SerializeField] public virtual TMP_Text ScoreText { get; set; }
    [field: SerializeField] public virtual TMP_Text HighScoreText { get; set; }

    protected void Start()
    {
        ScoreText.text = $"Score: {PlayerScore}";
        PlayerHighScore = PlayerPrefs.GetInt("PlayerHighScore");
        HighScoreText.text = $"High Score: {PlayerHighScore}";
    }

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
            case MathOperation.Set:
                PlayerScore = data.Value;
                break;
            case MathOperation.Multiply:
                PlayerScore *= data.Value;
                break;
            case MathOperation.Divide:
                if (data.Value != 0)
                {
                    PlayerScore /= data.Value;
                }
                break;
        }
        ScoreText.text = $"Score: {PlayerScore}";
        if (PlayerScore > PlayerHighScore)
        {
            PlayerHighScore = PlayerScore;
            PlayerPrefs.SetInt("PlayerHighScore", PlayerHighScore);
            HighScoreText.text = $"New High Score: {PlayerHighScore}";
        }
        else
        {
            HighScoreText.text = $"High Score: {PlayerHighScore}";
        }
    }
}