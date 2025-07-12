using Enums;

namespace Messaging.Messages
{
    public struct ScoreMessage
    {
       public MathOperation Operation { get; } 
       public ScoreType ScoreType { get; }
       public long Value { get; }

       public ScoreMessage(MathOperation op, ScoreType scoreType = ScoreType.None, long value = 0)
       {
            Operation = op;
            ScoreType = scoreType;
            Value = value;
       }
              
    }
}