using UnityEngine;

namespace Time
{
    /// <summary>
    ///  Represents the MonthObject.
    /// </summary>
    [CreateAssetMenu(fileName ="Month", menuName ="Game/Time/Month", order = 1)]
    public class MonthObject : ScriptableObject
    {
        [field: Tooltip("The name of the MonthObject")]
        [field: SerializeField] public virtual string Name { get; set; }
        
        [field: Tooltip("The abbreviated name with 1 letter of the MonthObject")]
        [field: SerializeField] public virtual string Abbreviated1LetterName { get; set; }
        
        [field: Tooltip("The abbreviated name with 2 letters of the MonthObject")]
        [field: SerializeField] public virtual string Abbreviated2LetterName { get; set; }
        
        [field: Tooltip("The abbreviated name with 3 letters of the MonthObject")]
        [field: SerializeField] public virtual string Abbreviated3LetterName { get; set; }
        
        [field: Tooltip("The value of the MonthObject")]
        [field: SerializeField] public virtual int Value { get; set; }
        
        [field: Tooltip("The days in the MonthObject")]
        [field: SerializeField] public virtual int DaysInMonth { get; set; } = 31;
        
        [field: Tooltip("The days in the week")]
        [field: SerializeField] public virtual int DaysInWeek { get; set; } = 7;
        
        [field: Tooltip("Whether the MonthObject can have a leap day")]
        [field: SerializeField] public virtual bool CanHaveLeapDay { get; set; }

        /// <summary>
        ///  Gets the days in the MonthObject with the given year.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public virtual int GetDays(int year)
        {
            return DaysInMonth + (CanHaveLeapDay && IsLeapYear(year) ? 1 : 0);
        }

        /// <summary>
        ///  Checks if the MonthObject has a leap day with the given year.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public virtual bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
        }
    }
}