using DLS.Chat;

namespace DLS.Messaging.Messages
{
    public struct AddSubscriberMessage
    {
        public ViewerUser User { get; }
        
        public AddSubscriberMessage(ViewerUser user)
        {
            User = user;
        }
    }
}