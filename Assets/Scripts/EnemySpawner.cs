using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("List of enemy prefabs in order of difficulty (easy → hard).")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public Transform player;

    [Header("Wave Settings")]
    public int startEnemies = 5;              // enemies in first wave
    public float timeBetweenWaves = 5f;
    public float spawnInterval = 0.3f;
    public float difficultyMultiplier = 1.2f; // enemies per wave multiplier

    private int currentWave = 1;
    private int enemiesThisWave;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        enemiesThisWave = startEnemies;
        StartCoroutine(SpawnWave());
    }

    System.Collections.IEnumerator SpawnWave()
    {
        Debug.Log($"<color=yellow>Spawning Wave {currentWave}</color>");
        int unlockedTypes = Mathf.Min(currentWave, enemyPrefabs.Length);
        // 👆 gradually unlocks new types each wave

        for (int i = 0; i < enemiesThisWave; i++)
        {
            SpawnEnemy(unlockedTypes);
            yield return new WaitForSeconds(spawnInterval);
        }

        // Wait for all enemies to die before next wave
        yield return new WaitUntil(() => activeEnemies.Count == 0);

        yield return new WaitForSeconds(timeBetweenWaves);

        // Increase difficulty and start next wave
        currentWave++;
        enemiesThisWave = Mathf.RoundToInt(enemiesThisWave * difficultyMultiplier);

        StartCoroutine(SpawnWave());
    }

    void SpawnEnemy(int unlockedTypes)
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0) return;

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = enemyPrefabs[Random.Range(0, unlockedTypes)]; // 👈 only unlocked ones
        GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);
        activeEnemies.Add(enemy);

        // --- Setup pursuit behavior ---
        Pursuit pursuit = enemy.GetComponent<Pursuit>();
        if (pursuit != null)
        {
            pursuit.player = player;

            // Slight randomization
            pursuit.baseSpeed = Random.Range(10f, 15f);
            pursuit.maxSpeed = Random.Range(15f, 20f);
            pursuit.jumpForce = Random.Range(4f, 7f);
            pursuit.dashForce = Random.Range(3f, 10f);
            pursuit.dashChance = Random.Range(0.2f, 0.5f);
            pursuit.teleportChance = Random.Range(0.1f, 0.3f);
        }

        // --- Health handling ---
        systeme_sante health = enemy.GetComponent<systeme_sante>();
        if (health != null)
        {
            // scale HP based on wave number
            float bonusHealth = (currentWave - 1) * 10f;
            health.Heal(bonusHealth);

            health.OnDeath += () =>
            {
                activeEnemies.Remove(enemy);
            };
        }
    }
}
