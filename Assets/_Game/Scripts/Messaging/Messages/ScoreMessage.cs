using Audio;
using Enums;

namespace Messaging.Messages
{
    public struct ScoreMessage
    {
        public MathOperation Operation { get; }
       public long Value { get; }

       public ScoreMessage(MathOperation op, long value = 0)
       {
        Operation = op;
        Value = value;
       }
              
    }
}