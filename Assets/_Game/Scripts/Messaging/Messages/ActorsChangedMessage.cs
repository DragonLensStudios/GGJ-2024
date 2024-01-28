using DLS.Enums;
using DLS.Actor;

namespace DLS.Messaging.Messages
{
    public struct ActorsChangedMessage
    {
        public Actor.Actor Actor { get; }
        public DataOperation Operation { get; }
        
        public ActorsChangedMessage(Actor.Actor actor, DataOperation operation)
        {
            Actor = actor;
            Operation = operation;
        }
    }
}