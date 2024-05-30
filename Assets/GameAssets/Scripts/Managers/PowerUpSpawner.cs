using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] PowerUp[] powerUpsPrefabs;
    [SerializeField] private float minSpawnTime = 5;
    [SerializeField] private float maxSpawnTime = 10;
    [SerializeField] private float minSpawnRange = 5;
    [SerializeField] private float maxSpawnRange = 10;
    [SerializeField] private float spawnRangeRight = 5;
    [SerializeField] private BikeController bikeRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnPowerUp());
    }

    IEnumerator SpawnPowerUp()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        Instantiate(powerUpsPrefabs[0],
            bikeRef.transform.position
            + bikeRef.transform.forward * Random.Range(minSpawnRange, maxSpawnRange)
            + bikeRef.transform.right * Random.Range(-spawnRangeRight, spawnRangeRight)
            + new Vector3(0,1,0), 
            Quaternion.identity);

        StartCoroutine(SpawnPowerUp());
    }
}
