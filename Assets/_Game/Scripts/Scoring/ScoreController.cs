using UnityEngine;

namespace Scoring
{
    [System.Serializable]
    public class ScoreController : MonoBehaviour
    {
        [field: SerializeField] public long Score { get; set; }
        [field: SerializeField] public long TimeScore { get; set; }
        [field: SerializeField] public long ViewerScore { get; set; }
        [field: SerializeField] public long SubscriberScore { get; set; }
        [field: SerializeField] public long DonationScore { get; set; }
        
        
        
    }
}