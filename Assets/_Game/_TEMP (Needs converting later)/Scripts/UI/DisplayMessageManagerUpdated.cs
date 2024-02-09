using System.Collections.Generic;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using FPS.Scripts.UI;
using UnityEngine;

public class DisplayMessageManagerUpdated : MonoBehaviour
{
    [field: SerializeField] public UITable InGameMessageRect { get; set; }
    [field: SerializeField] public UITable InStreamMessageRect { get; set; }
    [field: SerializeField] public NotificationToast MessagePrefab { get; set; }
    
    protected List<(float timestamp, float delay, string message, NotificationToast inGameNotification, NotificationToast inStreamNotification)> pendingMessages;

    private void OnEnable()
    {
        EventManager.AddListener<DisplayMessageEvent>(OnDisplayMessageEvent);
        pendingMessages = new List<(float, float, string, NotificationToast, NotificationToast)>();
    }

    private void OnDisplayMessageEvent(DisplayMessageEvent evt)
    {
        // Instantiate one notification for each UI
        NotificationToast inGameNotification = Instantiate(MessagePrefab, InGameMessageRect.transform).GetComponent<NotificationToast>();
        NotificationToast inStreamNotification = Instantiate(MessagePrefab, InStreamMessageRect.transform).GetComponent<NotificationToast>();

        pendingMessages.Add((Time.time, evt.DelayBeforeDisplay, evt.Message, inGameNotification, inStreamNotification));
    }
    
    private void Update()
    {
        foreach (var message in pendingMessages)
        {
            if (Time.time - message.timestamp > message.delay && !message.inGameNotification.Initialized && !message.inStreamNotification.Initialized)
            {
                // Initialize both notifications with the same message
                message.inGameNotification.Initialize(message.message);
                message.inStreamNotification.Initialize(message.message);

                DisplayMessage(message.inGameNotification, InGameMessageRect);
                DisplayMessage(message.inStreamNotification, InStreamMessageRect);
            }
        }

        // Clear deprecated messages
        pendingMessages.RemoveAll(x => x.inGameNotification.Initialized && x.inStreamNotification.Initialized);
    }
    
    private void DisplayMessage(NotificationToast notification, UITable targetUI)
    {
        targetUI.UpdateTable(notification.gameObject);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener<DisplayMessageEvent>(OnDisplayMessageEvent);
    }
}
