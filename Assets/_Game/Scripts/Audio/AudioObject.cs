using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Represents the Audio.
    /// The Audio class provides functionality related to audio management.
    /// This class contains methods and properties that assist in managing and processing audio related tasks.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioObject", menuName = "Game/Audio/AudioObject", order = 1)]
    public class AudioObject : ScriptableObject
    {
        [field: Tooltip("The actual audio clip that will be played.")]
        [field: SerializeField] public AudioClip Clip { get; set; }
        
        [field: Tooltip("Name of the audio clip for identification.")]
        [field: SerializeField] public string Name { get; set; }

        [field: Tooltip("Volume level of the audio clip. Ranges from 0 to 1.")]
        [field: SerializeField] [field: Range(0f, 1f)] public float Volume { get; set; } = 1f;

        [field: Tooltip("Pitch level of the audio clip. Ranges from 0.1 to 2")]
        [field: SerializeField] [field: Range(-3.0f, 3f)] public float Pitch { get; set; } = 1f;

        [field: Tooltip("Random variation to the volume level.")]
        [field: SerializeField] [field: Range(0f, 0.9f)] public float RandomVolume { get; set; } = 0f;

        [field: Tooltip("Random variation to the pitch level.")]
        [field: SerializeField] [field: Range(-3f, 3f)] public float RandomPitch { get; set; } = 0f;

        [field: Tooltip("Should the audio clip loop after it finishes playing?")]
        [field: SerializeField] public bool Loop { get; set; } = false;

        [field: Tooltip("The AudioSource component associated with this audio.")]
        [field: SerializeField] public AudioSource Source { get; set; }

        /// <summary>
        /// Executes the Play method.
        /// Handles the Play functionality.
        /// </summary>
        public void Play(bool? useRandomVolume = null, bool? useRandomPitch = null, float? v = null, float? p = null)
        {
            if (useRandomVolume.HasValue && useRandomVolume.Value)
            {
                if (v.HasValue)
                {
                    Source.volume = Volume * (1 + Random.Range(-v.Value / 2f, v.Value / 2f));
                }
                else
                {
                    Source.volume = Volume * (1 + Random.Range(-RandomVolume / 2f, RandomVolume / 2f));
                }
            }
            else
            {
                Source.volume = v ?? Volume;
            }

            if (useRandomPitch.HasValue && useRandomPitch.Value)
            {
                if (p.HasValue)
                {
                    Source.pitch = Pitch * (1 + Random.Range(-p.Value / 2f, p.Value / 2f));
                }
                else
                {
                    Source.pitch = Pitch * (1 + Random.Range(-RandomPitch / 2f, RandomPitch / 2f));
                }
            }
            else
            {
                Source.pitch = p ?? Pitch;
            }
            

            Source.Play();
        }

        /// <summary>
        /// Executes the Pause method.
        /// Handles the Pause functionality.
        /// </summary>
        public void Pause()
        {
            Source.Pause();
        }

        /// <summary>
        /// Executes the Resume method.
        /// Handles the Resume functionality.
        /// </summary>
        public void Resume()
        {
            Source.UnPause();
        }

        /// <summary>
        /// Executes the Stop method.
        /// Handles the Stop functionality.
        /// </summary>
        public void Stop()
        {
            Source.Stop();
        }
    }
}