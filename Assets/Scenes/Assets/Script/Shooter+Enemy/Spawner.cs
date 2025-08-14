using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float timeBetweenSpawns = 2f;
    private float nextSpawnTime;

    public Transform[] spawnPoints;
    public GameObject normalEnemyPrefab;
    public GameObject questionEnemyPrefab;

    [Range(0f, 1f)] public float questionEnemyChance = 0.3f;
    public int maxEnemiesAlive = 10;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    public int AliveEnemyCount => aliveEnemies.Count; // Read-only property

    void Update()
    {
        // Clean up destroyed enemies
        aliveEnemies.RemoveAll(e => e == null);

        if (Time.time > nextSpawnTime && aliveEnemies.Count < maxEnemiesAlive)
        {
            nextSpawnTime = Time.time + timeBetweenSpawns;

            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject toSpawn = Random.value < questionEnemyChance
                ? questionEnemyPrefab
                : normalEnemyPrefab;

            GameObject newEnemy = Instantiate(toSpawn, randomSpawnPoint.position, Quaternion.identity);
            aliveEnemies.Add(newEnemy);
        }
    }

    // Called after loading game so spawner knows about already-existing enemies
    public void RegisterExistingEnemy(GameObject enemy)
    {
        if (!aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Add(enemy);
        }
    }
}


