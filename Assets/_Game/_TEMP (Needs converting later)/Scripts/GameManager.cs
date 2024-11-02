using System;
using System.Collections;
using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [field: SerializeField] public List<Level> Levels { get; set; } = new();
    [field: SerializeField] public Level CurrentLevel { get; set; }
    
    [field: SerializeField] public Difficulty CurrentDifficulty { get; set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        MessageSystem.MessageManager.RegisterForChannel<LevelMessage>(MessageChannels.Level, HandleLevelMessage);
        MessageSystem.MessageManager.RegisterForChannel<DifficultyMessage>(MessageChannels.Difficulty, HandleDifficultyMessage);
    }

    private void OnDisable()
    {
        MessageSystem.MessageManager.UnregisterForChannel<LevelMessage>(MessageChannels.Level, HandleLevelMessage);
        MessageSystem.MessageManager.UnregisterForChannel<DifficultyMessage>(MessageChannels.Difficulty, HandleDifficultyMessage);
    }

    private void HandleDifficultyMessage(MessageSystem.IMessageEnvelope message)
    {
        if(!message.Message<DifficultyMessage>().HasValue) return;
        var data = message.Message<DifficultyMessage>().GetValueOrDefault();
        CurrentDifficulty = data.Difficulty;
    }

    private void HandleLevelMessage(MessageSystem.IMessageEnvelope message)
    {
        if(!message.Message<LevelMessage>().HasValue) return;
        var data = message.Message<LevelMessage>().GetValueOrDefault();
        switch (data.Operation)
        {
            case LevelOperation.Load:
                if (data.Level != null)
                {
                    LoadLevel(data.Level);
                }
                else
                {
                    LoadLevel(data.LevelName);
                }
                break;
            case LevelOperation.Unload:
                if (data.Level != null)
                {
                    UnloadLevel(data.Level);
                }
                else
                {
                    UnloadLevel(data.LevelName);
                }
                break;
            case LevelOperation.Reload:
                ReloadLevel();
                break;
        }
    }

    public void ReloadLevel()
    {
        if (CurrentLevel != null)
        {
            SceneManager.LoadSceneAsync(CurrentLevel.SceneName);
        }
    }

    public void UnloadLevel(Level dataLevel)
    {
        CurrentLevel = null;
        CurrentDifficulty = Difficulty.None;
        SceneManager.UnloadSceneAsync(dataLevel.SceneName);
    }

    public void LoadLevel(Level dataLevel)
    {
        CurrentLevel = dataLevel;
        CurrentDifficulty = dataLevel.Difficulty;
        SceneManager.LoadSceneAsync(dataLevel.SceneName);
    }
    
    public void LoadLevel(string levelName)
    {
        var level = Levels.Find(x => x.LevelName == levelName);
        if (level != null)
        {
            LoadLevel(level);
        }
    }
    
    public void UnloadLevel(string levelName)
    {
        var level = Levels.Find(x => x.LevelName == levelName);
        if (level != null)
        {
            UnloadLevel(level);
        }
    }
    
    public void SetDifficulty(Difficulty difficulty)
    {
        MessageSystem.MessageManager.Send(new DifficultyMessage(difficulty), MessageChannels.Difficulty);
    }
}
