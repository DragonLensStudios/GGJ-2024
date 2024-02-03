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
    
    [field: SerializeField] public float EnemiesPerWave { get; set; } = 1f;
    [field: SerializeField] public float BossesPerWave { get; set; } = 0.5f;
    [field: SerializeField] public float MaxEnemiesPerWave { get; set; } = 10f;
    [field: SerializeField] public float MaxBossesPerWave { get; set; } = 3f;

    [field: SerializeField] public float TimeBetweenWaves { get; set; } = 10f;
    [field: SerializeField] public float MaxTimeBetweenWaves { get; set; } = 30f;
    
    [field: SerializeField] public float TimeBetweenSpawns { get; set; } = 1f;
    
    [field: SerializeField] public float TimeAddedBetweenWavesMultiplier { get; set; } = 15f;
    
    [field: SerializeField] public float TimeBeforeFirstWave { get; set; } = 5f;
    
    [field: SerializeField] public float TimeSinceLastWave { get; set; } = 0f;
    [field: SerializeField] public bool MustDefeatEnemiesBeforeNextWave { get; set; } = true;


    //TODO: Remove this and handle with a message instead.
    protected EnemyManager EnemyManager { get; set; }
    
    protected List<Transform> UsedEnemySpawnPoints { get; set; } = new();
    protected List<Transform> UsedBossSpawnPoints { get; set; } = new();
    
    protected PatrolPath PatrolPath { get; set; }
    protected int CurrentEnemyCount = 0;
    protected int CurrentBossCount = 0;
    

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

    private void Update()
    {
        TimeSinceLastWave += Time.deltaTime;
    }

    private void StartNextWave()
    {
        WaveNumber++;
        EnemiesPerWave = Mathf.Floor(WaveNumber * EnemiesPerWave);
        BossesPerWave = Mathf.Floor(WaveNumber * BossesPerWave);
        TimeBetweenWaves += TimeAddedBetweenWavesMultiplier;
        TimeBetweenWaves = Mathf.Clamp(TimeBetweenWaves, 0f, MaxTimeBetweenWaves);
        StartCoroutine(SpawnEnemies());
        UsedBossSpawnPoints.Clear();
        UsedEnemySpawnPoints.Clear();
        DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
        displayMessage.Message = $"Wave {WaveNumber} Incoming!";
        displayMessage.DelayBeforeDisplay = 0f;
        EventManager.Broadcast(displayMessage);
        TimeSinceLastWave = 0f;
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
        
        if(EnemyManager.Enemies.Count <= 0 && TimeSinceLastWave < MaxTimeBetweenWaves)
        {
            if (EnemyManager.Enemies.Count <= 0)
            {
                CurrentEnemyCount = 0;
                CurrentBossCount = 0;
            }

            StartNextWave();
        }
        else if (MustDefeatEnemiesBeforeNextWave)
        {
            if (EnemyManager.Enemies.Count <= 0)
            {
                StartNextWave();
            }
        }

        else 
        {
            yield return new WaitForSeconds(TimeBetweenWaves);
            StartNextWave();
        }
    }
    
    protected virtual bool SpawnEnemy()
    {
        if (EnemySpawnPoints.Count <= 0)
        {
            return false;
        }

        if (CurrentEnemyCount > MaxEnemiesPerWave)
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

        CurrentEnemyCount++;

        return true;
    }
    
    protected virtual bool SpawnBoss()
    {
        if (BossSpawnPoints.Count <= 0)
        {
            return false;
        }

        if (CurrentBossCount > MaxBossesPerWave)
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

        CurrentBossCount++;

        return true;
    }
}
