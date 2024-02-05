using System;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using DLS.Time;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using UnityEngine;

namespace DLS.Managers
{
    /// <summary>
/// Represents the TimeManager.
/// The TimeManager class provides functionality related to Time Manager management.
/// This class contains methods and properties that assist in managing and processing Time Manager related tasks.
/// </summary>
    public class TimeManager : MonoBehaviour
    {
        /// <summary>
        ///  Singleton instance for the TimeManager.
        /// </summary>
        public static TimeManager Instance { get; private set; }
        
        [field: Tooltip("The current time object.")]
        [field: SerializeField] public virtual GameTimeObject CurrentTimeObject { get; set; }
        
        [field: Tooltip("The count down timer")]
        [field: SerializeField] public virtual GameTimeObject CountDownTimer { get; set; }
        
        [field: SerializeField] public virtual float CountDownMinutes { get; set; } = 10f;
        
        [field: Tooltip("is the game paused?")]
        [field: SerializeField] public virtual bool IsPaused { get; set; }


        /// <summary>
        ///  Singleton pattern for the time manager.
        /// </summary>
        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                //TODO:Replace with message system handler.
                EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
                EventManager.AddListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            }
            else
            {
                Destroy(gameObject);
            }

        }

        private void OnAllObjectivesCompleted(AllObjectivesCompletedEvent evt)
        {
            IsPaused = true;
            Reset();
        }

        private void OnPlayerDeath(PlayerDeathEvent evt)
        {
            IsPaused = true;
            Reset();
        }

        private void Start()
        {
            Reset();
        }

        public virtual void OnEnable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<TimeMessage>(MessageChannels.Time, HandleTimeMessage);
            MessageSystem.MessageManager.UnregisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
        }

        public virtual void OnDisable()
        {
            MessageSystem.MessageManager.RegisterForChannel<TimeMessage>(MessageChannels.Time, HandleTimeMessage);
            MessageSystem.MessageManager.RegisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
        }
            
        /// <summary>
        ///  Updates the current time object if it exists and the game is in the playing state and the game is not paused.
        /// </summary>
        public virtual void Update()
        {
            if(IsPaused) return;
            CurrentTimeObject.StartTime();
            if(CountDownTimer.Minute <= 0 && CountDownTimer.Second <= 0)
                return;
            CountDownTimer.ReverseTime();
        }

        public virtual void Reset()
        {
            if (CurrentTimeObject != null)
            {
                CurrentTimeObject.ResetFullDate();
            }
            
            if (CountDownTimer != null)
            {
                CountDownTimer.ResetFullDate();
                CountDownTimer.Minute = CountDownMinutes;
            }
        }

        /// <summary>
        ///  Resets the current time object if it exists on application quit.
        /// </summary>
        public virtual void OnApplicationQuit()
        {
            Reset();
        }
        
        /// <summary>
        ///  Handles the time message and based on the operation sets, adds, subtracts, multiplies, or divides the time.
        /// </summary>
        /// <param name="message"></param>
        public virtual void HandleTimeMessage(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<TimeMessage>().HasValue) return;
            if(CurrentTimeObject == null) return;
            var data = message.Message<TimeMessage>().GetValueOrDefault();

            switch (data.Operation)
            {
                case TimeOperation.Set:
                    if (data.Year.HasValue) CurrentTimeObject.Year = data.Year.Value;
                    if (data.Month.HasValue) CurrentTimeObject.Month = data.Month.Value;
                    if (data.Week.HasValue) CurrentTimeObject.Week = data.Week.Value;
                    if (data.Day.HasValue) CurrentTimeObject.Day = data.Day.Value;
                    if (data.Hour.HasValue) CurrentTimeObject.Hour = data.Hour.Value;
                    if (data.Minute.HasValue) CurrentTimeObject.Minute = data.Minute.Value;
                    if (data.Second.HasValue) CurrentTimeObject.Second = data.Second.Value;
                    if (data.MilliSecond.HasValue) CurrentTimeObject.MilliSecond = data.MilliSecond.Value;
                    break;

                case TimeOperation.Add:
                    if (data.Year.HasValue) CurrentTimeObject.Year += data.Year.Value;
                    if (data.Month.HasValue) CurrentTimeObject.Month += data.Month.Value;
                    if (data.Week.HasValue) CurrentTimeObject.Week += data.Week.Value;
                    if (data.Day.HasValue) CurrentTimeObject.Day += data.Day.Value;
                    if (data.Hour.HasValue) CurrentTimeObject.Hour += data.Hour.Value;
                    if (data.Minute.HasValue) CurrentTimeObject.Minute += data.Minute.Value;
                    if (data.Second.HasValue) CurrentTimeObject.Second += data.Second.Value;
                    if (data.MilliSecond.HasValue) CurrentTimeObject.MilliSecond += data.MilliSecond.Value;
                    break;

                case TimeOperation.Subtract:
                    if (data.Year.HasValue) CurrentTimeObject.Year -= data.Year.Value;
                    if (data.Month.HasValue) CurrentTimeObject.Month -= data.Month.Value;
                    if (data.Week.HasValue) CurrentTimeObject.Week -= data.Week.Value;
                    if (data.Day.HasValue) CurrentTimeObject.Day -= data.Day.Value;
                    if (data.Hour.HasValue) CurrentTimeObject.Hour -= data.Hour.Value;
                    if (data.Minute.HasValue) CurrentTimeObject.Minute -= data.Minute.Value;
                    if (data.Second.HasValue) CurrentTimeObject.Second -= data.Second.Value;
                    if (data.MilliSecond.HasValue) CurrentTimeObject.MilliSecond -= data.MilliSecond.Value;
                    break;

                case TimeOperation.Multiply:
                    if (data.Year.HasValue) CurrentTimeObject.Year *= data.Year.Value;
                    if (data.Month.HasValue) CurrentTimeObject.Month *= data.Month.Value;
                    if (data.Week.HasValue) CurrentTimeObject.Week *= data.Week.Value;
                    if (data.Day.HasValue) CurrentTimeObject.Day *= data.Day.Value;
                    if (data.Hour.HasValue) CurrentTimeObject.Hour *= data.Hour.Value;
                    if (data.Minute.HasValue) CurrentTimeObject.Minute *= data.Minute.Value;
                    if (data.Second.HasValue) CurrentTimeObject.Second *= data.Second.Value;
                    if (data.MilliSecond.HasValue) CurrentTimeObject.MilliSecond *= data.MilliSecond.Value;
                    break;

                case TimeOperation.Divide:
                    if (data.Year.HasValue && data.Year.Value != 0) CurrentTimeObject.Year /= data.Year.Value;
                    if (data.Month.HasValue && data.Month.Value != 0) CurrentTimeObject.Month /= data.Month.Value;
                    if (data.Week.HasValue && data.Week.Value != 0) CurrentTimeObject.Week /= data.Week.Value;
                    if (data.Day.HasValue && data.Day.Value != 0) CurrentTimeObject.Day /= data.Day.Value;
                    if (data.Hour.HasValue && data.Hour.Value != 0) CurrentTimeObject.Hour /= data.Hour.Value;
                    if (data.Minute.HasValue && data.Minute.Value != 0) CurrentTimeObject.Minute /= data.Minute.Value;
                    if (data.Second.HasValue && data.Second.Value != 0) CurrentTimeObject.Second /= data.Second.Value;
                    if (data.MilliSecond.HasValue && data.MilliSecond.Value != 0) CurrentTimeObject.MilliSecond /= data.MilliSecond.Value;
                    break;
            }
            CurrentTimeObject.ValidateTime();
        }
        
        /// <summary>
        ///  Handles the pause message and sets the is paused property.
        /// </summary>
        /// <param name="message"></param>
        public virtual void PauseMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PauseMessage>().HasValue) return;
            var data = message.Message<PauseMessage>().GetValueOrDefault();
            IsPaused = data.IsPaused;
        }
        
    }
}