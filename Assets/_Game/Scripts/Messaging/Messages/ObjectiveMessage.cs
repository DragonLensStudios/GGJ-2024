using DLS.Enums;
using JetBrains.Annotations;

namespace DLS.Messaging.Messages
{
    public struct ObjectiveMessage
    {
        public Objective.Objective Objective { get; }
        public ObjectiveStatus Status { get; }
        [CanBeNull] public string DescriptionText { get; }
        [CanBeNull] public string CounterText { get; }
        [CanBeNull] public string NotificationText { get; }

        public ObjectiveMessage(Objective.Objective objective, ObjectiveStatus status, string descriptionText = null, string counterText = null, string notificationText = null)
        {
            Objective = objective;
            Status = status;
            DescriptionText = descriptionText;
            CounterText = counterText;
            NotificationText = notificationText;
        }
    }
}