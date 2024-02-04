using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FPS.Scripts.AI;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using UnityEngine;

public class WaveSpawnerController : MonoBehaviour
{
    [field: SerializeField] public int WaveNumber { get; set; } = 0;
    
    [field: SerializeField] public List<Transform> EnemySpawnPoints { get; set; } = new();
    [field: SerializeField] public List<Transform> BossSpawnPoints { get; set; } = new();
    
    [field: SerializeField] public List<GameObject> EnemiesToSpawn { get; set; } = new();
    [field: SerializeField] public List<GameObject> BossesToSpawn { get; set; } = new();
    

    [field: SerializeField] public float MaxEnemiesPerWave { get; set; } = 10f;
    [field: SerializeField] public float MaxBossesPerWave { get; set; } = 3f;
    
    [field: SerializeField] public float EnemyScalingFactor { get; set; } = 1f; 
    [field: SerializeField] public float BossScalingFactor { get; set; } = 0.25f; 
    [field: SerializeField] public float TimeBetweenWaves { get; set; } = 10f;
    [field: SerializeField] public float MaxTimeBetweenWaves { get; set; } = 30f;
    
    [field: SerializeField] public float TimeBetweenSpawns { get; set; } = 1f;
    
    [field: SerializeField] public float TimeAddedBetweenWavesMultiplier { get; set; } = 15f;
    
    [field: SerializeField] public float TimeBeforeFirstWave { get; set; } = 5f;
    
    [field: SerializeField] public float TimeSinceLastWave { get; set; } = 0f;
    [field: SerializeField] public bool MustDefeatEnemiesBeforeNextWave { get; set; } = true;


    //TODO: Remove this and handle with a message instead.
    protected EnemyManager EnemyManager;
    
    protected List<Transform> UsedEnemySpawnPoints  = new();
    protected List<Transform> UsedBossSpawnPoints = new();

    protected PatrolPath PatrolPath;

    protected float EnemiesPerWave;
    protected float BossesPerWave;

    private void Awake()
    {
        EnemyManager = FindObjectOfType<EnemyManager>();
        PatrolPath = FindObjectOfType<PatrolPath>();
    }

    private void Start()
    {
        StartNextWave();
    }

    private void Update()
    {
        TimeSinceLastWave += Time.deltaTime;
    }

    private void StartNextWave()
    {
        WaveNumber++;
        
        // Adjust enemy count based on the wave number and scaling factor.
        EnemiesPerWave = Mathf.Min(Mathf.Floor(WaveNumber * EnemyScalingFactor), MaxEnemiesPerWave);
        // Adjust boss count based on the wave number and scaling factor.
        BossesPerWave = Mathf.Min(Mathf.Floor(WaveNumber * BossScalingFactor), MaxBossesPerWave);
        
        TimeBetweenWaves = Mathf.Clamp(TimeBetweenWaves + TimeAddedBetweenWavesMultiplier, 0f, MaxTimeBetweenWaves);

        UsedBossSpawnPoints.Clear();
        UsedEnemySpawnPoints.Clear();
        
        DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
        displayMessage.Message = $"Wave {WaveNumber} Incoming!";
        displayMessage.DelayBeforeDisplay = 0f;
        EventManager.Broadcast(displayMessage);
    
        TimeSinceLastWave = 0f;

        StartCoroutine(SpawnEnemies());
    }



    protected virtual IEnumerator SpawnEnemies()
    {
        if (WaveNumber == 1)
        {
            yield return new WaitForSeconds(TimeBeforeFirstWave);
        }

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

        StartCoroutine(CheckForNextWaveCondition());
        yield return new WaitForSeconds(TimeBetweenWaves);
    }

    protected IEnumerator CheckForNextWaveCondition()
    {
        // Check if all enemies are defeated to start the next wave immediately.
        while (EnemyManager.Enemies.Count > 0)
        {
            if(TimeSinceLastWave >= TimeBetweenWaves && !MustDefeatEnemiesBeforeNextWave)
                break;
            yield return new WaitForSeconds(1f);
        }

        if (MustDefeatEnemiesBeforeNextWave && EnemyManager.Enemies.Count == 0)
        {
            StartNextWave();
        }
        else
        {
            StartNextWave();
        }
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
