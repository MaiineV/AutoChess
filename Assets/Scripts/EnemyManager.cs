using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Transform> allWayPoints;

    public Transform spawnPoint;

    public GameObject enemyPrefab;

    public float spawnRate = 0;
    float actualTime = 0;

    private void Update()
    {
        actualTime += Time.deltaTime;

        if (actualTime >= spawnRate)
        {
            SpawnEnemy();
            actualTime = 0;
        }
    }

    void SpawnEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Enemy>();

        enemy.actualWaypoints = allWayPoints;
    }
}
