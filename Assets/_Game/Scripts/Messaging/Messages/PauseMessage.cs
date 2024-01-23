namespace Messaging.Messages
{
    public struct PauseMessage
    {
        public bool IsPaused { get; }
        public bool ShowPauseMenu { get;  }
        
        public PauseMessage(bool isPaused, bool showPauseMenu = false)
        {
            IsPaused = isPaused;
            ShowPauseMenu = showPauseMenu;
        }
    }
}