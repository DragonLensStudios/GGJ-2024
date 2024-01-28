using System.Collections.Generic;
using DLS.Messaging;
using DLS.Messaging.Messages;
using UnityEngine;

namespace DLS.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public List<EnemyController> Enemies { get; private set; }
        public int NumberOfEnemiesTotal { get; private set; }
        public int NumberOfEnemiesRemaining => Enemies.Count;

        void Awake()
        {
            Enemies = new List<EnemyController>();
        }

        public void RegisterEnemy(EnemyController enemy)
        {
            Enemies.Add(enemy);
            NumberOfEnemiesTotal++;
        }

        public void UnregisterEnemy(EnemyController enemyKilled)
        {
            int enemiesRemainingNotification = NumberOfEnemiesRemaining - 1;

            MessageSystem.MessageManager.BroadcastImmediate(new EnemyKillMessage(enemyKilled.gameObject, enemiesRemainingNotification));
            // removes the enemy from the list, so that we can keep track of how many are left on the map
            Enemies.Remove(enemyKilled);
        }
    }
}