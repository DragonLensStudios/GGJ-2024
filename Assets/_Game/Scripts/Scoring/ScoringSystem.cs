using System.Collections;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using Enums;
using Messaging;
using Messaging.Messages;
using Objective;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ScoringSystem : MonoBehaviour
{
    [field: SerializeField] public long PlayerScore { get; set; }
    [field: SerializeField] public long PlayerHighScore { get; set; }
    [field: SerializeField] public long Viewers { get; set; }
    [field: SerializeField] public virtual TMP_Text ScoreText { get; set; }
    [field: SerializeField] public virtual TMP_Text HighScoreText { get; set; }
    

    protected void Start()
    {
        ScoreText.text = $"Score: {PlayerScore}";
        Debug.Log("start");
        PlayerHighScore = PlayerPrefs.GetInt("PlayerHighScore");
        //HighScoreText.text = $"High Score: {PlayerHighScore}";
    }

    protected void OnEnable()
    {
        MessageSystem.MessageManager.RegisterForChannel<ScoreMessage>(MessageChannels.UI, ScoreMessageHandler);
        StartCoroutine(ScoreTickCoroutine(1, 2.5f));
    }

    protected void OnDisable()
    {
        MessageSystem.MessageManager.UnregisterForChannel<ScoreMessage>(MessageChannels.UI, ScoreMessageHandler);
        StopCoroutine(ScoreTickCoroutine(1, 2.5f));
    }

    private IEnumerator ScoreTickCoroutine(long scoreValue, float delay = 1f)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            //AddScore(scoreValue);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI,new ScoreMessage(MathOperation.Add, scoreType: ScoreType.None, value: scoreValue));
        }
    }
    
    public virtual void ScoreMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if (!message.Message<ScoreMessage>().HasValue) return;
        var data = message.Message<ScoreMessage>().GetValueOrDefault();
        switch (data.Operation)
        {
            case MathOperation.Add:
                switch (data.ScoreType)
                {
                    case ScoreType.None:
                        PlayerScore += data.Value;
                        break;
                    case ScoreType.Viewer:
                        PlayerScore += data.Value * (long)ScoreType.Viewer;
                        break;
                    case ScoreType.Subscriber:
                        PlayerScore += data.Value * (long)ScoreType.Subscriber;
                        break;
                    case ScoreType.Donation:
                        PlayerScore += data.Value * (long)ScoreType.Donation;
                        break;
                }
                break;
            case MathOperation.Subtract:
                switch (data.ScoreType)
                {
                    case ScoreType.None:
                        PlayerScore -= data.Value;
                        break;
                    case ScoreType.Viewer:
                        PlayerScore -= data.Value * (long)ScoreType.Viewer;
                        break;
                    case ScoreType.Subscriber:
                        PlayerScore -= data.Value * (long)ScoreType.Subscriber;
                        break;
                    case ScoreType.Donation:
                        PlayerScore -= data.Value * (long)ScoreType.Donation;
                        break;
                }
                break;
            case MathOperation.Set:
                switch (data.ScoreType)
                {
                    case ScoreType.None:
                        PlayerScore *= data.Value;
                        break;
                    case ScoreType.Viewer:
                        PlayerScore *= data.Value * (long)ScoreType.Viewer;
                        break;
                    case ScoreType.Subscriber:
                        PlayerScore *= data.Value * (long)ScoreType.Subscriber;
                        break;
                    case ScoreType.Donation:
                        PlayerScore *= data.Value * (long)ScoreType.Donation;
                        break;
                }
                break;
            case MathOperation.Multiply:
                PlayerScore *= data.Value;
                break;
            case MathOperation.Divide:
                if (data.Value != 0)
                {
                    switch (data.ScoreType)
                    {
                        case ScoreType.None:
                            PlayerScore /= data.Value;
                            break;
                        case ScoreType.Viewer:
                            PlayerScore /= data.Value * (long)ScoreType.Viewer;
                            break;
                        case ScoreType.Subscriber:
                            PlayerScore /= data.Value * (long)ScoreType.Subscriber;
                            break;
                        case ScoreType.Donation:
                            PlayerScore /= data.Value * (long)ScoreType.Donation;
                            break;
                    }
                }
                break;
        }
        ScoreText.text = $"Score: {PlayerScore}";
        // if (PlayerScore > PlayerHighScore)
        // {
        //     PlayerHighScore = PlayerScore;
        //    // PlayerPrefs.SetInt("PlayerHighScore", PlayerHighScore);
        //     HighScoreText.text = $"New High Score: {PlayerHighScore}";
        // }
        // else
        // {
        //     HighScoreText.text = $"High Score: {PlayerHighScore}";
        // }
    }

    public virtual void ViewersMessageHandler(MessageSystem.IMessageEnvelope message) {

        if (!message.Message<ViewerMessage>().HasValue) return;
        //var data = message.Message<ViewerMessage>().GetValueOrDefault();
    }
}