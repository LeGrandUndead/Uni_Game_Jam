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

    public void SpawnPowerUpsForWave()
    {
        // Spawn 1 special powerup
        SpawnRandomSpecial();

        // Spawn 2–3 heal powerups
        int healCount = Random.Range(healPerWave, maxHealPerWave + 1);
        for (int i = 0; i < healCount; i++)
        {
            SpawnAtRandomPosition(healPrefab);
        }
    }

    void SpawnRandomSpecial()
    {
        GameObject prefab = specialPrefabs[Random.Range(0, specialPrefabs.Count)];
        SpawnAtRandomPosition(prefab);
    }

    void SpawnAtRandomPosition(GameObject prefab)
    {
        Vector3 pos = new Vector3(
            Random.Range(mapMinBounds.x, mapMaxBounds.x),
            Random.Range(mapMinBounds.y, mapMaxBounds.y),
            Random.Range(mapMinBounds.z, mapMaxBounds.z)
        );

        Instantiate(prefab, pos, Quaternion.identity);
    }
}
