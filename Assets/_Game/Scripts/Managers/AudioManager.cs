using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DLS.Audio;
using DLS.Enums;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;
using UnityEngine.Audio;

namespace DLS.Managers
{
    /// <summary>
    /// Represents the AudioManager.
    /// The AudioManager class provides functionality related to audiomanager management.
    /// This class contains methods and properties that assist in managing and processing audiomanager related tasks.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        
        public static AudioManager Instance;
        
        /// <summary>
        ///  Is the audiomanager initialized? used to ensure that the audiomanager is initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; } = false;

        [field: Tooltip("The volume threshold.")]
        [field: SerializeField] [field: Header("Preferences")] public virtual float VolumeThreshold { get; set; } = -80.0f;
        
        [field: Tooltip("The audio mixer.")]
        [field: SerializeField] [field: Header("References")] public virtual AudioMixer Mixer { get; set; }
        
        [field: Tooltip("The music audio objects.")]
        [field: SerializeField] public virtual List<AudioObject> Music { get; set; }
        
        [field: Tooltip("The sound effects audio objects.")]
        [field: SerializeField] public virtual List<AudioObject> SoundEffects { get; set; }
        
        [field: Tooltip("The currently playing sound effects.")]
        [field: SerializeField] [field: Header("Currently Playing")] public virtual List<AudioObject> CurrentlyPlayingSfx { get; set; }
        
        [field: Tooltip("The currently playing music.")]
        [field: SerializeField] public virtual List<AudioObject> CurrentlyPlayingMusic { get; set; }

        /// <summary>
        ///  Singleton pattern for the audiomanager and loading the audio objects for the music and sound effects and spawning audio gameobjects with audio sources.
        /// </summary>
        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            Music = Resources.LoadAll<AudioObject>("Audio/Music").ToList();
            SoundEffects = Resources.LoadAll<AudioObject>("Audio/Sound Effects").ToList();

            for (int i = 0; i < Music.Count; i++)
            {
                GameObject audioObject = new GameObject("Music_" + i + "_" + Music[i].Name);
                audioObject.transform.parent = transform;
                audioObject.AddComponent<AudioSource>();
                audioObject.GetComponent<AudioSource>().outputAudioMixerGroup = Mixer.FindMatchingGroups("Music")[0];
                audioObject.GetComponent<AudioSource>().loop = Music[i].Loop;
                audioObject.GetComponent<AudioSource>().volume = Music[i].Volume;
                audioObject.GetComponent<AudioSource>().clip = Music[i].Clip;
                Music[i].Source = audioObject.GetComponent<AudioSource>();
            }

