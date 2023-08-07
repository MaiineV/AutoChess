using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    //public List<Transform> allWayPoints;
    [SerializeField] private Transform _kingTransform;

    public Transform[] spawnPoint;

    public List<GameObject> enemyPrefab;

    public float spawnRate = 0;
    float actualTime = 0;

    private void Update()
    {
        actualTime += Time.deltaTime;

        if (!(actualTime >= spawnRate)) return;
        
        SpawnEnemy();
        actualTime = 0;
    }

    private void SpawnEnemy()
    {
        var randomSpawnIndex = Random.Range(0, spawnPoint.Length);
        
        Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Count)], spawnPoint[randomSpawnIndex].position, spawnPoint[randomSpawnIndex].rotation)
            .GetComponent<Enemy>().Init(_kingTransform);
    }
}
