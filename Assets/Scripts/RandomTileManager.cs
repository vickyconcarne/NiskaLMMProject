using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;
using System;

public class RandomTileManager : MonoBehaviour
{
    public Transform playerTransform;
    [Header("Different tile prefabs")]
    public List<GameObject> tilePrefabs = new List<GameObject>();
    public List<GameObject> emptyTilePrefabs = new List<GameObject>();
    public List<Wave> possibleWaves = new List<Wave>();
    public List<GameObject> activeTiles = new List<GameObject>();
    //public ChallengeType currentTypeStandingOn;
    [Header("Placement options")]
    public int seed = 43;
    public float zSpawn = 0;
    public float tileLength = 10;
    public int numberOfTilesToSpawnPerIteration = 4;
    public ChallengeType currentChallenge;
    [Header("NPC Placement")]
    public float maxTimeBeforeSpawn;
    public float laneDifferential = 5f;
    public float frontDifferential = 100f;
    public float backDifferential = -20f;

    private float timeBeforeSpawn;
    public int currentCountedObstacles;
    public int currentCountedCars;
    public int maxCountedCars;
    private bool spawnedCars = false;

    public static RandomTileManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (!instance) instance = this;
        currentChallenge = ChallengeType.obstacles;
        UnityEngine.Random.InitState(seed);
        for(int i = 0; i < numberOfTilesToSpawnPerIteration; i++)
        {
            if(i == 0)
            {
                SpawnSimpleTile(0);
            }
            else
            {
                SpawnObstacleTile(UnityEngine.Random.Range(0, tilePrefabs.Count));
            }
        }
        //currentCountedObstacles = numberOfTilesToSpawnPerIteration;
        currentCountedObstacles = 2;
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
                SpawnWave(UnityEngine.Random.Range(0, possibleWaves.Count));
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
                }
                break;
        }
    }

    public void SpawnWave(int index)
    {

        Wave randWave = possibleWaves[index];
        maxCountedCars = randWave.waveList.Count;
        currentCountedCars = 0;
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
        //GameObject go = Instantiate(tilePrefabs[index], transform.forward * zSpawn, transform.rotation);
        //activeTiles.Add(go);

    }
}

[System.Serializable]
public class Wave
{
    public List<SpawnPosition> waveList = new List<SpawnPosition>();
}


[System.Serializable]

public class SpawnPosition
{
    public float distance;
    public SpawnInstance instance;
}

public enum ChallengeType { obstacles = 0, cars = 1 };
