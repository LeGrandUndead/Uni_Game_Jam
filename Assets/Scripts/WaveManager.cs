using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveSettings
    {
        public int enemyCount = 5;
        public float spawnInterval = 1f;
        public float difficultyMultiplier = 1f;
    }

    [Header("Setup")]
    public List<GameObject> enemyPrefabs;      // Assign your enemy prefabs here
    public Transform[] spawnPoints;            // Where enemies can spawn
    public Transform player;                   // So we can assign target automatically
    public float timeBetweenWaves = 5f;

    [Header("Progression")]
    public int currentWave = 0;
    public List<WaveSettings> waveSettings;    // Define per-wave behavior in inspector

    private int enemiesAlive = 0;
    private bool waveActive = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        // Check if wave is finished
        if (waveActive && enemiesAlive <= 0)
        {
            waveActive = false;
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        if (currentWave > waveSettings.Count)
            currentWave = waveSettings.Count; // Cap difficulty

        WaveSettings settings = waveSettings[currentWave - 1];
        waveActive = true;

        for (int i = 0; i < settings.enemyCount; i++)
        {
            SpawnEnemy(settings.difficultyMultiplier);
            yield return new WaitForSeconds(settings.spawnInterval);
        }
    }

    void SpawnEnemy(float difficultyMultiplier)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Auto-assign target if it has a Pursuit script
        Pursuit pursuit = newEnemy.GetComponent<Pursuit>();
        if (pursuit != null && player != null)
        {
            pursuit.player = player;
            pursuit.baseSpeed *= difficultyMultiplier;
            pursuit.maxSpeed *= difficultyMultiplier;
        }

        enemiesAlive++;
        systeme_sante health = newEnemy.GetComponent<systeme_sante>();
        if (health != null)
        {
            health.OnDeath += () => { enemiesAlive--; };
        }
    }
}
