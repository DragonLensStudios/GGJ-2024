using DLS.Chat;

namespace DLS.Messaging.Messages
{
    public struct AddChatMessage
    {
        public ViewerUser User { get; }
        public string Message { get; }
        
        public AddChatMessage(ViewerUser user, string message)
        {
            User = user;
            Message = message;
        }
    }
}