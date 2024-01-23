using System.Collections;
using System.Collections.Generic;
using Enums;
using Managers;
using Messaging;
using Messaging.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
/// <summary>
/// Represents the SceneMusicPlayer.
/// The SceneMusicPlayer class provides functionality related to scenemusicplayer management.
/// This class contains methods and properties that assist in managing and processing scenemusicplayer related tasks.
/// </summary>
    public class SceneMusicPlayer : MonoBehaviour
    {
        [field: Tooltip("The list of music to play in order of scene index.")]
        [field: SerializeField] public List<AudioObject> MusicList { get; set; }
        
        [field: Tooltip("The amount of time to wait before retrying to send the message.")]
        [field: SerializeField] public float RetryDelay { get; set; } = 0.1f;

        /// <summary>
        ///  When the object is enabled, subscribe to the sceneLoaded event.
        /// </summary>
        public virtual void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        /// <summary>
        ///  When the object is disabled, unsubscribe from the sceneLoaded event.
        /// </summary>
        public virtual void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        /// <summary>
        ///  When a scene is loaded, send a message to play the music for that scene.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        public virtual void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (MusicList.Count <= 0 || MusicList.Count < scene.buildIndex + 1) return;
            StartCoroutine(DelayedMessageSend(scene.buildIndex));
        }
        
        /// <summary>
        ///  Send the message to play the music after a delay to allow AudioManager to initialize.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual IEnumerator DelayedMessageSend(int index)
        {
            
            // Wait until AudioManager is initialized
            while (!AudioManager.IsInitialized)
            {
                yield return new WaitForSeconds(RetryDelay);
            }
            
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(MusicList[index], AudioOperation.Play, AudioChannel.Music));
        }
    }
}
