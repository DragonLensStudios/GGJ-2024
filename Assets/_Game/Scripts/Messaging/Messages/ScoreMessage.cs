using Enums;

namespace Messaging.Messages
{
    public struct ScoreMessage
    {
       public MathOperation Operation { get; } 
       public ScoreType ScoreType { get; }
       public int Value { get; }

       public ScoreMessage(MathOperation op, ScoreType scoreType = ScoreType.None, int value = 0)
       {
            Operation = op;
            ScoreType = scoreType;
            Value = value;
       }
              
    }
}