            for (int i = 0; i < SoundEffects.Count; i++)
            {
                GameObject audioObject = new GameObject("Effects_" + i + "_" + SoundEffects[i].Name);
                audioObject.transform.parent = transform;
                audioObject.AddComponent<AudioSource>();
                audioObject.GetComponent<AudioSource>().outputAudioMixerGroup = Mixer.FindMatchingGroups("SFX")[0];
                audioObject.GetComponent<AudioSource>().loop = SoundEffects[i].Loop;
                audioObject.GetComponent<AudioSource>().volume = SoundEffects[i].Volume;
                audioObject.GetComponent<AudioSource>().clip = SoundEffects[i].Clip;
                SoundEffects[i].Source = audioObject.GetComponent<AudioSource>();
            }

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        ///  When the object is enabled, load the player preferences for volume values for master, music, and sound effects.
        /// </summary>
        public virtual void Start()
        {
            Mixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("masterVolume"));
            Mixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("musicVolume"));
            Mixer.SetFloat("effectsVolume", PlayerPrefs.GetFloat("effectsVolume"));
            IsInitialized = true;
        }

        public virtual void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<AudioMessage>(MessageChannels.Audio, SetAudio);
        }

        public virtual void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<AudioMessage>(MessageChannels.Audio, SetAudio);
        }

        /// <summary>
        /// Play music from the array.
        /// Executes the PlayMusic method.
        /// Handles the PlayMusic functionality.
        /// </summary>
        public virtual void PlayMusic(AudioObject audioObj, bool? useRandomVolume = null, bool? useRandomPitch = null, float? volume = null, float? pitch = null)
        {
            var music = Music.Find(x=> x.Equals(audioObj));
            if(music == null) return;
            if (CurrentlyPlayingMusic.Contains(music))
            {
                music.Stop();
                CurrentlyPlayingMusic.Remove(music);
            }
            CurrentlyPlayingMusic.Add(music);
            music.Play(useRandomVolume, useRandomPitch, volume, pitch);
            StartCoroutine(StopPlayingMusicAfterAudioClipPlays(music.Clip, audioObj.Name));
        }
        
        /// <summary>
        /// Play music from the array.
        /// Executes the PlayMusic method.
        /// Handles the PlayMusic functionality.
        /// </summary>
        public virtual void PlayMusic(string audioName, bool? useRandomVolume = null, bool? useRandomPitch = null, float? volume = null, float? pitch = null)
        {
            var music = Music.FirstOrDefault(x=> x.Name.Equals(audioName));
            if(music == null) return;
            if (CurrentlyPlayingMusic.Contains(music)) return;
            CurrentlyPlayingMusic.Add(music);
            music.Play(useRandomVolume, useRandomPitch, volume, pitch);
            StartCoroutine(StopPlayingMusicAfterAudioClipPlays(music.Clip, audioName));
        }
        
        /// <summary>
        /// Play a sound-effect from the array.
        /// Executes the PlaySound method.
        /// Handles the PlaySound functionality.
        /// </summary>
        public virtual void PlaySound(AudioObject audioObj, bool? useRandomVolume = null, bool? useRandomPitch = null, float? volume = null, float? pitch = null)
        {
            var sfx = SoundEffects.Find(x => x.Equals(audioObj));
            if (sfx == null) return;
            if (CurrentlyPlayingSfx.Contains(sfx)) return;
            CurrentlyPlayingSfx.Add(sfx);
            sfx.Play(useRandomVolume, useRandomPitch, volume, pitch);
            StartCoroutine(StopPlayingSoundAfterAudioClipPlays(sfx.Clip, audioObj.Name));
        }

        /// <summary>
        /// Play a sound-effect from the array.
        /// Executes the PlaySound method.
        /// Handles the PlaySound functionality.
        /// </summary>
        public virtual void PlaySound(string audioName, bool? useRandomVolume = null, bool? useRandomPitch = null, float? volume = null, float? pitch = null)
        {
            var sfx = SoundEffects.FirstOrDefault(x => x.Name.Equals(audioName));
            if (sfx == null) return;
            if (CurrentlyPlayingSfx.Contains(sfx)) return;
            CurrentlyPlayingSfx.Add(sfx);
            sfx.Play(useRandomVolume, useRandomPitch, volume, pitch);
            StartCoroutine(StopPlayingSoundAfterAudioClipPlays(sfx.Clip, audioName));
        }

        
        /// <summary>
        /// Pause music from the array.
        /// Executes the PauseMusic method.
        /// Handles the PauseMusic functionality.
        /// </summary>
        public virtual void PauseMusic(AudioObject audioObj)
        {
            var music = Music.Find(x => x.Equals(audioObj));
            if (music == null) return;
            music.Pause();
        }
        
        /// <summary>
        /// Pause music from the array.
        /// Executes the PauseMusic method.
        /// Handles the PauseMusic functionality.
        /// </summary>
        public virtual void PauseMusic(string audioName)
        {
            var music = Music.FirstOrDefault(x => x.Name.Equals(audioName));
            if (music == null) return;
            music.Pause();
        }
        
        /// <summary>
        /// Pause a sound from the array.
        /// Executes the PauseSound method.
        /// Handles the PauseSound functionality.
        /// </summary>
        public virtual void PauseSound(AudioObject audioObj)
        {
            var sfx = SoundEffects.Find(x => x.Equals(audioObj));
            if (sfx == null) return;
            sfx.Pause();
        }

        /// <summary>
        /// Pause a sound from the array.
        /// Executes the PauseSound method.
        /// Handles the PauseSound functionality.
        /// </summary>
        public virtual void PauseSound(string audioName)
        {
            var sfx = SoundEffects.FirstOrDefault(x => x.Name.Equals(audioName));
            if (sfx == null) return;
            sfx.Pause();
        }
        
        /// <summary>
        /// Resume music from the array.
        /// Executes the ResumeMusic method.
        /// Handles the ResumeMusic functionality.
        /// </summary>
        public virtual void ResumeMusic(AudioObject audioObj)
        {
            var music = Music.Find(x => x.Equals(audioObj));
            if (music == null) return;
            music.Resume();
        }

        /// <summary>
        /// Resume music from the array.
        /// Executes the ResumeMusic method.
        /// Handles the ResumeMusic functionality.
        /// </summary>
        public virtual void ResumeMusic(string audioName)
        {
            var music = Music.FirstOrDefault(x => x.Name.Equals(audioName));
            if (music == null) return;
            music.Resume();
        }
        
        /// <summary>
        /// Resume a sound-effect from the array.
        /// Executes the ResumeSound method.
        /// Handles the ResumeSound functionality.
        /// </summary>
        public virtual void ResumeSound(AudioObject audioObj)
        {
            var sfx = SoundEffects.Find(x => x.Equals(audioObj));
            if (sfx == null) return;
            sfx.Resume();
        }

        /// <summary>
        /// Resume a sound-effect from the array.
        /// Executes the ResumeSound method.
        /// Handles the ResumeSound functionality.
        /// </summary>
        public virtual void ResumeSound(string audioName)
        {
            var sfx = SoundEffects.FirstOrDefault(x => x.Name.Equals(audioName));
            if (sfx == null) return;
            sfx.Resume();
        }

        
        /// <summary>
        /// Stop music from the array.
        /// Executes the StopMusic method.
        /// Handles the StopMusic functionality.
        /// </summary>
        public virtual void StopMusic(AudioObject audioObj)
        {
            var music = Music.Find(x => x.Equals(audioObj));
            if (music == null) return;
            CurrentlyPlayingMusic.Remove(music);
            music.Stop();
        }
        
        /// <summary>
        /// Stop music from the array.
        /// Executes the StopMusic method.
        /// Handles the StopMusic functionality.
        /// </summary>
        public virtual void StopMusic(string audioName)
        {
            var music = Music.FirstOrDefault(x => x.Name.Equals(audioName));
            if (music == null) return;
            CurrentlyPlayingMusic.Remove(music);
            music.Stop();
        }
        
        /// <summary>
        /// Stop a sound-effect from the array.
        /// Executes the StopSound method.
        /// Handles the StopSound functionality.
        /// </summary>
        public virtual void StopSound(AudioObject audioObj)
        {
            var sfx = SoundEffects.Find(x => x.Equals(audioObj));
            if (sfx == null) return;
            CurrentlyPlayingSfx.Remove(sfx);
            sfx.Stop();
        }

        /// <summary>
        /// Stop a sound-effect from the array.
        /// Executes the StopSound method.
        /// Handles the StopSound functionality.
        /// </summary>
        public virtual void StopSound(string audioName)
        {
            var sfx = SoundEffects.FirstOrDefault(x => x.Name.Equals(audioName));
            if (sfx == null) return;
            CurrentlyPlayingSfx.Remove(sfx);
            sfx.Stop();
        }

        /// <summary>
        /// Set the master volume of the audio mixer.
        /// Executes the SetMasterVolume method.
        /// Handles the SetMasterVolume functionality.
        /// </summary>
        public virtual void SetMasterVolume(float sliderValue)
        {
            if (sliderValue <= 0)
            {
                Mixer.SetFloat("masterVolume", VolumeThreshold);
                PlayerPrefs.SetFloat("masterVolume", -80);
            }
            else
            {
                // Translate unit range to logarithmic value. 
                float value = 20f * Mathf.Log10(sliderValue);
                Mixer.SetFloat("masterVolume", value);
                PlayerPrefs.SetFloat("masterVolume", value);
            }
        }

        /// <summary>
        /// Set the music volume of the audio mixer.
        /// Executes the SetMusicVolume method.
        /// Handles the SetMusicVolume functionality.
        /// </summary>
        public virtual void SetMusicVolume(float sliderValue)
        {
            if (sliderValue <= 0)
            {
                Mixer.SetFloat("musicVolume", VolumeThreshold);
                PlayerPrefs.SetFloat("musicVolume", -80);
            }
            else
            {
                // Translate unit range to logarithmic value. 
                float value = 20f * Mathf.Log10(sliderValue);
                Mixer.SetFloat("musicVolume", value);
                PlayerPrefs.SetFloat("musicVolume", value);
            }
        }

        /// <summary>
        /// Set the SFX volume of the audio mixer.
        /// Executes the SetSoundEffectsVolume method.
        /// Handles the SetSoundEffectsVolume functionality.
        /// </summary>
        public virtual void SetSoundEffectsVolume(float sliderValue)
        {
            if (sliderValue <= 0)
            {
                Mixer.SetFloat("effectsVolume", VolumeThreshold);
                PlayerPrefs.SetFloat("effectsVolume", -80);
            }
            else
            {
                // Translate unit range to logarithmic value. 
                float value = 20f * Mathf.Log10(sliderValue);
                Mixer.SetFloat("effectsVolume", value);
                PlayerPrefs.SetFloat("effectsVolume", value);
            }
        }

        /// <summary>
        /// Clear the master volume of the audio mixer. This is useful for audio snapshots.
        /// Executes the ClearMasterVolume method.
        /// Handles the ClearMasterVolume functionality.
        /// </summary>
        public virtual void ClearMasterVolume()
        {
            Mixer.ClearFloat("masterVolume");
        }

        /// <summary>
        /// Clear the music volume of the audio mixer. This is useful for audio snapshots.
        /// Executes the ClearMusicVolume method.
        /// Handles the ClearMusicVolume functionality.
        /// </summary>
        public virtual void ClearMusicVolume()
        {
            Mixer.ClearFloat("musicVolume");
        }

        /// <summary>
        /// Clear the SFX volume of the audio mixer. This is useful for audio snapshots.
        /// Executes the ClearSoundEffectsVolume method.
        /// Handles the ClearSoundEffectsVolume functionality.
        /// </summary>
        public virtual void ClearSoundEffectsVolume()
        {
            Mixer.ClearFloat("effectsVolume");
        }

        /// <summary>
        ///  Stop playing sound after audio clip plays.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="audioName"></param>
        /// <returns></returns>
        public virtual IEnumerator StopPlayingSoundAfterAudioClipPlays(AudioClip clip, string audioName)
        {
            yield return new WaitForSeconds(clip.length);
            CurrentlyPlayingSfx.Remove(CurrentlyPlayingSfx.FirstOrDefault(x=> x.Name.Equals(audioName)));
        }
    
        /// <summary>
        ///  Stop playing music after audio clip plays.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="audioName"></param>
        /// <returns></returns>
        public virtual IEnumerator StopPlayingMusicAfterAudioClipPlays(AudioClip clip, string audioName)
        {
            yield return new WaitForSeconds(clip.length);
            CurrentlyPlayingMusic.Remove(CurrentlyPlayingMusic.FirstOrDefault(x=> x.Name.Equals(audioName)));
        }
        
        /// <summary>
        ///  Set the audio and handles the audio operation for the audio channel with values Music, SoundEffects and operation Play, Resume, Pause, Stop, SetVolume.
        /// </summary>
        /// <param name="message"></param>
        public virtual void SetAudio(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AudioMessage>().HasValue) return;

            var audioMessage = message.Message<AudioMessage>().GetValueOrDefault();

            switch (audioMessage.Operation)
            {
                case AudioOperation.Play:
                    switch (audioMessage.AudioChannel)
                    {
                        case AudioChannel.Music:
                            if (audioMessage.AudioObject != null)
                            {
                                PlayMusic(audioMessage.AudioObject, audioMessage.UseRandomVolume, audioMessage.UseRandomPitch, audioMessage.Volume, audioMessage.Pitch);
                            }
                            else if (!string.IsNullOrWhiteSpace(audioMessage.AudioName))
                            {
                                PlayMusic(audioMessage.AudioName, audioMessage.UseRandomVolume, audioMessage.UseRandomPitch, audioMessage.Volume, audioMessage.Pitch);
                            }
                            break;
                        case AudioChannel.SoundEffects:
                            if (audioMessage.AudioObject != null)
                            {
                                PlaySound(audioMessage.AudioObject, audioMessage.UseRandomVolume, audioMessage.UseRandomPitch, audioMessage.Volume, audioMessage.Pitch);
                            }
                            else if (!string.IsNullOrWhiteSpace(audioMessage.AudioName))
                            {
                                PlaySound(audioMessage.AudioName, audioMessage.UseRandomVolume, audioMessage.UseRandomPitch, audioMessage.Volume, audioMessage.Pitch);
                            }
                            break;
                        default:
                            Debug.Log("Invalid volume type.");
                            break;
                    }
                    break;
                case AudioOperation.Resume:
                    switch (audioMessage.AudioChannel)
                    {
                        case AudioChannel.Music:
                            if (audioMessage.AudioObject != null)
                            {
                                ResumeMusic(audioMessage.AudioObject);
                            }
                            else if (!string.IsNullOrWhiteSpace(audioMessage.AudioName))
                            {
                                ResumeMusic(audioMessage.AudioName);
                            }
                            break;
                        case AudioChannel.SoundEffects:
                            if (audioMessage.AudioObject != null)
                            {
                                ResumeSound(audioMessage.AudioObject);
                            }
                            else if (!string.IsNullOrWhiteSpace(audioMessage.AudioName))
                            {
                                ResumeSound(audioMessage.AudioName);
                            }
                            break;
                        default:
                            Debug.Log("Invalid volume type.");
                            break;
                    }
                    break;
                case AudioOperation.Pause:
                    switch (audioMessage.AudioChannel)
                    {
                        case AudioChannel.Music:
                            if (audioMessage.AudioObject != null)
                            {
                                PauseMusic(audioMessage.AudioObject);
                            }
                            else if (!string.IsNullOrWhiteSpace(audioMessage.AudioName))
                            {
                                PauseMusic(audioMessage.AudioName);
                            }
                            break;
                        case AudioChannel.SoundEffects:
                            if (audioMessage.AudioObject != null)
                            {
                                PauseSound(audioMessage.AudioObject);
                            }
                            else if (!string.IsNullOrWhiteSpace(audioMessage.AudioName))
                            {
                                PauseSound(audioMessage.AudioName);
                            }
                            break;
                        default:
                            Debug.Log("Invalid volume type.");
                            break;
                    }
                    break;
                case AudioOperation.Stop:
                    switch (audioMessage.AudioChannel)
                    {
                        case AudioChannel.Music:
                            if (audioMessage.AudioObject != null)
                            {
                                StopMusic(audioMessage.AudioObject);
                            }
                            else if (!string.IsNullOrWhiteSpace(audioMessage.AudioName))
                            {
                                StopMusic(audioMessage.AudioName);
                            }
                            break;
                        case AudioChannel.SoundEffects:
                            if (audioMessage.AudioObject != null)
                            {
                                StopSound(audioMessage.AudioObject);
                            }
                            else if (!string.IsNullOrWhiteSpace(audioMessage.AudioName))
                            {
                                StopSound(audioMessage.AudioName);
                            }
                            break;
                        default:
                            Debug.Log("Invalid volume type.");
                            break;
                    }
                    break;
                case AudioOperation.SetVolume:
                    if (audioMessage is { Volume: not null, AudioChannel: not null })
                    {
                        switch (audioMessage.AudioChannel.Value)
                        {
                            case AudioChannel.Master:
                                SetMasterVolume(audioMessage.Volume.Value);
                                break;
                            case AudioChannel.Music:
                                SetMusicVolume(audioMessage.Volume.Value);
                                break;
                            case AudioChannel.SoundEffects:
                                SetSoundEffectsVolume(audioMessage.Volume.Value);
                                break;
                            default:
                                Debug.Log("Invalid volume type.");
                                break;
                        }
                    }
                    break;
                default:
                    Debug.Log("Invalid audio operation.");
                    break;
            }
        }

    }
}