﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [Header("Spawner")]
    public GameObject player;
    public float offsetFromPlayer;
    public float cleanupDistanceFromPlayer = 200f;
    public bool isActive;

    [Header("General Hazard Settings")]
    public Bounds hazardBounds = new Bounds();
    private Vector3 hazardBoundsOffset;

    [Header("Hazard: Asteroids")]
    public List<GameObject> activeAsteroids = new List<GameObject>();
    public float minAsteroidScaleFactor;
    public float maxAsteroidScaleFactor;
    public int currentAsteroidCap = 0;
    // Raise asteroid density above 1f to increase spacing between asteroid spawns.
    public float asteroidDensity;

    [Header("Hazard: Gas Clouds")]
    public List<GameObject> activeGasClouds = new List<GameObject>();
    public float minGasCloudScaleFactor;
    public float maxGasCloudScaleFactor;
    public int currentGasCloudCap = 0;
    public float gasCloudDensity = 1;

    [Header("Hazard: Black Holes")]
    public List<GameObject> activeBlackHoles = new List<GameObject>();
    public float minBlackHoleScaleFactor;
    public float maxBlackHoleScaleFactor;
    public int currentBlackHoleCap = 0;
    public float blackHoleDensity = 1;

    [Header("Collectables: Powerups")]
    public Bounds powerupBounds = new Bounds();
    private Vector3 powerupBoundsOffset;
    public List<GameObject> activePowerups = new List<GameObject>();
    public float minPowerupFrequency;
    public float maxPowerupFrequency;
    [ReadOnly] public float timeUntilNextPowerup;

    [Header("Background Asteroids")]
    public Bounds backgroundAsteroidBounds = new Bounds();
    private Vector3 backgroundAsteroidBoundsOffset;
    public List<GameObject> activeBackgroundAsteroids = new List<GameObject>();
    public float minBackgroundScaleFactor;
    public float maxBackgroundScaleFactor;

    [Header("Collectables: Resources")]
    public List<GameObject> activeIron = new List<GameObject>();
    public List<GameObject> activeSilver = new List<GameObject>();
    public List<GameObject> activeGold = new List<GameObject>();

    [Header("Events")]
    private UnityAction playerDeathDelegate;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerController controller = player.GetComponent<PlayerController>();
        powerupBounds.extents = new Vector3(controller.maxX, powerupBounds.extents.y, powerupBounds.extents.z);
        powerupBoundsOffset = powerupBounds.center;
        backgroundAsteroidBoundsOffset = backgroundAsteroidBounds.center;
    }

    private void Start()
    {
        RegisterListeners();
        PopulateBackgroundField();
        InvokeRepeating("IncreaseAsteroidCap", 1f, 1f);
        timeUntilNextPowerup = GetNextSpawnTime();
    }

    private void Update()
    {
        SpawnAndClear();
    }

    private void RegisterListeners()
    {
        playerDeathDelegate = delegate { isActive = false; };
        EventManager.StartListening("PlayerDeath", playerDeathDelegate);
    }

    private void SpawnAndClear()
    {
        if(isActive)
        {
            UpdateAndClean();
            transform.position = (player.transform.position.z + offsetFromPlayer) * Vector3.forward;
            hazardBounds.center = transform.position;
            powerupBounds.center = transform.position + powerupBoundsOffset;
            backgroundAsteroidBounds.center = transform.position + backgroundAsteroidBoundsOffset;

            if (ObjectPooler.instance.asteroidCount > 0 && activeAsteroids.Count < currentAsteroidCap)
            {
                GameObject asteroid = ObjectPooler.instance.GetPooledAsteroid();
                Vector3 spawnPoint;
                float randomScaleFactor = Utility.GenerateRandomFloat(minAsteroidScaleFactor, maxAsteroidScaleFactor);
                asteroid.transform.localScale = Vector3.one * randomScaleFactor;
                asteroid.transform.localRotation = Quaternion.Euler(Utility.GenerateRandomVector(0f, 360f));
                Asteroid roid = asteroid.GetComponent<Asteroid>();
                Bounds roidBounds = roid.mainObject.GetComponent<MeshRenderer>().bounds;

                bool boundsCheck = false;
                int maxAttempts = 3;
                while (!boundsCheck && maxAttempts >= 0)
                {
                    spawnPoint = GetHazardSpawnPoint();
                    Collider[] list = Physics.OverlapBox(spawnPoint, roidBounds.extents * asteroidDensity);
                    if (list.Length == 0)
                    {
                        activeAsteroids.Add(asteroid);
                        asteroid.transform.position = spawnPoint;
                        asteroid.SetActive(true);
                        boundsCheck = true;
                    }
                    else
                    {
                        maxAttempts--;
                    }
                }
            }

            if (activeGasClouds.Count < currentGasCloudCap)
            {
                GameObject gasCloud = ObjectPooler.instance.GetPooledGasCloud();

                if (gasCloud != null)
                {
                    Vector3 spawnPoint = GetHazardSpawnPoint();
                    float randomScaleFactor = Utility.GenerateRandomFloat(minGasCloudScaleFactor, maxGasCloudScaleFactor);
                    activeGasClouds.Add(gasCloud);
                    gasCloud.transform.position = spawnPoint;
                    gasCloud.transform.localScale = Vector3.one * randomScaleFactor;
                    gasCloud.SetActive(true);
                }

            }

            if (activeBlackHoles.Count < currentBlackHoleCap)
            {
                GameObject blackHole = ObjectPooler.instance.GetPooledBlackHole();

                if (blackHole != null)
                {
                    Vector3 spawnPoint = GetHazardSpawnPoint();
                    float randomScaleFactor = Utility.GenerateRandomFloat(minBlackHoleScaleFactor, maxBlackHoleScaleFactor);
                    activeBlackHoles.Add(blackHole);
                    blackHole.transform.position = spawnPoint;
                    blackHole.transform.localScale = Vector3.one * randomScaleFactor;
                    blackHole.SetActive(true);
                }
            }

            if (activeBackgroundAsteroids.Count < ObjectPooler.instance.backgroundAsteroidCount)
            {
                GameObject asteroid = ObjectPooler.instance.GetPooledBackgroundAsteroid();

                if (asteroid != null)
                {
                    Vector3 spawnPoint = GetBackgroundSpawnPoint();
                    float randomScaleFactor = Utility.GenerateRandomFloat(minBackgroundScaleFactor, maxBackgroundScaleFactor);
                    activeBackgroundAsteroids.Add(asteroid);
                    asteroid.transform.position = spawnPoint;
                    asteroid.transform.localScale = Vector3.one * randomScaleFactor;
                    asteroid.transform.localRotation = Quaternion.Euler(Utility.GenerateRandomVector(0f, 360f));
                    asteroid.SetActive(true);

                }
            }

            if (activePowerups.Count < ObjectPooler.instance.powerupCount && Time.time > timeUntilNextPowerup)
            {
                GameObject powerup = ObjectPooler.instance.GetPooledPowerup();

                if (powerup != null)
                {
                    Vector3 spawnPoint = GetPowerupSpawnPoint();
                    activePowerups.Add(powerup);
                    powerup.transform.position = spawnPoint;
                    powerup.SetActive(true);
                    timeUntilNextPowerup = GetNextSpawnTime();
                }
                else
                {
                    Debug.Log("GetPooledPowerup() returned null.");
                }
            }
        }   
    }

    private Vector3 GetHazardSpawnPoint()
    {
        Vector3 spawnPoint = new Vector3(
            Random.Range(hazardBounds.center.x - hazardBounds.extents.x, hazardBounds.center.x + hazardBounds.extents.x), 
            Random.Range(hazardBounds.center.y - hazardBounds.extents.y, hazardBounds.center.y + hazardBounds.extents.y), 
            Random.Range(hazardBounds.center.z - hazardBounds.extents.z, hazardBounds.center.z + hazardBounds.extents.z)
            );
        return spawnPoint;
    }

    private Vector3 GetPowerupSpawnPoint()
    {
        Vector3 spawnPoint = new Vector3(
            Random.Range(powerupBounds.center.x - powerupBounds.extents.x, powerupBounds.center.x + powerupBounds.extents.x),
            Random.Range(powerupBounds.center.y - powerupBounds.extents.y, powerupBounds.center.y + powerupBounds.extents.y),
            Random.Range(powerupBounds.center.z - powerupBounds.extents.z, powerupBounds.center.z + powerupBounds.extents.z)
            );
        //Debug.Log("Powerup spawn point: " + spawnPoint);
        return spawnPoint;
    }

    private Vector3 GetBackgroundSpawnPoint()
    {
        Vector3 spawnPoint = new Vector3(
            Random.Range(backgroundAsteroidBounds.center.x - backgroundAsteroidBounds.extents.x, backgroundAsteroidBounds.center.x + backgroundAsteroidBounds.extents.x),
            Random.Range(backgroundAsteroidBounds.center.y - backgroundAsteroidBounds.extents.y, backgroundAsteroidBounds.center.y + backgroundAsteroidBounds.extents.y),
            Random.Range(backgroundAsteroidBounds.center.z - backgroundAsteroidBounds.extents.z, backgroundAsteroidBounds.center.z + backgroundAsteroidBounds.extents.z)
            );
        return spawnPoint;
    }

    private void IncreaseAsteroidCap()
    {
        if(currentAsteroidCap < ObjectPooler.instance.asteroidCount)
        {
            currentAsteroidCap++;
        }
    }

    private void IncreaseGasCloudCap()
    {
        if (currentGasCloudCap < ObjectPooler.instance.gasCloudCount)
        {
            currentGasCloudCap++;
        }
    }

    private void IncreaseBlackHoleCap()
    {
        if (currentBlackHoleCap < ObjectPooler.instance.blackHoleCount)
        {
            currentBlackHoleCap++;
        }
    }

    private void UpdateAndClean()
    {
        if(activeAsteroids.Count > 0)
        {
            foreach (GameObject asteroid in activeAsteroids.ToArray())
            {
                Asteroid roid = asteroid.GetComponent<Asteroid>();

                if (asteroid.transform.position.z < player.transform.position.z - cleanupDistanceFromPlayer)
                {
                    Rigidbody rb = asteroid.GetComponent<Rigidbody>();
                    activeAsteroids.Remove(asteroid);
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    roid.ResetHazard();
                    asteroid.SetActive(false);
                    roid.healthBar.Deactivate();
                }
                else
                {
                    if(roid.currentHealth < roid.maxHealth)
                    {
                        roid.healthBar.UpdatePosition(asteroid.transform);
                        float newPercentage = roid.currentHealth / roid.maxHealth;
                        roid.healthBar.UpdateHpBar(newPercentage);
                    }
                }
            }
        }
        

        if(activeGasClouds.Count > 0)
        {
            foreach (GameObject gasCloud in activeGasClouds.ToArray())
            {
                if (gasCloud.transform.position.z < player.transform.position.z - cleanupDistanceFromPlayer)
                {
                    Rigidbody rb = gasCloud.GetComponent<Rigidbody>();
                    // TO DO: Add gas cloud class
                    activeGasClouds.Remove(gasCloud);
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    // TO DO: Add gas cloud class
                    gasCloud.SetActive(false);
                }
            }
        }
        
        if(activeBlackHoles.Count > 0)
        {
            foreach (GameObject blackHole in activeBlackHoles.ToArray())
            {
                if (blackHole.transform.position.z < player.transform.position.z - cleanupDistanceFromPlayer)
                {
                    Rigidbody rb = blackHole.GetComponent<Rigidbody>();
                    // TO DO: Add black hole class
                    activeBlackHoles.Remove(blackHole);
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    // TO DO: Add black hole class
                    blackHole.SetActive(false);
                }
            }
        }
        
        if(activeBackgroundAsteroids.Count > 0)
        {
            foreach (GameObject asteroid in activeBackgroundAsteroids.ToArray())
            {
                if (asteroid.transform.position.z < player.transform.position.z - 1000f)
                {
                    activeBackgroundAsteroids.Remove(asteroid);
                    asteroid.SetActive(false);
                }
            }
        }

        if(activePowerups.Count > 0)
        {
            foreach(GameObject powerup in activePowerups.ToArray())
            {
                if(powerup.transform.position.z < player.transform.position.z - cleanupDistanceFromPlayer && !powerup.GetComponent<IPowerup>().IsActive)
                {
                    activePowerups.Remove(powerup);
                    powerup.SetActive(false);
                }
            }
        }

        if(activeIron.Count > 0)
        {
            foreach (GameObject iron in activeIron.ToArray())
            {
                if (iron.transform.position.z < player.transform.position.z - cleanupDistanceFromPlayer)
                {
                    activeIron.Remove(iron);
                    iron.SetActive(false);
                }
            }
        }

        if (activeSilver.Count > 0)
        {
            foreach (GameObject silver in activeSilver.ToArray())
            {
                if (silver.transform.position.z < player.transform.position.z - cleanupDistanceFromPlayer)
                {
                    activeSilver.Remove(silver);
                    silver.SetActive(false);
                }
            }
        }

        if (activeGold.Count > 0)
        {
            foreach (GameObject gold in activeGold.ToArray())
            {
                if (gold.transform.position.z < player.transform.position.z - cleanupDistanceFromPlayer)
                {
                    activeGold.Remove(gold);
                    gold.SetActive(false);
                }
            }
        }

    }

    private void PopulateBackgroundField()
    {
        Vector3 previousCenter = backgroundAsteroidBounds.center;
        Vector3 previousExtents = backgroundAsteroidBounds.extents;

        // Moves the spawner box to halfway between its current position and z0
        Vector3 newCenter = new Vector3(previousCenter.x, previousCenter.y, previousCenter.z / 2);
        backgroundAsteroidBounds.center = newCenter;

        // Sets the extents of the box to 1.5x so it ends up going just beyond the player (meaning background asteroids spawn slightly behind the player too)
        Vector3 newExtents = new Vector3(previousExtents.x, previousExtents.y, previousCenter.z / 1.5f);
        backgroundAsteroidBounds.extents = newExtents;

        // Refactor this later - not DRY code
        // Sets the counter to -50 so there are still some asteroids in the pool (otherwise leaves a giant gap in asteroid coverage)
        for (int i = 0; i < ObjectPooler.instance.backgroundAsteroidCount - 50; i++)
        {
            GameObject asteroid = ObjectPooler.instance.GetPooledBackgroundAsteroid();
            Vector3 spawnPoint = GetBackgroundSpawnPoint();
            float randomScaleFactor = Utility.GenerateRandomFloat(minBackgroundScaleFactor, maxBackgroundScaleFactor);
            activeBackgroundAsteroids.Add(asteroid);
            asteroid.transform.position = spawnPoint;
            asteroid.transform.localScale = Vector3.one * randomScaleFactor;
            asteroid.SetActive(true);
        }

        backgroundAsteroidBounds.center = previousCenter;
        backgroundAsteroidBounds.extents = previousExtents;
    }

    private float GetNextSpawnTime()
    {
        float nextSpawnTime = Time.time + Utility.GenerateRandomFloat(minPowerupFrequency, maxPowerupFrequency);
        //Debug.Log("Next powerup spawn time is: " + nextSpawnTime);
        return nextSpawnTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hazardBounds.center, hazardBounds.size);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(powerupBounds.center, powerupBounds.size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(backgroundAsteroidBounds.center, backgroundAsteroidBounds.size);
        Gizmos.color = Color.blue;
        foreach (GameObject asteroid in activeAsteroids)
        {
            Asteroid roid = asteroid.GetComponent<Asteroid>();
            Bounds roidBounds = roid.mainObject.GetComponent<MeshRenderer>().bounds;
            Gizmos.DrawCube(roidBounds.center, roidBounds.size * asteroidDensity);
        }


    }


}
