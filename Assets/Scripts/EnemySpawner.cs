using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public Transform player;

    public GameObject SpawnEnemy(int unlockedTypes, int currentWave)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("❌ EnemySpawner: No spawn points assigned!");
            return null;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("❌ EnemySpawner: No enemy prefabs assigned!");
            return null;
        }

        // Clamp unlocked types safely
        unlockedTypes = Mathf.Clamp(unlockedTypes, 1, enemyPrefabs.Length);

        // Pick prefab safely
        GameObject prefab = null;
        for (int attempts = 0; attempts < 10 && prefab == null; attempts++)
        {
            prefab = enemyPrefabs[Random.Range(0, unlockedTypes)];
        }

        if (prefab == null)
        {
            Debug.LogWarning($"⚠️ EnemySpawner: No valid prefab found among first {unlockedTypes} types!");
            return null;
        }

        // Pick random spawn point
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (spawn == null)
        {
            Debug.LogWarning("⚠️ EnemySpawner: Random spawn point was null!");
            return null;
        }

        GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);
        if (enemy == null)
        {
            Debug.LogError($"💥 EnemySpawner: Instantiate failed for prefab {prefab.name}!");
            return null;
        }

        // Setup Pursuit
        Pursuit pursuit = enemy.GetComponent<Pursuit>();
        if (pursuit != null && player != null)
        {
            pursuit.player = player;
            pursuit.baseSpeed = Random.Range(10f, 15f) * (1f + currentWave * 0.05f);
            pursuit.maxSpeed = Random.Range(15f, 20f) * (1f + currentWave * 0.05f);
            pursuit.jumpForce = Random.Range(4f, 7f);
            pursuit.dashForce = Random.Range(3f, 10f);
            pursuit.dashChance = Random.Range(0.2f, 0.5f);
        }

        // Scale health per wave
        systeme_sante health = enemy.GetComponent<systeme_sante>();
        if (health != null)
        {
            health.Heal((currentWave - 1) * 10f);
        }

        Debug.Log($"🧟 Spawned enemy {enemy.name} for wave {currentWave} at {spawn.position}");
        return enemy;
    }
}
