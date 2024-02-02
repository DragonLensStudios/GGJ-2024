using DLS.Enums;
using UnityEngine;

namespace DLS.Chat
{
    [System.Serializable]
    public class ChatMessage
    {
        [field: SerializeField] public string Message { get; set; }
        [field: SerializeField] public ChatResponseTypes ResponseType { get; set; }
    
        public ChatMessage(string message, ChatResponseTypes responseType = ChatResponseTypes.Neutral)
        {
            Message = message;
            ResponseType = responseType;
        }
    }
}