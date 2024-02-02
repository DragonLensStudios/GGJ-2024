using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using FPS.Scripts.Game;
using Newtonsoft.Json;
using UnityEngine;

namespace DLS.Chat
{
    public class ChatMessageController : MonoBehaviour
    {
        [field: SerializeField] public List<ViewerUser> CurrentUsers { get; set; } = new();
        [field: SerializeField] public List<ViewerUser> AvailableUsers { get; set; } = new();
        
        [field: SerializeField] public List<ViewerUser> AvailableGuestUsers { get; set; } = new();
        [field: SerializeField, EnumFlags] public UserGameType UserGameType { get; set; } = UserGameType.General;
        [field: SerializeField] public float TimeBetweenMessagesMin { get; set; } = 5f;
        [field: SerializeField] public float TimeBetweenMessagesMax { get; set; } = 15f;
        [field: SerializeField] public float TimeBetweenAddUsersMin { get; set; } = 5f;
        [field: SerializeField] public float TimeBetweenAddUsersMax { get; set; } = 60f;
        
        [field: SerializeField] public float SubscriberUserChance { get; set; } = 0.05f;
        private void Awake()
        {
            // Assuming there's a base path in the Resources folder for chat users
            string basePath = "ChatUsers/";

            // Convert UserGameType to a list of strings to match folder names
            List<string> userGameTypeFolders = GetUserGameTypeFolders(UserGameType);

            AvailableUsers.Clear(); // Clear the list to avoid duplicates

            foreach (var folder in userGameTypeFolders)
            {
                // Load all TextAsset objects from the specific folder
                var users = Resources.LoadAll<TextAsset>($"{basePath}{folder}");
                foreach (var userTextAsset in users)
                {
                    // Deserialize the JSON text to a ViewerUser object
                    var viewerUser = JsonConvert.DeserializeObject<ViewerUser>(userTextAsset.text);
                    if (viewerUser != null)
                    {
                        AvailableUsers.Add(viewerUser);
                    }
                }
            }
        }
        
        private void Start()
        {
            StartCoroutine(UserAddEnumerator());
            StartCoroutine(MessageAddEnumerator());
        }

        // Helper method to convert UserGameType flags to folder names
        private List<string> GetUserGameTypeFolders(UserGameType userGameType)
        {
            var folders = new List<string>();

            // Check each enum flag and add the matching folder name if the flag is set
            foreach (UserGameType gameType in Enum.GetValues(typeof(UserGameType)))
            {
                if (userGameType.HasFlag(gameType))
                {
                    // Convert the enum value to a string that matches the folder name
                    // Assuming the enum names directly match the folder names
                    folders.Add(gameType.ToString());
                }
            }

            return folders;
        }

        public IEnumerator UserAddEnumerator()
        {
            var randomTimeBetweenUsers = UnityEngine.Random.Range(TimeBetweenAddUsersMin, TimeBetweenAddUsersMax);
            yield return new WaitForSeconds(randomTimeBetweenUsers);

            ViewerUser randomUser = null;
            var randomSubscriberUserChance = UnityEngine.Random.Range(0f, 1f);
            var addSubscriber = randomSubscriberUserChance <= SubscriberUserChance;
            if (AvailableUsers.Count > 0 && addSubscriber)
            {
                randomUser = AvailableUsers[UnityEngine.Random.Range(0, AvailableUsers.Count)];
                if(CurrentUsers.Contains(randomUser) && AvailableUsers.Count > 0)
                {
                    StartCoroutine(UserAddEnumerator());
                }
                else if (!CurrentUsers.Contains(randomUser) && AvailableUsers.Count > 0)
                {
                    CurrentUsers.Add(randomUser);
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AddUserMessage(randomUser));
                    if (randomUser.UserType == UserType.Subscriber)
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AddSubscriberMessage(randomUser));
                    }
                    AvailableUsers.Remove(randomUser);
                    StartCoroutine(UserAddEnumerator());
                }
            }
            else
            {
                var randomTypeOfGuest = UnityEngine.Random.Range(0f, 1f);
                UserType userType = UserType.Guest;
                if (addSubscriber)
                {
                    userType = UserType.Subscriber;
                }

                var guest = new ViewerUser("Guest", "I'm a guest", "USA", 18, userType, UserGameType.General);
                AvailableGuestUsers.Add(guest);
                CurrentUsers.Add(guest);
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AddUserMessage(guest));
                if (guest.UserType == UserType.Subscriber)
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AddSubscriberMessage(guest));
                }
                StartCoroutine(UserAddEnumerator());
            }

        }
        
        public IEnumerator MessageAddEnumerator()
        {
            //TODO: Refactor to not use hardcoded string for guest.
            yield return new WaitUntil(() => CurrentUsers.Count(x=> !x.Username.Equals("Guest")) > 0);
            var currentRealUsers = CurrentUsers.Select(x=> x).Where(x => x.Username != "Guest").ToList();
            var randomTimeBetweenMessages = UnityEngine.Random.Range(TimeBetweenMessagesMin, TimeBetweenMessagesMax);
            yield return new WaitForSeconds(randomTimeBetweenMessages);
            var randomUser = currentRealUsers[UnityEngine.Random.Range(0, currentRealUsers.Count)];
            if (randomUser.ChatMessages.Count > 0)
            {
                var message = randomUser.ChatMessages[UnityEngine.Random.Range(0, randomUser.ChatMessages.Count)];
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AddChatMessage(randomUser, message.Message));
            }
            StartCoroutine(MessageAddEnumerator());
        }
       
    }
}