using System.Collections;
using System.Collections.Generic;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using FPS.Scripts.Game.Shared;
using FPS.Scripts.Gameplay;
using FPS.Scripts.Gameplay.Managers;
using FPS.Scripts.UI;
using UnityEngine;

public class NotificationHudManagerUpdated : MonoBehaviour
{
    [field: SerializeField] public RectTransform InGameNotificationRect { get; set; }
    [field: SerializeField] public RectTransform InStreamNotificationRect { get; set; }
    [field: SerializeField] public GameObject NotificationPrefab { get; set; }
    
    protected PlayerWeaponsManager PlayerWeaponsManager;
    protected Jetpack Jetpack;

    private void Awake()
    {
        PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();
        Jetpack = FindObjectOfType<Jetpack>();
    }

    private void OnEnable()
    {
        PlayerWeaponsManager.OnAddedWeapon += OnAddedWeapon;
        PlayerWeaponsManager.OnSwitchedToWeapon += OnSwitchedToWeapon;
        Jetpack.OnUnlockJetpack += OnJetpackUnlocked;
        EventManager.AddListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);
    }

    private void OnDisable()
    {
        PlayerWeaponsManager.OnAddedWeapon -= OnAddedWeapon;
        PlayerWeaponsManager.OnSwitchedToWeapon -= OnSwitchedToWeapon;
        Jetpack.OnUnlockJetpack -= OnJetpackUnlocked;
        EventManager.RemoveListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);
    }
    
    private void OnObjectiveUpdateEvent(ObjectiveUpdateEvent evt)
    {
        if (!string.IsNullOrEmpty(evt.NotificationText))
        {
            CreateNotification(evt.NotificationText);
        }
    }

    private void OnJetpackUnlocked(bool unlock)
    {
        CreateNotification("Jetpack unlocked");
    }

    private void OnAddedWeapon(WeaponController weaponController, int index)
    {
        CreateNotification("Picked up weapon: " + weaponController.WeaponName);
    }
    
    private void OnSwitchedToWeapon(WeaponController weaponController)
    {
        CreateNotification("Selected Weapon: " + weaponController.WeaponName);
    }
    
    private void CreateNotification(string text)
    {
        // Instantiate notifications for both in-game and in-stream
        GameObject inGameNotificationInstance = Instantiate(NotificationPrefab, InGameNotificationRect);
        GameObject inStreamNotificationInstance = Instantiate(NotificationPrefab, InStreamNotificationRect);

        // Set sibling index to 0 to display at the top
        inGameNotificationInstance.transform.SetSiblingIndex(0);
        inStreamNotificationInstance.transform.SetSiblingIndex(0);

        NotificationToast inGameToast = inGameNotificationInstance.GetComponent<NotificationToast>();
        NotificationToast inStreamToast = inStreamNotificationInstance.GetComponent<NotificationToast>();

        // Initialize both notifications with the same text
        if (inGameToast)
        {
            inGameToast.Initialize(text);
        }

        if (inStreamToast)
        {
            inStreamToast.Initialize(text);
        }
    }
}
