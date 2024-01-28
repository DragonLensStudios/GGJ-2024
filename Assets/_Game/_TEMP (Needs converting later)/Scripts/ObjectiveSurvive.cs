using System;
using DLS.Time;
using UnityEngine;

namespace _TEMP__Needs_converting_later_.Scripts
{
    public class ObjectiveSurvive : FPS.Scripts.Game.Shared.Objective
    {
        [field: SerializeField] public virtual GameTimeObject CountDownTimer { get; set; }
        [field: SerializeField] public virtual WaveSpawnerController WaveSpawner { get; set; }

        protected override void Start()
        {
            base.Start();
            
            WaveSpawner = FindObjectOfType<WaveSpawnerController>();
            // set a title and description specific for this type of objective, if it hasn't one
            if (string.IsNullOrEmpty(Title))
                Title = $"Survive For {CountDownTimer.Minute} Minutes";

            if (string.IsNullOrEmpty(Description))
                Description = $"Remaining Time: {CountDownTimer.Minute}";
            
        }

        protected void Update()
        {
            if(IsCompleted)
                return;

            if (CountDownTimer.Minute <= 0 && CountDownTimer.Second <= 0)
            {
                CompleteObjective(string.Empty, String.Empty, "You Survived!");
            }
            else
            {
                UpdateObjective(string.Empty, $"Wave:{WaveSpawner.WaveNumber} Next Wave IN: {WaveSpawner.TimeSinceLastWave:0}", String.Empty);
            }

        }
    }
}