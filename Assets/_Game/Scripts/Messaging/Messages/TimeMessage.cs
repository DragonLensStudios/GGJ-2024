using Enums;

namespace Messaging.Messages
{
    public struct TimeMessage
    {
        public TimeOperation Operation { get; }

        public float? Year { get; }
        public float? Month { get; }
        public float? Week { get; }
        public float? Day { get; }
        public float? Hour { get; }
        public float? Minute { get; }
        public float? Second { get; }
        public float? MilliSecond { get; }

        public TimeMessage(TimeOperation operation, float? year = null, float? month = null, float? week = null,
            float? day = null, float? hour = null, float? minute = null, float? second = null, float? milliSecond = null)
        {
            Operation = operation;
            Year = year;
            Month = month;
            Week = week;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            MilliSecond = milliSecond;
        }
    }
}