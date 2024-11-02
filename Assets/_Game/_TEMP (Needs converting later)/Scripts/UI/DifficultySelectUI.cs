using System.Collections;
using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using UnityEngine;

public class DifficultySelectUI : MonoBehaviour
{
    public void LoadLevel(string levelName)
    {
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Level,new LevelMessage(levelName, LevelOperation.Load));
    }
}
