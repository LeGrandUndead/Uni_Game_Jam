using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int maxEnemies = 10;
    public Transform player;

    void Start()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);

        Pursuit pursuit = enemy.GetComponent<Pursuit>();
        if (pursuit != null)
        {
            pursuit.player = player;

            pursuit.baseSpeed = Random.Range(10f, 20f);
            pursuit.maxSpeed = Random.Range(30f, 50f);
            pursuit.jumpForce = Random.Range(4f, 7f);
            pursuit.dashForce = Random.Range(10f, 100f);
            pursuit.dashChance = Random.Range(0.2f, 0.5f);
            pursuit.teleportChance = Random.Range(0.1f, 0.3f);
        }
    }
}
