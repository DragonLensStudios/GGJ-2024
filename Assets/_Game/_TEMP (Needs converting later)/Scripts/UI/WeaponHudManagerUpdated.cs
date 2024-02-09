using System.Collections.Generic;
using FPS.Scripts.Gameplay.Managers;
using FPS.Scripts.UI;
using UnityEngine;
using WeaponController = FPS.Scripts.Game.Shared.WeaponController;

public class WeaponHudManagerUpdated : MonoBehaviour
{
    [field: SerializeField] public RectTransform InGameAmmoRect { get; set; }
    [field: SerializeField] public RectTransform InStreamAmmoRect { get; set; }
    [field: SerializeField] public GameObject AmmoPrefab { get; set; }
    
    protected PlayerWeaponsManager PlayerWeaponsManager;
    // Store tuples for ammo counters to manage both in-game and in-stream UIs
    protected List<(AmmoCounter inGameCounter, AmmoCounter inStreamCounter)> AmmoCounters = new List<(AmmoCounter, AmmoCounter)>();
    
    private void Awake()
    {
        PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();
        
        WeaponController activeWeapon = PlayerWeaponsManager.GetActiveWeapon();
        if (activeWeapon != null)
        {
            AddWeapon(activeWeapon, PlayerWeaponsManager.ActiveWeaponIndex);
            ChangeWeapon(activeWeapon);
        }
    }

    private void OnEnable()
    {
        PlayerWeaponsManager.OnAddedWeapon += AddWeapon;
        PlayerWeaponsManager.OnRemovedWeapon += RemoveWeapon;
        PlayerWeaponsManager.OnSwitchedToWeapon += ChangeWeapon;
    }

    private void OnDisable()
    {
        PlayerWeaponsManager.OnAddedWeapon -= AddWeapon;
        PlayerWeaponsManager.OnRemovedWeapon -= RemoveWeapon;
        PlayerWeaponsManager.OnSwitchedToWeapon -= ChangeWeapon;
    }
    
    private void ChangeWeapon(WeaponController weapon)
    {
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(InGameAmmoRect);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(InStreamAmmoRect);
    }

    private void RemoveWeapon(WeaponController weapon, int index)
    {
        // Search for the ammo counter by index and remove it
        var found = AmmoCounters.FindIndex(counter => counter.inGameCounter.WeaponCounterIndex == index);
        if (found != -1)
        {
            Destroy(AmmoCounters[found].inGameCounter.gameObject);
            Destroy(AmmoCounters[found].inStreamCounter.gameObject);
            AmmoCounters.RemoveAt(found);
        }
    }

    private void AddWeapon(WeaponController weapon, int index)
    {
        GameObject inGameAmmoCounterInstance = Instantiate(AmmoPrefab, InGameAmmoRect);
        GameObject inStreamAmmoCounterInstance = Instantiate(AmmoPrefab, InStreamAmmoRect);
        
        AmmoCounter inGameAmmoCounter = inGameAmmoCounterInstance.GetComponent<AmmoCounter>();
        AmmoCounter inStreamAmmoCounter = inStreamAmmoCounterInstance.GetComponent<AmmoCounter>();

        inGameAmmoCounter.Initialize(weapon, index);
        inStreamAmmoCounter.Initialize(weapon, index);

        // Add both counters to the list as a tuple
        AmmoCounters.Add((inGameAmmoCounter, inStreamAmmoCounter));
    }
}
