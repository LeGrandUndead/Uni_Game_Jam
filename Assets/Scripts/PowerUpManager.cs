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
        specialPrefabs = new List<GameObject>() { riflePrefab, homingPrefab, spreadshotPrefab, shockwavePrefab };
    }

    public void SpawnPowerUpsForWave(int currentWave, int totalEnemyTypes)
    {
        Debug.Log($"⚡ Spawning powerups for wave {currentWave}");

        // Spawn 1 special powerup near ground
        SpawnRandomSpecialAtGround();

        // Spawn 2–3 heal powerups in the air
        int healCount = Random.Range(healPerWave, maxHealPerWave + 1);
        for (int i = 0; i < healCount; i++)
        {
            SpawnHealInAir();
        }
    }

    void SpawnRandomSpecialAtGround()
    {

        if (specialPrefabs == null || specialPrefabs.Count == 0)
        {
            Debug.Log("⚠️ No special powerups left to spawn this wave, skipping...");
            return;
        }

        GameObject prefab = specialPrefabs[Random.Range(0, specialPrefabs.Count)];

        // Center of the map
        Vector3 center = (mapMinBounds + mapMaxBounds) / 2f;
        center.y = -1.32f; // Ground level

        // Small random offset to avoid exact overlap
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
      
        Instantiate(prefab, center + offset, Quaternion.identity);
        specialPrefabs.Remove(prefab);

        Debug.Log($"✨ Spawned special powerup {prefab.name} at {center + offset}");
    }

    void SpawnHealInAir()
    {
        Vector3 pos = new Vector3(
            Random.Range(mapMinBounds.x, mapMaxBounds.x),
            Random.Range(-2f, -3f), // Slightly above ground
            Random.Range(mapMinBounds.z, mapMaxBounds.z)
        );

        Instantiate(healPrefab, pos, Quaternion.identity);
        Debug.Log($"💚 Spawned heal powerup at {pos}");
    }
}
