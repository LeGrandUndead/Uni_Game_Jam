using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;   // Assign enemy prefabs in order of difficulty
    public Transform[] spawnPoints;     // Spawn locations
    public Transform player;            // Target for Pursuit

    public GameObject SpawnEnemy(int unlockedTypes, int currentWave)
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: No spawn points or enemy prefabs assigned!");
            return null;
        }

        // Choose random spawn point
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Only pick among unlocked enemy types
        GameObject prefab = enemyPrefabs[Random.Range(0, Mathf.Min(unlockedTypes, enemyPrefabs.Length))];

        GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);

        // Setup Pursuit
        Pursuit pursuit = enemy.GetComponent<Pursuit>();
        if (pursuit != null && player != null)
        {
            pursuit.player = player;

            // Randomize stats slightly, scaled with wave
            pursuit.baseSpeed = Random.Range(10f, 15f) * (1f + currentWave * 0.05f);
            pursuit.maxSpeed = Random.Range(15f, 20f) * (1f + currentWave * 0.05f);
            pursuit.jumpForce = Random.Range(4f, 7f);
            pursuit.dashForce = Random.Range(3f, 10f);
            pursuit.dashChance = Random.Range(0.2f, 0.5f);
        }

        // Optional: scale health by wave
        systeme_sante health = enemy.GetComponent<systeme_sante>();
        if (health != null)
        {
            health.Heal((currentWave - 1) * 10f);
        }

        return enemy;
    }
}
