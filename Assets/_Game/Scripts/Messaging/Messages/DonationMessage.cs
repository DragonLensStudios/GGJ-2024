using DLS.Chat;

namespace Messaging.Messages
{
    public struct DonationMessage
    {
        public ViewerUser ViewerUser { get; }
        public int Amount { get; }
        
        public DonationMessage(ViewerUser viewerUser, int amount)
        {
            ViewerUser = viewerUser;
            Amount = amount;
        }
    }
}