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
    public GameObject dashPrefab;
    public GameObject speedBoostPrefab;
    public GameObject turretPrefab;



    [Header("Spawn Settings")]
    public Vector3 mapMinBounds;
    public Vector3 mapMaxBounds;
    public int healPerWave = 2;
    public int maxHealPerWave = 3;

    private List<GameObject> specialPrefabs;

    void Start()
    {
        specialPrefabs = new List<GameObject>() { riflePrefab, homingPrefab, spreadshotPrefab, shockwavePrefab, dashPrefab, speedBoostPrefab, turretPrefab };
    }

    public void SpawnPowerUpsForWave(int currentWave, int totalEnemyTypes)
    {

        SpawnRandomSpecialAtGround();

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
            return;
        }

        GameObject prefab = specialPrefabs[Random.Range(0, specialPrefabs.Count)];

        Vector3 center = (mapMinBounds + mapMaxBounds) / 2f;
        center.y = -1.32f; 

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
      
        Instantiate(prefab, center + offset, Quaternion.identity);

        if (prefab != turretPrefab)
            specialPrefabs.Remove(prefab);

    }

    void SpawnHealInAir()
    {
        Vector3 pos = new Vector3(
            Random.Range(mapMinBounds.x, mapMaxBounds.x),
            Random.Range(-2f, -3f),
            Random.Range(mapMinBounds.z, mapMaxBounds.z)
        );

        Instantiate(healPrefab, pos, Quaternion.identity);
    }
}
