using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{

    GroundSpawner groundSpawner;

    public GameObject obstaclePrefab;

    // Will call the GroundSpawner script on the start of the scene
    // Calls the SpawnObstacle method on the start of the scene
    private void Start()
    {
        groundSpawner = GameObject.FindObjectOfType<GroundSpawner>();
        SpawnObstacle();
    }
    // When the player leaves a tile; after two seconds, the tile will destroy itself. 
    // This is so the scene doesn't gain a massive amount of game objects during a long session.
    private void OnTriggerExit(Collider other)
    {
        groundSpawner.SpawnTile();
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// This will find a predetermine point - chosen at random, and then place an obstacle on it.
    /// </summary>
    void SpawnObstacle() 
    {
        int obstacleSpawnIndex = Random.Range(2, 5);
        Transform spawnPoint = transform.GetChild(obstacleSpawnIndex).transform;
        // Spawn the object
        Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity, transform);
    }

}
