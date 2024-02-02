using System;
using System.Collections.Generic;
using DLS.Enums;
using UnityEngine;

namespace DLS.Chat
{
    [System.Serializable]
    public class ViewerUser
    {
        [field: SerializeField] public Guid Id { get; set; }
        [field: SerializeField] public string Username { get; set; }
        [field: SerializeField] public string PersonalityDescription { get; set; }
        [field: SerializeField] public string Country { get; set; }
        [field: SerializeField] public int Age { get; set; }
        [field: SerializeField] public UserType UserType { get; set; } = UserType.Guest;
        [field: SerializeField] public List<ChatMessage> ChatMessages { get; set; } = new();
        [field: SerializeField] public UserGameType UserGameType { get; set; }


        public ViewerUser()
        {
            Id = Guid.NewGuid();
        }
        
        public ViewerUser(string username, string personalityDescription, string country, int age, UserType userType, UserGameType userGameType)
        {
            Id = Guid.NewGuid();
            Username = username;
            PersonalityDescription = personalityDescription;
            Country = country;
            Age = age;
            UserType = userType;
            UserGameType = userGameType;
        }
    }
}