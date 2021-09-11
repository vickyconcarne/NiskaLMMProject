using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;
using System;

public class RandomTileManager : MonoBehaviour
{
    public Transform playerTransform;
    [Header("Different tile prefabs")]
    public GameObject tileIntro;
    public List<GameObject> tilePrefabs = new List<GameObject>();
    public List<GameObject> emptyTilePrefabs = new List<GameObject>();
    
    public List<GameObject> activeTiles = new List<GameObject>();

    [Header("Wave prefabs")]
    public List<Wave> possibleWavePrefabs;
    private List<string> waveRessourceIds = new List<string>();

    //public ChallengeType currentTypeStandingOn;
    [Header("Placement options")]
    public int seed = 43;
    public float zSpawn = 0;
    public float tileLength = 10;
    public int numberOfTilesToSpawnPerIteration = 4;
    public ChallengeType currentChallenge;
    [Header("NPC Placement")]
    public float maxTimeBeforeSpawn;
    
    private float timeBeforeSpawn;
    public int currentCountedObstacles;
    public int currentCountedCars;
    public int maxCountedCars;
    [SerializeField] private bool spawnedCars = true; //Since we spawn cop cars at the start
    private GameObject currentActiveCar;

    //Singleton pattern

    public static RandomTileManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (!instance) instance = this;
        currentChallenge = ChallengeType.cars;
        UnityEngine.Random.InitState(seed);
        for(int i = 0; i < numberOfTilesToSpawnPerIteration; i++)
        {
            if(i == 0)
            {
                //Spawn intro tile
                GameObject go = Instantiate(tileIntro, transform.forward * zSpawn, transform.rotation);
                activeTiles.Add(go);
                if (currentCountedObstacles > 0)
                {
                    currentCountedObstacles -= 1;
                    CheckState();
                }
                zSpawn += tileLength;
            }
            else
            {
                SpawnSimpleTile(UnityEngine.Random.Range(0, emptyTilePrefabs.Count));
            }
        }

        //Index the wave ressource ids so we can instantiate from prefabs
        foreach(Wave go in possibleWavePrefabs)
        {
            waveRessourceIds.Add(go.prefab.name);
        }
        SpawnWave(0);
        //currentCountedObstacles = numberOfTilesToSpawnPerIteration;
        //currentCountedObstacles = 2;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (playerTransform.position.z - tileLength * 2 > zSpawn - (numberOfTilesToSpawnPerIteration * tileLength)) //Substract initially by tile length *2 so we dont delete the first tile on start
        {
            SpawnObstacleTile(UnityEngine.Random.Range(0, tilePrefabs.Count));
            DeleteTile();
        }*/
        if(playerTransform.position.z - tileLength*2 > zSpawn - (numberOfTilesToSpawnPerIteration * tileLength)) //Substract initially by tile length *2 so we dont delete the first tile on start
        {
            switch (currentChallenge)
            {
                case ChallengeType.obstacles:
                    SpawnObstacleTile(UnityEngine.Random.Range(0, tilePrefabs.Count));
                    currentCountedObstacles -= 1;
                    CheckState();
                    break;
                case ChallengeType.cars:
                    SpawnSimpleTile(UnityEngine.Random.Range(0, emptyTilePrefabs.Count));
                    break;
            }
            
            DeleteTile();
        }

        if((currentChallenge == ChallengeType.cars) && !spawnedCars && (WhatTypeStandingOn() == ChallengeType.cars))
        {
            if(timeBeforeSpawn > maxTimeBeforeSpawn)
            {
                timeBeforeSpawn = 0f; 
                SpawnWave(UnityEngine.Random.Range(0, possibleWavePrefabs.Count));
                spawnedCars = true;
            }
            else
            {
                timeBeforeSpawn += Time.deltaTime;
            }
        }
    }

    public void SpawnObstacleTile(int index)
    {
        GameObject go = Instantiate(tilePrefabs[index], transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(go);

        zSpawn += tileLength;
    }

    public void SpawnSimpleTile(int index)
    {
        GameObject go = Instantiate(emptyTilePrefabs[index], transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(go);
        if(currentCountedObstacles > 0)
        {
            currentCountedObstacles -= 1;
            CheckState();
        }
        zSpawn += tileLength;
    }

    public void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    public ChallengeType WhatTypeStandingOn()
    {
        if (zSpawn < numberOfTilesToSpawnPerIteration * tileLength) {

            Debug.LogError("calling to spawn cars at start");
        }
        float currentZ = playerTransform.position.z;
        float quotient = Mathf.Round((zSpawn - currentZ)/tileLength);
        quotient = ((zSpawn - currentZ)/tileLength) - quotient != 0 ? quotient + 1 : quotient; //Rounds to the nearest integer up
        int index = activeTiles.Count - ((int)quotient - 1);
        //Debug.Log("quotient value " + quotient + " difference zSpawn-currentZ was " + (zSpawn - currentZ) + " remainder index was thus " + index);
        return activeTiles[index].GetComponent<TileType>().type;
    }

    public void CheckState()
    {
        switch (currentChallenge)
        {
            case ChallengeType.obstacles:
                if (currentCountedObstacles <= 0)
                {
                    currentChallenge = ChallengeType.cars;
                    spawnedCars = false;
                }
                break;
            case ChallengeType.cars:
                if(currentCountedCars >= maxCountedCars)
                {
                    currentChallenge = ChallengeType.obstacles;
                    currentCountedObstacles = numberOfTilesToSpawnPerIteration;
                    if(currentActiveCar) Destroy(currentActiveCar.gameObject, 6f); //Destroy current incarnation of cars
                }
                break;
        }
    }


    public void SpawnWave(int index)
    {
        Wave randWave = possibleWavePrefabs[index];
        maxCountedCars = randWave.numberOfCars;
        currentCountedCars = 0;
        GameObject prefab = Resources.Load(waveRessourceIds[index]) as GameObject;
        Vector3 spawnPosition = playerTransform.position;
        currentActiveCar = Instantiate(prefab, spawnPosition, transform.rotation);
    }
    /*
    public void SpawnWave(int index)
    {

        Wave randWave = possibleWaves[index];
        maxCountedCars = randWave.waveList.Count;
        currentCountedCars = 0;
        Debug.Log("Spawned cars");
        foreach(SpawnPosition sp in randWave.waveList)
        {
            SpawnInstance toSpawn = sp.instance;
            GameObject prefab = Resources.Load(toSpawn.ressourceID) as GameObject;
            Vector3 spawnPosition = playerTransform.position;
            if (toSpawn.fromTop)
            {
                spawnPosition += transform.forward * (frontDifferential + sp.distance);
            }
            else
            {
                spawnPosition -= transform.forward * (backDifferential + sp.distance);
            }
            switch (toSpawn.laneToPlace)
            {
                case lane.left:
                    spawnPosition -= transform.right * laneDifferential;
                    break;
                case lane.middle:
                    break;
                case lane.right:
                    spawnPosition += transform.right * laneDifferential;
                    break;
            }
            GameObject go = Instantiate(prefab, spawnPosition, transform.rotation);
        }

    }*/
}

[System.Serializable]
public class Wave
{
    public int numberOfCars;
    public GameObject prefab;
}



public enum ChallengeType { obstacles = 0, cars = 1 };
