using System;
using System.Collections.Generic;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using FPS.Scripts.Game.Shared;
using UnityEngine;

namespace FPS.Scripts.UI
{
    public class ObjectiveHUDManager : MonoBehaviour
    {
        [Tooltip("In Game UI panel containing the layoutGroup for displaying objectives")]
        public RectTransform InGameObjectivePanel;
        
        [Tooltip("Prefab for the primary objectives")]
        public GameObject PrimaryObjectivePrefab;

        [Tooltip("Prefab for the primary objectives")]
        public GameObject SecondaryObjectivePrefab;

        Dictionary<Objective, ObjectiveToast> m_ObjectivesDictionnary;

        void Awake()
        {
            m_ObjectivesDictionnary = new Dictionary<Objective, ObjectiveToast>();

            EventManager.AddListener<ObjectiveUpdateEvent>(OnUpdateObjective);

            Objective.OnObjectiveCreated += RegisterObjective;
            Objective.OnObjectiveCompleted += UnregisterObjective;
        }

        public void RegisterObjective(Objective objective)
        {
            // instanciate the Ui element for the new objective
            GameObject inGameObjectiveUIInstance = Instantiate(objective.IsOptional ? SecondaryObjectivePrefab : PrimaryObjectivePrefab, InGameObjectivePanel);

            if (!objective.IsOptional)
            {
                inGameObjectiveUIInstance.transform.SetSiblingIndex(0);
            }

            ObjectiveToast inGameToast = inGameObjectiveUIInstance.GetComponent<ObjectiveToast>();
            DebugUtility.HandleErrorIfNullGetComponent<ObjectiveToast, ObjectiveHUDManager>(inGameToast, this, inGameObjectiveUIInstance.gameObject);

            // initialize the element and give it the objective description
            inGameToast.Initialize(objective.Title, objective.Description, "", objective.IsOptional, objective.DelayVisible);

            m_ObjectivesDictionnary.Add(objective, inGameToast);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(InGameObjectivePanel);
        }

        public void UnregisterObjective(Objective objective)
        {
            // if the objective if in the list, make it fade out, and remove it from the list
            if (m_ObjectivesDictionnary.TryGetValue(objective, out ObjectiveToast toast) && toast != null)
            {
                toast.Complete();
            }

            m_ObjectivesDictionnary.Remove(objective);
        }

        void OnUpdateObjective(ObjectiveUpdateEvent evt)
        {
            if (m_ObjectivesDictionnary.TryGetValue(evt.Objective, out ObjectiveToast toast) && toast != null)
            {
                // set the new updated description for the objective, and forces the content size fitter to be recalculated
                Canvas.ForceUpdateCanvases();
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

        void OnDestroy()
        {
            EventManager.AddListener<ObjectiveUpdateEvent>(OnUpdateObjective);

            Objective.OnObjectiveCreated -= RegisterObjective;
            Objective.OnObjectiveCompleted -= UnregisterObjective;
        }
    }
}