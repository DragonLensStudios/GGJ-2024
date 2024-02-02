using System.Runtime.InteropServices;
using DLS.Chat;

namespace DLS.Messaging.Messages
{
    public struct AddUserMessage
    {
        public ViewerUser User { get; }
        
        public AddUserMessage(ViewerUser user)
        {
            User = user;
        }
    }
}