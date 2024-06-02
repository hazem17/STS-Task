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

    public void StartSpawning()
    {
        StartCoroutine(SpawnPowerUp());
    }

    IEnumerator SpawnPowerUp()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        float randomValue = Random.value;
        int selectedPowerIndex = 0;
        if (randomValue > 0.3f)
        {
            selectedPowerIndex = 0;
        }
        else
        {
            selectedPowerIndex = 1;
        }
        PowerUp temp = Instantiate(powerUpsPrefabs[selectedPowerIndex],
            bikeRef.transform.position
            + bikeRef.transform.forward * Random.Range(minSpawnRange, maxSpawnRange)
            + bikeRef.transform.right * Random.Range(-spawnRangeRight, spawnRangeRight), 
            Quaternion.identity);

        temp.transform.position = new Vector3(temp.transform.position.x,1, temp.transform.position.z);

        StartCoroutine(SpawnPowerUp());
    }
}
