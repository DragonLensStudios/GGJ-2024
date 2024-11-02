using System.Collections;
using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using UnityEngine;

public class ReloadLevelController : MonoBehaviour
{
    public void ReloadLevel()
    {
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Level , new LevelMessage(GameManager.Instance.CurrentLevel.LevelName, LevelOperation.Reload));
    }
}
