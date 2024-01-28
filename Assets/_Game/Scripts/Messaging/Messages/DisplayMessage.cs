namespace DLS.Messaging.Messages
{
    public struct DisplayMessage
    {
        public string Message { get; }
        public float Delay { get; }
        
        public DisplayMessage(string message, float delay)
        {
            Message = message;
            Delay = delay;
        }
    }
}