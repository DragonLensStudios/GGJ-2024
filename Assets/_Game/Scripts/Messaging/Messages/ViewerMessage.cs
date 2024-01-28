using Enums;

namespace Messaging.Messages
{
    public struct ViewerMessage
    {
        //public MathOperation Operation { get; }
        //public ScoreType ScoreType { get; }
        public int Value { get; }

        public ViewerMessage(int value = 0)
        {
            //Operation = op;
            //ScoreType = scoreType;
            Value = value;
        }

    }
}