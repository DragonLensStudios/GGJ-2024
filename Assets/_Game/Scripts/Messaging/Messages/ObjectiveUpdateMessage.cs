using Unity.FPS.Game;

namespace Messaging.Messages
{
    public struct ObjectiveUpdateMessage
    {
        public Objective Objective { get; }
        public string DescriptionText { get; }
        public string CounterText { get; }
        public string NotificationText { get; }
        public bool IsComplete { get; }
        
        public ObjectiveUpdateMessage(Objective objective, string descriptionText, string counterText, string notificationText, bool isComplete = false)
        {
            Objective = objective;
            DescriptionText = descriptionText;
            CounterText = counterText;
            NotificationText = notificationText;
            IsComplete = isComplete;
        }

    }
}