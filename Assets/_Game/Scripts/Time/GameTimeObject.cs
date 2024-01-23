using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace Time
{
    /// <summary>
    /// This is the Time class, This class holds time variables and methods used for custom timers and custom clocks.
    /// To use this class simply add it as a variable or a property to utilize each of the methods and types.
    /// </summary>
    [CreateAssetMenu(fileName ="GameTime", menuName ="Game/Time/GameTime", order = 3)]
    public class GameTimeObject : ScriptableObject
    {
        #region Public Virtual Properties
        
        [field: Tooltip("The name of the GameTimeObject")]
        [field: SerializeField] public virtual string Name { get; set; }
        
        [field: Tooltip("The Months of the GameTimeObject")]
        [field: SerializeField] public virtual List<MonthObject> Months { get; set; } = new();
        
        [field: Tooltip("The Days of the GameTimeObject")]
        [field: SerializeField] public virtual List<DayObject> Days { get; set; } = new();
        
        [field: Tooltip("The CurrentMonth of the GameTimeObject")]
        [field: SerializeField] public virtual MonthObject CurrentMonth { get; set; }
        
        [field: Tooltip("The CurrentDay of the GameTimeObject")]
        [field: SerializeField] public virtual DayObject CurrentDay { get; set; }
        
        [field: Tooltip("The Year of the GameTimeObject")]
        [field: SerializeField] public virtual float Year { get; set; }
        
        [field: Tooltip("The Month of the GameTimeObject")]
        [field: SerializeField] public virtual float Month { get; set; } = 1;
        
        [field: Tooltip("The Week of the GameTimeObject")]
        [field: SerializeField] public virtual float Week { get; set; } = 1;
        
        [field: Tooltip("The Day of the GameTimeObject")]
        [field: SerializeField] public virtual float Day { get; set; } = 1;
        
        [field: Tooltip("The Hour of the GameTimeObject")]
        [field: SerializeField] public virtual float Hour { get; set; }
        
        [field: Tooltip("The Minute of the GameTimeObject")]
        [field: SerializeField] public virtual float Minute { get; set; }
        
        [field: Tooltip("The Second of the GameTimeObject")]
        [field: SerializeField] public virtual float Second { get; set; }
        
        [field: Tooltip("The MilliSecond of the GameTimeObject")]
        [field: SerializeField] public virtual float MilliSecond { get; set; }
        
        [field: Tooltip("The TimeScale of the GameTimeObject")]
        [field: SerializeField] public virtual float TimeScale { get; set; } = 1;
        
        [field: Tooltip("The MilliSecondsInSecond of the GameTimeObject")]
        [field: SerializeField] public virtual int MilliSecondsInSecond { get; set; } = 1000;
        
        [field: Tooltip("The SecondsInMinute of the GameTimeObject")]
        [field: SerializeField] public virtual int SecondsInMinute { get; set; } = 60;
        
        [field: Tooltip("The MinutesInHour of the GameTimeObject")]
        [field: SerializeField] public virtual int MinutesInHour { get; set; } = 60;
        
        #endregion
        
        #region Protected Virtual Properties

        /// <summary>
        ///  The last known week of the GameTimeObject
        /// </summary>
        protected virtual int LastKnownWeek { get; set; } = 1;
        
        #endregion

        #region Public Virtual Methods

        /// <summary>
        ///  Registers for the LevelResetMessage and Application quitting event.
        /// </summary>
        public virtual void OnEnable()
        {
          Application.quitting += ApplicationOnquitting;
        }
        
        /// <summary>
        ///  Unregisters for the LevelResetMessage and Application quitting event.
        /// </summary>
        public virtual void OnDisable()
        {
            Application.quitting -= ApplicationOnquitting;
        }
        
        /// <summary>
        ///  Resets the full date of the GameTimeObject when application quits.
        /// </summary>
        public virtual void ApplicationOnquitting()
        {
            ResetFullDate();
        }
        
        /// <summary>
        ///  Gets the months and days loading them from the resources folder.
        /// </summary>
        public virtual void GetMonthsAndDays()
        {
            Months = new List<MonthObject>(Resources.LoadAll<MonthObject>("Time/Months"));
            Days = new List<DayObject>(Resources.LoadAll<DayObject>("Time/Days"));

            if (Months.Count <= 0 || Days.Count <= 0) return;
            // Sort days and months based on their values
            Days.Sort((a, b) => a.Value.CompareTo(b.Value));
            Months.Sort((a, b) => a.Value.CompareTo(b.Value));

            if (CurrentMonth == null && Months.Count > 0)
            {
                CurrentMonth = Months.First();
            }

            if (CurrentDay == null && Days.Count > 0)
            {
                CurrentDay = GetDayOfWeek(1, 1, (int)Year);
            }
        }
        
        /// <summary>
        ///  Gets the total time in seconds.
        /// </summary>
        public virtual long TotalTimeSeconds
        {
            get
            {
                long totalSeconds = 0;

                // Years
                for (int i = 1; i < Year; i++)
                {
                    for (var index = 0; index < Months.Count; index++)
                    {
                        var month = Months[index];
                        totalSeconds += month.GetDays(index) * CurrentDay.HoursInDay * MinutesInHour * SecondsInMinute;
                    }
                }

                // Months
                for (int i = 1; i < Month; i++)
                {
                    totalSeconds += Months[i-1].GetDays((int)Year) * CurrentDay.HoursInDay * MinutesInHour * SecondsInMinute;
                }

                // Weeks (assuming every week is 7 days for now)
                totalSeconds += (int)Week * Days.Count * CurrentDay.HoursInDay * MinutesInHour * SecondsInMinute;

                // Days
                totalSeconds += (int)Day * CurrentDay.HoursInDay * MinutesInHour * SecondsInMinute;

                // Hours, Minutes, Seconds, and MilliSeconds
                totalSeconds += (int)Hour * MinutesInHour * SecondsInMinute;
                totalSeconds += (int)Minute * SecondsInMinute;
                totalSeconds += (int)Second;
                totalSeconds += (int)MilliSecond / MilliSecondsInSecond;

                return totalSeconds;
            }
        }
        
        /// <summary>
        /// This sets the Seconds to Time.deltaTime * TimeScale. This effectively will set the proper selected value when called in update.
        /// this method also calls ValidateTime. <see cref="ValidateTime"/>
        /// (Must be called in Update for the Time to be properly set.)
        /// </summary>
        public virtual void StartTime(TimeType timeType = TimeType.MilliSecond)
        {
            switch (timeType)
            {
                case TimeType.MilliSecond:
                    MilliSecond += UnityEngine.Time.deltaTime * TimeScale * MilliSecondsInSecond;
                    break;
                case TimeType.Second:
                    Second += UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Minute:
                    Minute += UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Hour:
                    Hour += UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Day:
                    Day += UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Week:
                    Week += UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Month:
                    Month += UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Year:
                    Year += UnityEngine.Time.deltaTime * TimeScale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeType), timeType, null);
            }
            ValidateTime();
        }

        /// <summary>
        /// This sets the Seconds to Time.deltaTime * TimeScale. This effectively will set the proper selected value in reverse when called in update.
        /// this method also calls ValidateTime. <see cref="ValidateTime"/>
        /// (Must be called in Update for the Time to be properly set.)
        /// For example if you had a had a timer that had 5 minutes it would start going down by the value provided.
        /// </summary>
        public virtual void ReverseTime(TimeType timeType = TimeType.MilliSecond)
        {
            switch (timeType)
            {
                case TimeType.MilliSecond:
                    MilliSecond -= UnityEngine.Time.deltaTime * TimeScale * MilliSecondsInSecond;
                    break;
                case TimeType.Second:
                    Second -= UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Minute:
                    Minute -= UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Hour:
                    Hour -= UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Day:
                    Day -= UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Week:
                    Week -= UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Month:
                    Month -= UnityEngine.Time.deltaTime * TimeScale;
                    break;
                case TimeType.Year:
                    Year -= UnityEngine.Time.deltaTime * TimeScale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeType), timeType, null);
            }
            ValidateTime();
        }

        /// <summary>
        /// This method checks the time counting to make sure that after 60 seconds 1 minute is added (by default) and 60 Minutes adds 1 hour, etc...
        /// </summary>
        public virtual void ValidateTime()
        {
            // Millisecond
            while (MilliSecond >= MilliSecondsInSecond)
            {
                Second++;
                MilliSecond -= MilliSecondsInSecond;
            }
            while (MilliSecond < 0 && Second > 0)
            {
                Second--;
                MilliSecond += MilliSecondsInSecond;
            }

            // Second
            while (Second >= SecondsInMinute)
            {
                Minute++;
                Second -= SecondsInMinute;
            }
            while (Second < 0 && Minute > 0)
            {
                Minute--;
                Second += SecondsInMinute;
            }

            // Minute
            while (Minute >= MinutesInHour)
            {
                Hour++;
                Minute -= MinutesInHour;
            }
            while (Minute < 0 && Hour > 0)
            {
                Hour--;
                Minute += MinutesInHour;
            }

            // Hour
            while (Hour >= CurrentDay.HoursInDay)
            {
                Day++;
                Hour -= CurrentDay.HoursInDay;
            }
            while (Hour < 0 && Day > 0)
            {
                Day--;
                Hour += CurrentDay.HoursInDay;
            }

            // Day
            while (Day > CurrentMonth.GetDays((int)Year))
            {
                Month++;
                Day -= CurrentMonth.GetDays((int)Year);
            }
            while (Day <= 0 && Month > 0)
            {
                Month--;
                if (Month > 0)
                {
                    Day += Months[(int)Month - 1].GetDays((int)Year);
                }
                else
                {
                    Day += Months[^1].GetDays((int)Year);
                }
            }

            // Month
            while (Month > Months.Count)
            {
                Year++;
                Month -= Months.Count;
            }
            while (Month <= 0 && Year > 0)
            {
                Year--;
                Month += Months.Count;
            }

            // Calculate first day of the week for the current month
            var firstDayOfWeekInMonth = GetDayOfWeek(1, (int)Month, (int)Year).Value;

// Adjust the week if manually changed
            if ((int)Week != LastKnownWeek) 
            {
                if (Week <= 0)
                {
                    if (--Month == 0)
                    {
                        Year--;
                        Month = Months.Count;
                    }
                    CurrentMonth = Months[(int)Month - 1];
                    Week = (int)Math.Ceiling((double)CurrentMonth.GetDays((int)Year) / CurrentMonth.DaysInWeek);
                    Day = CurrentMonth.GetDays((int)Year);
                }
                else if ((int)Week > LastKnownWeek)  // Incremented the week
                {
                    Day = ((int)Week - 1) * CurrentMonth.DaysInWeek + 1 - (firstDayOfWeekInMonth - 1);
                    while (Day > CurrentMonth.GetDays((int)Year))
                    {
                        Day -= CurrentMonth.DaysInWeek;
                        Month++;
                        if (Month > Months.Count)
                        {
                            Month = 1;
                            Year++;
                        }
                        CurrentMonth = Months[(int)Month - 1];
                        firstDayOfWeekInMonth = GetDayOfWeek(1, (int)Month, (int)Year).Value;
                    }
                }
                else  // Decremented the week
                {
                    Day = (int)Week * CurrentMonth.DaysInWeek - (CurrentMonth.DaysInWeek - 1) - (firstDayOfWeekInMonth - 1);
                    while (Day <= 0)
                    {
                        Month--;
                        CurrentMonth = Month > 0 ? Months[(int)Month - 1] : Months[^1];
                        firstDayOfWeekInMonth = GetDayOfWeek(1, (int)Month, (int)Year).Value;
                        Day += CurrentMonth.DaysInWeek;
                    }
                }
            }


// Calculate the week based on the adjusted Day and Month
            var adjustedDay = (int)Day + firstDayOfWeekInMonth - 1;
            Week = (int)Math.Ceiling((double)adjustedDay / CurrentMonth.DaysInWeek);
            LastKnownWeek = (int)Week;


            // Set the current day and month
            CurrentMonth = Months[(int)Month - 1];
            CurrentDay = GetDayOfWeek((int)Day, (int)Month, (int)Year);
        }

        /// <summary>
        /// Gets the day of the week for a given date using the Zeller Congruence formula.
        /// the Zeller Congruence is an algorithm devised by Christian Zeller to calculate 
        /// the day of the week for any date. It can be used to determine the day of 
        /// the week for any date from the year 1583 onward, provided that one uses 
        /// the Gregorian calendar.
        /// </summary>
        /// <param name="day">The day of the month (1-31).</param>
        /// <param name="month">The month (1=January, 12=December).</param>
        /// <param name="year">The year (e.g., 2023).</param>
        /// <returns>The corresponding DayObject representing the day of the week.</returns>
        public virtual DayObject GetDayOfWeek(int day, int month, int year)
        {
            // If the month is January or February, adjust it by adding 12 
            // and decrement the year. This is because the Zeller formula 
            // considers January and February as months 13 and 14 of the previous year.
            if (month < 3)
            {
                month += 12;
                year -= 1;
            }

            // centuryYear is the year of the century (year % 100). For example, for 1995, K would be 95.
            var centuryYear = year % 100;

            // centuryYearZeroBased is the zero-based century. For example, for the year 1995, J would be 19.
            var centuryYearZeroBased = year / 100;

            // Compute the formula to get a value 'f' which is then used to determine 
            // the day of the week. The value of 'f' corresponds to the day of the week 
            // (0 = Saturday, 1 = Sunday, 2 = Monday, ..., 6 = Friday).
            var calc = day + ((13 * (month + 1)) / 5) + centuryYear + (centuryYear / 4) + (centuryYearZeroBased / 4) - (2 * centuryYearZeroBased);

            // Take modulo with the number of days in a week to get the result 
            // in the range [0, DaysInWeek - 1].
            var result = calc % CurrentMonth.DaysInWeek;

            // Adjust the result to align with our custom day mapping.
            if (result == 0)
                // If the Zeller formula returns 0 (Saturday), 
                // map it to the last day in your Days list.
                result = CurrentMonth.DaysInWeek - 1; 
            else
                // Otherwise, decrement the result by 1 to adjust the 
                // starting point (the Zeller Sunday is 1 whereas ours is 0).
                result -= 1; 

            return Days[result];
        }

        
        /// <summary>
        /// This will check the time provided and return true or false based on the check. This has optional parameters that can be called like this
        /// EXAMPLE: CheckFullTIme(day:5,minute:30); - This will check to make sure Day 5 and 30 Minutes is true and execute.
        /// Passing a non null value on a parameter will always evaluate the selected check to true for the check call.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public virtual bool CheckTime(int? year = null, int? month = null, int? week = null, int? day = null, int? hour = null, int? minute = null, int? second = null, int? milliSecond = null)
        {
            var yearChecked = true;
            var monthChecked = true;
            var weekChecked = true;
            var dayChecked = true;
            var hourChecked = true;
            var minuteChecked = true;
            var secondChecked = true;
            var milliSecondChecked = true;
            
            if (year.HasValue)
            {
                yearChecked = (int)Year == year.Value;
            }
            if (month.HasValue)
            {
                monthChecked = (int)Month == month.Value;
            }
            if (week.HasValue)
            {
                weekChecked = (int)Week == week.Value;
            }
            if (day.HasValue)
            {
                dayChecked = (int)Day == day.Value;
            }
            if (hour.HasValue)
            {
                hourChecked = (int)Hour == hour.Value;
            }
            if (minute.HasValue)
            {
                minuteChecked = (int)Minute == minute.Value;
            }
            if (second.HasValue)
            {
                secondChecked = (int)Second == second.Value;
            }
            if (milliSecond.HasValue)
            {
                milliSecondChecked = (int)MilliSecond == milliSecond.Value;
            }

            return yearChecked && monthChecked && weekChecked && dayChecked && hourChecked && minuteChecked && secondChecked && milliSecondChecked;
        }

        /// <summary>
        /// Sets the time to the optional times provided. just provide a value that is not null in order to set the time for that slot.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public void SetTime(int? year = null, int? month = null, int? week = null, int? day = null, int? hour = null, int? minute = null, int? second = null, int? milliSecond = null)
        {
            if (year.HasValue)
            {
                Year = year.Value;
            }
            if (month.HasValue)
            {
                Month = month.Value;
            }
            if (week.HasValue)
            {
                Week = week.Value;
            }
            if (day.HasValue)
            {
                Day = day.Value;
            }
            if (hour.HasValue)
            {
                Hour = hour.Value;
            }
            if (minute.HasValue)
            {
                Minute = minute.Value;
            }
            if (second.HasValue)
            {
                Second = second.Value;
            }
            if (milliSecond.HasValue)
            {
                MilliSecond = milliSecond.Value;
            }
            ValidateTime();
        }

        /// <summary>
        /// Adds the time to the optional times provided. just provide a value that is not null in order to add the time for that slot.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public void AddTime(int? year = null, int? month = null, int? week = null, int? day = null, int? hour = null, int? minute = null, int? second = null, int? milliSecond = null)
        {
            if (year.HasValue)
            {
                Year += year.Value;
            }
            if (month.HasValue)
            {
                Month += month.Value;
            }
            if (week.HasValue)
            {
                Week += week.Value;
            }
            if (day.HasValue)
            {
                Day += day.Value;
            }
            if (hour.HasValue)
            {
                Hour += hour.Value;
            }
            if (minute.HasValue)
            {
                Minute += minute.Value;
            }
            if (second.HasValue)
            {
                Second += second.Value;
            } 
            if (milliSecond.HasValue)
            {
                MilliSecond += milliSecond.Value;
            }
            ValidateTime();
        }

        /// <summary>
        /// Subtracts the time to the optional times provided. just provide a value that is not null in order to remove the time for that slot.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public void SubtractTime(int? year = null, int? month = null, int? week = null, int? day = null, int? hour = null, int? minute = null, int? second = null, int? milliSecond = null)
        {
            if (year.HasValue)
            {
                Year -= year.Value;
            }
            if (month.HasValue)
            {
                Month -= month.Value;
            }
            if (week.HasValue)
            {
                Week -= week.Value;
            }
            if (day.HasValue)
            {
                Day -= day.Value;
            }
            if (hour.HasValue)
            {
                Hour -= hour.Value;
            }
            if (minute.HasValue)
            {
                Minute -= minute.Value;
            }
            if (second.HasValue)
            {
                Second -= second.Value;
            } 
            if (milliSecond.HasValue)
            {
                MilliSecond -= milliSecond.Value;
            }
            ValidateTime();
        }

        /// <summary>
        /// Multiplies the time to the optional times provided. just provide a value that is not null in order to multiply the time for that slot.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public void MultiplyTime(int? year = null, int? month = null, int? week = null, int? day = null, int? hour = null, int? minute = null, int? second = null, int? milliSecond = null)
        {
            if (year.HasValue)
            {
                Year *= year.Value;
            }
            if (month.HasValue)
            {
                Month *= month.Value;
            }
            if (week.HasValue)
            {
                Week *= week.Value;
            }
            if (day.HasValue)
            {
                Day *= day.Value;
            }
            if (hour.HasValue)
            {
                Hour *= hour.Value;
            }
            if (minute.HasValue)
            {
                Minute *= minute.Value;
            }
            if (second.HasValue)
            {
                Second *= second.Value;
            } 
            if (milliSecond.HasValue)
            {
                MilliSecond *= milliSecond.Value;
            }
            ValidateTime();
        }

        /// <summary>
        /// Divides the time to the optional times provided. just provide a value that is not null in order to divide the time for that slot.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public void DivideTime(int? year = null, int? month = null, int? week = null, int? day = null, int? hour = null, int? minute = null, int? second = null, int? milliSecond = null)
        {
            if (year.HasValue && year.Value != 0)
            {
                Year /= year.Value;
            }
            if (month.HasValue && month.Value != 0)
            {
                Month /= month.Value;
            }
            if (week.HasValue && week.Value != 0)
            {
                Week /= week.Value;
            }
            if (day.HasValue && day.Value != 0)
            {
                Day /= day.Value;
            }
            if (hour.HasValue && hour.Value != 0)
            {
                Hour /= hour.Value;
            }
            if (minute.HasValue && minute.Value != 0)
            {
                Minute /= minute.Value;
            }
            if (second.HasValue && second.Value != 0)
            {
                Second /= second.Value;
            }
            if (milliSecond.HasValue && milliSecond.Value != 0)
            {
                Second /= milliSecond.Value;
            }
            ValidateTime();
        }
        /// <summary>
        /// Sets the time to the provided GameTime to the optional times provided. just provide a value that is not null in order to set the time for that slot.
        /// </summary>
        /// <param name="gameTimeObject"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public void SetTimeToGameTime(GameTimeObject gameTimeObject, bool? year = null, bool? month = null, bool? week = null, bool? day = null, bool? hour = null, bool? minute = null, bool? second = null, bool? milliSecond = null)
        {
            if (gameTimeObject == null) return;
            if (year.HasValue && year.Value)
            {
                Year = gameTimeObject.Year;
            }
            if (month.HasValue && month.Value)
            {
                Month = gameTimeObject.Month;
            }
            if (week.HasValue && week.Value)
            {
                Week = gameTimeObject.Week;
            }
            if (day.HasValue && day.Value)
            {
                Day = gameTimeObject.Day;
            }
            if (hour.HasValue && hour.Value)
            {
                Hour = gameTimeObject.Hour;
            }
            if (minute.HasValue && minute.Value)
            {
                Minute = gameTimeObject.Minute;
            }
            if (second.HasValue && second.Value)
            {
                Second = gameTimeObject.Second;
            }
            if (milliSecond.HasValue && milliSecond.Value)
            {
                MilliSecond = gameTimeObject.MilliSecond;
            }
            ValidateTime();
        }

        /// <summary>
        /// <para>Checks the Exact MilliSecond.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="milliSecond"></param>

        public bool CheckExactTime(int milliSecond)
        {
            return (int)MilliSecond == milliSecond;
        }
        
        /// <summary>
        /// <para>Checks the Exact Second.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>

        public bool CheckExactTime(int second, int milliSecond)
        {
            return (int)Second == second && (int)MilliSecond == milliSecond;
        }

        /// <summary>
        /// <para>Checks the Exact Minute, Second, MilliSecond</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public bool CheckExactTime(int minute, int second, int milliSecond)
        {
            return (int)Minute == minute && (int)Second == second && (int)MilliSecond == milliSecond;
        }

        /// <summary>
        /// <para>Checks the Exact Hour, Minute, Second, MilliSecond.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public bool CheckExactTime(int hour, int minute, int second, int milliSecond)
        {
            return (int)Hour == hour && (int)Minute == minute && (int)Second == second && (int)MilliSecond == milliSecond;
        }

        /// <summary>
        /// <para>Checks the Exact Day, Hour, Minute, Second, MilliSecond.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public bool CheckExactTime(int day, int hour, int minute, int second, int milliSecond)
        {
            return (int)Day == day && (int)Hour == hour && (int)Minute == minute && (int)Second == second && (int)MilliSecond == milliSecond;
        }

        /// <summary>
        /// <para>Checks the Exact Week, Day, Hour, Minute, Second, MilliSecond.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public bool CheckExactTime(int week, int day, int hour, int minute, int second, int milliSecond)
        {
            return (int)Week == week && (int)Day == day && (int)Hour == hour 
                   && (int)Minute == minute && (int)Second == second && (int)MilliSecond == milliSecond;
        }

        /// <summary>
        /// <para>Checks the Exact Month, Week, Day, Hour, Minute, Second, MilliSecond.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public bool CheckExactTime(int month, int week, int day, int hour, int minute, int second, int milliSecond)
        {
            return (int)Month == month && (int)Week == week && (int)Day == day && (int)Hour == hour
                   && (int)Minute == minute && (int)Second == second && (int)MilliSecond == milliSecond;
        }

        /// <summary>
        /// <para>Checks the Exact Year, Month, Week, Day, Hour, Minute, Second, MilliSecond.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public bool CheckExactTime(int year, int month, int week, int day, int hour, int minute, int second, int milliSecond)
        {
            return (int)Year == year && (int)Month == month && (int)Week == week && (int)Day == day && (int)Hour == hour
                   && (int)Minute == minute && (int)Second == second && (int)MilliSecond == milliSecond;
        }

        /// <summary>
        /// <para>Checks the time between two provided <see cref="GameTimeObject"/> </para>
        /// Returns True if the time current time is within the specified two times and selected time checks. Otherwise returns False.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <param name="checkYear"></param>
        /// <param name="checkMonth"></param>
        /// <param name="checkWeek"></param>
        /// <param name="checkDay"></param>
        /// <param name="checkHour"></param>
        /// <param name="checkMinute"></param>
        /// <param name="checkSecond"></param>
        /// <param name="checkMilliSecond"></param>
        public bool CheckTimeBetweenGameTimes(GameTimeObject time1, GameTimeObject time2, bool checkYear = false, bool checkMonth = false, bool checkWeek = false, bool checkDay = false, bool checkHour = false, bool checkMinute = false, bool checkSecond = false, bool checkMilliSecond = false)
        {
            if (time1 == null || time2 == null) { return false; }
            bool isBetween = false;
            if (checkYear) { isBetween = (int)Year >= (int)time1.Year && (int)Year <= (int)time2.Year; }
            if (checkMonth) { isBetween = (int)Month >= (int)time1.Month && (int)Month <= (int)time2.Month; }
            if (checkWeek) { isBetween = (int)Week >= (int)time1.Week && (int)Week <= (int)time2.Week; }
            if (checkDay) { isBetween = (int)Day >= (int)time1.Day && (int)Day <= (int)time2.Day; }
            if (checkHour) { isBetween = (int)Hour >= (int)time1.Hour && (int)Hour <= (int)time2.Hour; }
            if (checkMinute) { isBetween = (int)Minute >= (int)time1.Minute && (int)Minute <= (int)time2.Minute; }
            if (checkSecond) { isBetween = (int)Second >= (int)time1.Second && (int)Second <= (int)time2.Second; }
            if (checkMilliSecond) { isBetween = (int)MilliSecond >= (int)time1.MilliSecond && (int)MilliSecond <= (int)time2.MilliSecond; }

            return isBetween;
        }

        /// <summary>
        /// Resets the time to the optional values use true for each value you want to reset.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="week"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliSecond"></param>
        public virtual void ResetSpecificTime(bool year = false, bool month = false, bool week = false, bool day = false, bool hour = false, bool minute = false, bool second = false)
        {
            if (year)
            {
                ResetCurrentYear();
            }
            if (month)
            {
                ResetCurrentMonth();
            }
            if (week)
            {
                ResetCurrentWeek();
            }
            if (day)
            {
                ResetCurrentDay();
            }
            if (hour)
            {
                ResetCurrentHour();
            }
            if (minute)
            {
                ResetCurrentMinute();
            }
            if (second)
            {
                ResetCurrentSecond();
            }
            ValidateTime();
        }
        
        /// <summary>
        /// Resets the Days, Hours, Minutes, Seconds and MilliSeconds within a month and the week.
        /// </summary>
        public virtual void ResetCurrentYear()
        {
            ResetWeeks();
        }
        
        /// <summary>
        /// Resets the Days, Hours, Minutes, Seconds and MilliSeconds within a month and the week.
        /// </summary>
        public virtual void ResetCurrentMonth()
        {
           ResetWeeks();
        }

        /// <summary>
        /// Resets the day to the day of the week first.
        /// </summary>
        public virtual void ResetCurrentWeek()
        {
            // Calculate the difference between the current day of the week and the first day of the week
            var daysDifference = GetDayOfWeek((int)Day, (int)Month, (int)Year).Value - 1;

            // Subtract the difference to reset to the start of the week
            Day -= daysDifference;

            // Set the current day as the first day of the week
            CurrentDay = GetDayOfWeek((int)Day, (int)Month, (int)Year);

            // Reset hours, minutes, seconds, etc. to the start of the day
            ResetHours();
        }

        /// <summary>
        /// Resets the Hours, Minutes, Seconds and MilliSeconds within a day.
        /// </summary>
        public virtual void ResetCurrentDay()
        {
           ResetHours();
        }

        /// <summary>
        /// Resets the Minutes and seconds within an hour.
        /// </summary>
        public virtual void ResetCurrentHour()
        {
            ResetMinutes();
        }

        /// <summary>
        /// Resets the Seconds within a minute.
        /// </summary>
        public virtual void ResetCurrentMinute()
        {
            ResetSeconds();
        }
        
        /// <summary>
        /// Resets the MilliSeconds within a second.
        /// </summary>
        public virtual void ResetCurrentSecond()
        {
            ResetMilliSeconds();
        }

        /// <summary>
        /// Resets the Years, Months, Days, Hours, Minutes, Seconds and MilliSeconds.
        /// </summary>
        public virtual void ResetYears()
        {
            Year = 1;
            ResetMonths();
            ResetWeeks();
            ResetDays();
            ResetHours();
            ResetMinutes();
            ResetSeconds();
            ResetMilliSeconds();
            ValidateTime();
        }
        
        /// <summary>
        /// Resets the Months, Days, Hours, Minutes, Seconds and MilliSeconds.
        /// </summary>
        public virtual void ResetMonths()
        {
            Month = 1;
            if (Months.Count > 0)
            {
                CurrentMonth = Months.First();
            }
            ResetWeeks();
            ResetDays();
            ResetHours();
            ResetMinutes();
            ResetSeconds();
            ResetMilliSeconds();
            ValidateTime();
        }

        /// <summary>
        /// Resets the day to the day of the week first.
        /// </summary>
        public virtual void ResetWeeks()
        {
            Week = 1;
            Day = (int)Day % CurrentMonth.DaysInWeek;
            CurrentDay = GetDayOfWeek((int)Day, (int)Month, (int)Year);
            LastKnownWeek = 1;
            ResetDays();
            ResetHours();
            ResetMinutes();
            ResetSeconds();
            ResetMilliSeconds();
            ValidateTime();
        }

        /// <summary>
        /// Resets the Days, Hours, Minutes, Seconds and MilliSeconds.
        /// </summary>
        public virtual void ResetDays()
        {
            Day = 1;
            CurrentDay = GetDayOfWeek((int)Day, (int)Month, (int)Year);
            ResetHours();
            ResetMinutes();
            ResetSeconds();
            ResetMilliSeconds();
            ValidateTime();
        }

        /// <summary>
        /// Resets the Hours, Minutes, Seconds and MilliSeconds.
        /// </summary>
        public virtual void ResetHours()
        {
            Hour = 0;
            ResetMinutes();
            ResetSeconds();
            ResetMilliSeconds();
            ValidateTime();
        }

        /// <summary>
        /// Resets the Minutes, Seconds and MilliSeconds.
        /// </summary>
        public virtual void ResetMinutes()
        {
            Minute = 0;
            ResetSeconds();
            ResetMilliSeconds();
            ValidateTime();
        }
        
        /// <summary>
        /// Resets the Seconds and MilliSeconds.
        /// </summary>
        public virtual void ResetSeconds()
        {
            Second = 0;
            ResetMilliSeconds();
            ValidateTime();
        }
        
        /// <summary>
        /// Resets the MilliSeconds.
        /// </summary>
        public virtual void ResetMilliSeconds()
        {
            MilliSecond = 0;
            ValidateTime();
        }
        
        /// <summary>
        /// Resets all of the values to the defaults 1/1/1 00:00:00
        /// </summary>
        public virtual void ResetFullDate()
        {
            ResetYears();
            ValidateTime();
        }
        #endregion
    }
}