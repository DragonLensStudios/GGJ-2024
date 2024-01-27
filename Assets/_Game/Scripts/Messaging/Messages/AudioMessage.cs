using Audio;
using Enums;
using Unity.FPS.Game;

namespace Messaging.Messages
{
    public struct AudioMessage
    {
        public AudioObject AudioObject { get; }
        public string AudioName { get; }
        public AudioOperation Operation { get; }
        public AudioChannel? AudioChannel { get; }
        public bool? UseRandomVolume { get; }
        public bool? UseRandomPitch { get; }
        public float? Volume { get; }
        public float? Pitch { get; }

        /// <summary>
        /// Executes the AudioMessage method.
        /// Handles the AudioMessage functionality.
        /// </summary>
        public AudioMessage(AudioObject audioObject, AudioOperation operation, AudioChannel? audioChannel = null, bool? useRandomVolume = null, bool? useRandomPitch = null, float? volume = null, float? pitch = null)
        {
            AudioObject = audioObject;
            AudioName = AudioObject != null ? AudioObject.Name : string.Empty;
            Operation = operation;
            AudioChannel = audioChannel;
            UseRandomVolume = useRandomVolume;
            UseRandomPitch = useRandomPitch;
            Volume = volume;
            Pitch = pitch;
        }
        
        /// <summary>
        /// Executes the AudioMessage method.
        /// Handles the AudioMessage functionality.
        /// </summary>
        public AudioMessage(string audioName, AudioOperation operation, AudioChannel? audioChannel = null, bool? useRandomVolume = null, bool? useRandomPitch = null, float? volume = null, float? pitch = null)
        {
            AudioObject = null;
            AudioName = audioName;
            Operation = operation;
            AudioChannel = audioChannel;
            UseRandomVolume = useRandomVolume;
            UseRandomPitch = useRandomPitch;
            Volume = volume;
            Pitch = pitch;
        }
    }
}