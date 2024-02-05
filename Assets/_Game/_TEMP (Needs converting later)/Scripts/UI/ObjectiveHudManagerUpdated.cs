using System.Collections.Generic;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using FPS.Scripts.UI;
using UnityEngine;

public class ObjectiveHudManagerUpdated : MonoBehaviour
{
    [field: SerializeField] RectTransform InGameObjectiveRect { get; set; }
    [field: SerializeField] RectTransform InStreamObjectiveRect { get; set; }
    [field: SerializeField] GameObject PrimaryObjectivePrefab { get; set; }
    [field: SerializeField] GameObject SecondaryObjectivePrefab { get; set; }
    
    // Updated to hold a tuple of ObjectiveToast for in-game and in-stream
    protected Dictionary<FPS.Scripts.Game.Shared.Objective, (ObjectiveToast inGameToast, ObjectiveToast inStreamToast)> ObjectivesDictionary;

    private void Awake()
    {
        ObjectivesDictionary = new Dictionary<FPS.Scripts.Game.Shared.Objective, (ObjectiveToast, ObjectiveToast)>();
    }

    private void OnEnable()
    {
        EventManager.AddListener<ObjectiveUpdateEvent>(OnUpdateObjective);
        FPS.Scripts.Game.Shared.Objective.OnObjectiveCreated += RegisterObjective;
        FPS.Scripts.Game.Shared.Objective.OnObjectiveCompleted += UnregisterObjective;
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<ObjectiveUpdateEvent>(OnUpdateObjective);
        FPS.Scripts.Game.Shared.Objective.OnObjectiveCreated -= RegisterObjective;
        FPS.Scripts.Game.Shared.Objective.OnObjectiveCompleted -= UnregisterObjective;
    }
    
    private void OnUpdateObjective(ObjectiveUpdateEvent evt)
    {
        if (ObjectivesDictionary.TryGetValue(evt.Objective, out var toastPair))
        {
            UpdateToast(toastPair.inGameToast, evt);
            UpdateToast(toastPair.inStreamToast, evt);
        }
    }

    private void UpdateToast(ObjectiveToast toast, ObjectiveUpdateEvent evt)
    {
        if (toast != null)
        {
            if (!string.IsNullOrEmpty(evt.DescriptionText))
                toast.DescriptionTextContent.text = evt.DescriptionText;

            if (!string.IsNullOrEmpty(evt.CounterText))
                toast.CounterTextContent.text = evt.CounterText;

            if (toast.GetComponent<RectTransform>())
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(toast.GetComponent<RectTransform>());
            }
        }
    }
    
    private void RegisterObjective(FPS.Scripts.Game.Shared.Objective objective)
    {
        GameObject inGameObjectiveUIInstance = Instantiate(objective.IsOptional ? SecondaryObjectivePrefab : PrimaryObjectivePrefab, InGameObjectiveRect);
        GameObject inStreamObjectiveUIInstance = Instantiate(objective.IsOptional ? SecondaryObjectivePrefab : PrimaryObjectivePrefab, InStreamObjectiveRect);

        ObjectiveToast inGameToast = inGameObjectiveUIInstance.GetComponent<ObjectiveToast>();
        ObjectiveToast inStreamToast = inStreamObjectiveUIInstance.GetComponent<ObjectiveToast>();

        // Initialize both toasts
        inGameToast.Initialize(objective.Title, objective.Description, "", objective.IsOptional, objective.DelayVisible);
        inStreamToast.Initialize(objective.Title, objective.Description, "", objective.IsOptional, objective.DelayVisible);

        ObjectivesDictionary.Add(objective, (inGameToast, inStreamToast));

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(InGameObjectiveRect);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(InStreamObjectiveRect);
    }
    
    private void UnregisterObjective(FPS.Scripts.Game.Shared.Objective objective)
    {
        if (ObjectivesDictionary.TryGetValue(objective, out var toastPair))
        {
            toastPair.inGameToast.Complete();
            toastPair.inStreamToast.Complete();
        }

        ObjectivesDictionary.Remove(objective);
    }
}
