using System.Collections;
using System.Collections.Generic;
using GEmojiSharp;
using TMPro;
using UnityEngine;

public class ChatController : MonoBehaviour
{
    [field: SerializeField] public List<ChatMessage> ChatMessages { get; set; } = new();
    
}