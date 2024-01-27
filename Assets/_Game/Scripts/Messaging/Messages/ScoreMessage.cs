using Audio;
using Enums;

namespace Messaging.Messages
{
    public struct ScoreMessage
    {
       public MathOperation Operation { get; }
       public int Value { get; }

       public ScoreMessage(MathOperation op, int value = 0)
       {
        Operation = op;
        Value = value;
       }
              
    }
}