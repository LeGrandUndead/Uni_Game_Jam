using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner Reference")]
    public EnemySpawner spawner;

    [Header("Wave Settings")]
    public int startingEnemies = 5;
    public float spawnInterval = 0.5f;
    public float timeBetweenWaves = 5f;
    public float difficultyMultiplier = 1.2f;

    [Header("UI")]
    public Text waveText;

    [HideInInspector] public int currentWave = 0;
    private int enemiesAlive = 0;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        // Update wave UI
        if (waveText != null)
            waveText.text = $"Wave: {currentWave}";
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        int enemiesThisWave = Mathf.RoundToInt(startingEnemies * Mathf.Pow(difficultyMultiplier, currentWave - 1));

        // Gradually unlock enemy types
        int unlockedTypes = Mathf.Min(currentWave, spawner.enemyPrefabs.Length);

        for (int i = 0; i < enemiesThisWave; i++)
        {
            GameObject enemy = spawner.SpawnEnemy(unlockedTypes, currentWave);
            if (enemy != null)
            {
                enemiesAlive++;

                systeme_sante health = enemy.GetComponent<systeme_sante>();
                if (health != null)
                    health.OnDeath += () => { enemiesAlive--; };
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        // Wait until all enemies die
        yield return new WaitUntil(() => enemiesAlive <= 0);

        // Start next wave
        StartCoroutine(StartNextWave());
    }
}
