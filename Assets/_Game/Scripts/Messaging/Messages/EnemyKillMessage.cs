using UnityEngine;

namespace DLS.Messaging.Messages
{
    public struct EnemyKillMessage
    {
        public GameObject Enemy { get; }
        public int RemainingEnemyCount { get; }
        
        public EnemyKillMessage(GameObject enemy, int remainingEnemyCount)
        {
            Enemy = enemy;
            RemainingEnemyCount = remainingEnemyCount;
        }
    }
}