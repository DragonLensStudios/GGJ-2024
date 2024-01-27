namespace Messaging.Messages
{
    public struct AddChatMessage
    {
        public string Sender { get; }
        public string Message { get; }
        
        public AddChatMessage(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }
}