using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("PowerUp Prefabs")]
    public GameObject healPrefab;
    public GameObject riflePrefab;
    public GameObject homingPrefab;
    public GameObject spreadshotPrefab;
    public GameObject shockwavePrefab;

    [Header("Spawn Settings")]
    public Vector3 mapMinBounds;
    public Vector3 mapMaxBounds;
    public int healPerWave = 2;
    public int maxHealPerWave = 3;

    private List<GameObject> specialPrefabs;

    void Start()
    {
        // All special powerups except Heal
        specialPrefabs = new List<GameObject>() { riflePrefab, homingPrefab, spreadshotPrefab, shockwavePrefab };
    }

    /// <summary>
    /// Call this at the start of each wave
    /// </summary>
    /// <param name="currentWave">Current wave number</param>
    /// <param name="maxEnemyTypesUnlocked">Number of enemy types unlocked for this wave</param>
    public void SpawnPowerUpsForWave(int currentWave, int maxEnemyTypesUnlocked)
    {
        Debug.Log($"[PowerUpManager] Spawning powerups for wave {currentWave}");

        // Spawn 1 special powerup if unlocked
        SpawnRandomSpecial(maxEnemyTypesUnlocked);

        // Spawn 2–3 heal powerups
        int healCount = Random.Range(healPerWave, maxHealPerWave + 1);
        for (int i = 0; i < healCount; i++)
        {
            SpawnAtRandomPosition(healPrefab);
        }
    }

    /// <summary>
    /// Spawn one special powerup if the corresponding enemy type is unlocked
    /// </summary>
    /// <param name="maxEnemyTypesUnlocked">Number of enemy types unlocked</param>
    void SpawnRandomSpecial(int maxEnemyTypesUnlocked)
    {
        // Only allow special powerups corresponding to unlocked enemy types
        List<GameObject> unlockedSpecials = new List<GameObject>();

        // Example logic: each new enemy type unlocks a powerup in order
        for (int i = 0; i < maxEnemyTypesUnlocked - 1 && i < specialPrefabs.Count; i++)
        {
            unlockedSpecials.Add(specialPrefabs[i]);
        }

        if (unlockedSpecials.Count == 0)
        {
            Debug.Log("[PowerUpManager] No special powerups unlocked yet this wave.");
            return;
        }

        GameObject prefab = unlockedSpecials[Random.Range(0, unlockedSpecials.Count)];
        SpawnAtRandomPosition(prefab);
        Debug.Log($"[PowerUpManager] Spawned special powerup: {prefab.name}");
    }

    void SpawnAtRandomPosition(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[PowerUpManager] Tried to spawn null prefab!");
            return;
        }

        Vector3 pos = new Vector3(
            Random.Range(mapMinBounds.x, mapMaxBounds.x),
            Random.Range(mapMinBounds.y, mapMaxBounds.y),
            Random.Range(mapMinBounds.z, mapMaxBounds.z)
        );

        Instantiate(prefab, pos, Quaternion.identity);
        Debug.Log($"[PowerUpManager] Spawned {prefab.name} at {pos}");
    }
}
