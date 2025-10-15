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
            return null;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            return null;
        }


        unlockedTypes = Mathf.Clamp(unlockedTypes, 1, enemyPrefabs.Length);


        GameObject prefab = null;
        for (int attempts = 0; attempts < 10 && prefab == null; attempts++)
        {
            prefab = enemyPrefabs[Random.Range(0, unlockedTypes)];
        }

        if (prefab == null)
        {
            return null;
        }

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (spawn == null)
        {
            return null;
        }

        GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);
        if (enemy == null)
        {
            return null;
        }

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

        systeme_sante health = enemy.GetComponent<systeme_sante>();
        if (health != null)
        {
            health.Heal((currentWave - 1) * 10f);
        }

        return enemy;
    }
}
