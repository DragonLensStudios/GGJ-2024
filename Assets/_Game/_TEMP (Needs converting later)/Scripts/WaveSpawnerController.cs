using System;
using System.Collections;
using System.Collections.Generic;
using FPS.Scripts.AI;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using UnityEngine;

public class WaveSpawnerController : MonoBehaviour
{
    [field: SerializeField] public int WaveNumber { get; set; } = 1;
    
    [field: SerializeField] public List<Transform> EnemySpawnPoints { get; set; } = new();
    [field: SerializeField] public List<Transform> BossSpawnPoints { get; set; } = new();
    
    [field: SerializeField] public List<GameObject> EnemiesToSpawn { get; set; } = new();
    [field: SerializeField] public List<GameObject> BossesToSpawn { get; set; } = new();
    
    [field: SerializeField] public int EnemiesPerWave { get; set; } = 2;
    [field: SerializeField] public int BossesPerWave { get; set; } = 1;
    
    [field: SerializeField] public float TimeBetweenWaves { get; set; } = 10f;
    
    [field: SerializeField] public float TimeBetweenSpawns { get; set; } = 1f;
    
    [field: SerializeField] public float TimeAddedBetweenWavesMultiplier { get; set; } = 15f;
    
    [field: SerializeField] public float TimeBeforeFirstWave { get; set; } = 5f;
    
    //TODO: Remove this and handle with a message instead.
    protected EnemyManager EnemyManager { get; set; }
    
    protected List<Transform> UsedEnemySpawnPoints { get; set; } = new();
    protected List<Transform> UsedBossSpawnPoints { get; set; } = new();
    
    protected PatrolPath PatrolPath { get; set; }

    private void Awake()
    {
        EnemyManager = FindObjectOfType<EnemyManager>();
        PatrolPath = FindObjectOfType<PatrolPath>();
    }

    private void Start()
    {
        DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
        displayMessage.Message = $"Wave {WaveNumber} Incoming!";
        displayMessage.DelayBeforeDisplay = 0f;
        EventManager.Broadcast(displayMessage);
        StartCoroutine(SpawnEnemies());
    }

    private void StartNextWave()
    {
        WaveNumber++;
        EnemiesPerWave++;
        BossesPerWave++;
        TimeBetweenWaves += TimeAddedBetweenWavesMultiplier;
        StartCoroutine(SpawnEnemies());
        UsedBossSpawnPoints.Clear();
        UsedEnemySpawnPoints.Clear();
        DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
        displayMessage.Message = $"Wave {WaveNumber} Incoming!";
        displayMessage.DelayBeforeDisplay = 0f;
        EventManager.Broadcast(displayMessage);
    }

    protected virtual IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(TimeBeforeFirstWave);
        
        for (int i = 0; i < EnemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(TimeBetweenSpawns);
        }
        
        for (int i = 0; i < BossesPerWave; i++)
        {
            SpawnBoss();
            yield return new WaitForSeconds(TimeBetweenSpawns);
        }
        
        yield return new WaitForSeconds(TimeBetweenWaves);
        
        StartNextWave();
        
    }
    
    protected virtual bool SpawnEnemy()
    {
        if (EnemySpawnPoints.Count <= 0)
        {
            return false;
        }
        var spawnPoint = EnemySpawnPoints[UnityEngine.Random.Range(0, EnemySpawnPoints.Count)];
        if(UsedEnemySpawnPoints.Count < EnemySpawnPoints.Count)
        {
            while (UsedEnemySpawnPoints.Contains(spawnPoint))
            {
                spawnPoint = EnemySpawnPoints[UnityEngine.Random.Range(0, EnemySpawnPoints.Count)];
            }
            UsedEnemySpawnPoints.Add(spawnPoint);
        }
        else
        {
            return false;
        }
        var enemy = Instantiate(EnemiesToSpawn[UnityEngine.Random.Range(0, EnemiesToSpawn.Count)]);
        enemy.transform.position = spawnPoint.position;
        enemy.GetComponent<EnemyController>().PatrolPath = PatrolPath;
        return true;
    }
    
    protected virtual bool SpawnBoss()
    {
        if (BossSpawnPoints.Count <= 0)
        {
            return false;
        }
        var spawnPoint = BossSpawnPoints[UnityEngine.Random.Range(0, BossSpawnPoints.Count)];
        if(UsedBossSpawnPoints.Count < BossSpawnPoints.Count)
        {
            while (UsedBossSpawnPoints.Contains(spawnPoint))
            {
                spawnPoint = BossSpawnPoints[UnityEngine.Random.Range(0, BossSpawnPoints.Count)];
            }
            UsedBossSpawnPoints.Add(spawnPoint);
        }
        else
        {
            return false;
        }
        var boss = Instantiate(BossesToSpawn[UnityEngine.Random.Range(0, BossesToSpawn.Count)]);
        boss.transform.position = spawnPoint.position;
        return true;
    }
}
