using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundTile;
    Vector3 nextSpawnPoint;

    /// <summary>
    /// This method is used to handle the spawning of tiles, and giving them their position in the 
    /// game scene.
    /// </summary>
    public void SpawnTile() 
    {
        GameObject temp = Instantiate(groundTile, nextSpawnPoint, Quaternion.identity);
        nextSpawnPoint = temp.transform.GetChild(1).transform.position;
    }

    /// <summary>
    /// This will spawn the initial ground tiles at the start of the game, by using a forloop 
    /// to spawn 5 tiles.
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnTile();
        }
    }
}
