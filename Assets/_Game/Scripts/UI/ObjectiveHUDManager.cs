using System;
using System.Collections.Generic;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using FPS.Scripts.Game;
using UnityEngine;

namespace DLS.UI
{
    public class ObjectiveHUDManager : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying objectives")]
        public RectTransform ObjectivePanel;

        [Tooltip("Prefab for the primary objectives")]
        public GameObject PrimaryObjectivePrefab;

        [Tooltip("Prefab for the primary objectives")]
        public GameObject SecondaryObjectivePrefab;

        Dictionary<Objective.Objective, ObjectiveToast> m_ObjectivesDictionnary;

        void Awake()
        {
            m_ObjectivesDictionnary = new Dictionary<Objective.Objective, ObjectiveToast>();
        }


        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<ObjectiveMessage>(MessageChannels.Objective, ObjectiveMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<ObjectiveMessage>(MessageChannels.Objective, ObjectiveMessageHandler);
        }

        private void ObjectiveMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<ObjectiveMessage>().HasValue) return;
            var data = message.Message<ObjectiveMessage>().GetValueOrDefault();
            switch (data.Status)
            {
                case ObjectiveStatus.Created:
                    RegisterObjective(data.Objective);
                    break;
                case ObjectiveStatus.Completed:
                    UnregisterObjective(data.Objective);
                    break;
                case ObjectiveStatus.Updated:
                    if (m_ObjectivesDictionnary.TryGetValue(data.Objective, out ObjectiveToast toast) && toast != null)
                    {
                        // set the new updated description for the objective, and forces the content size fitter to be recalculated
                        Canvas.ForceUpdateCanvases();
                        if (!string.IsNullOrEmpty(data.DescriptionText))
                            toast.DescriptionTextContent.text = data.DescriptionText;

                        if (!string.IsNullOrEmpty(data.CounterText))
                            toast.CounterTextContent.text = data.CounterText;

                        if (toast.GetComponent<RectTransform>())
                        {
                            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(toast.GetComponent<RectTransform>());
                        }
                    }
                    break;
            }
        }

        public void RegisterObjective(Objective.Objective objective)
        {
            // instanciate the Ui element for the new objective
            GameObject objectiveUIInstance = Instantiate(objective.IsOptional ? SecondaryObjectivePrefab : PrimaryObjectivePrefab, ObjectivePanel);

            if (!objective.IsOptional)
                objectiveUIInstance.transform.SetSiblingIndex(0);

            ObjectiveToast toast = objectiveUIInstance.GetComponent<ObjectiveToast>();
            DebugUtility.HandleErrorIfNullGetComponent<ObjectiveToast, ObjectiveHUDManager>(toast, this,
                objectiveUIInstance.gameObject);

            // initialize the element and give it the objective description
            toast.Initialize(objective.Title, objective.Description, "", objective.IsOptional, objective.DelayVisible);

            m_ObjectivesDictionnary.Add(objective, toast);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ObjectivePanel);
        }

        public void UnregisterObjective(Objective.Objective objective)
        {
            // if the objective if in the list, make it fade out, and remove it from the list
            if (m_ObjectivesDictionnary.TryGetValue(objective, out ObjectiveToast toast) && toast != null)
            {
                toast.Complete();
            }

            m_ObjectivesDictionnary.Remove(objective);
        }
        
    }
}