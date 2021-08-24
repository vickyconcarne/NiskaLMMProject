using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;

public class RandomTileManager : MonoBehaviour
{
    public Transform playerTransform;
    [Header("Different tile prefabs")]
    public List<GameObject> tilePrefabs = new List<GameObject>();
    public List<GameObject> emptyTilePrefabs = new List<GameObject>();
    public List<Wave> possibleWaves;
    private List<GameObject> activeTiles = new List<GameObject>();
    [Header("Placement options")]
    public int seed = 43;
    public float zSpawn = 0;
    public float tileLength = 10;
    public int numberOfTilesToSpawnPerIteration = 4;

    [Header("NPC Placement")]
    public float laneDifferential = 5f;


    public int currentCountedCars;
    public int maxCountedCars;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(seed);
        for(int i = 0; i < numberOfTilesToSpawnPerIteration; i++)
        {
            if(i == 0)
            {
                SpawnTile(0);
            }
            else
            {
                SpawnTile(Random.Range(0, tilePrefabs.Count));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform.position.z - tileLength*2 > zSpawn - (numberOfTilesToSpawnPerIteration * tileLength)) //Substract initially by tile length *2 so we dont delete the first tile on start
        {
            SpawnTile(Random.Range(0, tilePrefabs.Count));
            DeleteTile();
        }
    }

    public void SpawnTile(int index)
    {
        GameObject go = Instantiate(tilePrefabs[index], transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(go);

        zSpawn += tileLength;
    }

    public void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    public void CheckCars()
    {

    }
}

[System.Serializable]
public class Wave
{
    public List<SpawnInstance> waveList = new List<SpawnInstance>();
}

[System.Serializable]
public class SpawnInstance
{
    public string ressourceID;
    public float distance;
    public bool fromTop; //Spawn from top of screen or bottom
    public lane laneToPlace;
}
