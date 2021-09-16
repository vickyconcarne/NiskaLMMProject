using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;
using System;
using TMPro;

public class RandomTileManager : MonoBehaviour
{
    public Transform playerTransform;

    [Header("Levels")]
    private Level currentLevel;
    private int currentLevelIndex;
    [SerializeField] private List<Level> allLevels = new List<Level>();
    private bool finishedInitialiazingLevel = true;
    [SerializeField] private int currentIteration = 0;
    private int maxNumberOfIterations;
    private RavitailleOnDetect currentRavitaillement;

    [Header("Camera")]
    public Camera cam;
    [Header("Score")]

    public Animator scoreBox;
    public RectTransform scoreEndPoint;
    public TextMeshProUGUI highScoreText;
    public RectTransform scoreElement;
    const float timeToGoToScore = 0.5f;
    private Vector3 currentScoreOriginPoint;
    public Canvas canvasOfScore;
    private float scaleFactor;
    private float currentScoreUITime;
    private int currentHighScore;
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

    [Header("Track name & animations")]
    public Animator trackAnimator;
    public TextMeshProUGUI trackTitle;
    public AudioSource audiosource;
    //Singleton pattern

    public static RandomTileManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (!instance) instance = this;
        currentChallenge = ChallengeType.cars;
        UnityEngine.Random.InitState(seed);
        StartCoroutine(SwitchLevels(0));
        for (int i = 0; i < numberOfTilesToSpawnPerIteration; i++)
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
        
        Invoke("SpawnInitialWave", 2f);


        //Get scale factor of score canvas to correctly position
        scaleFactor = canvasOfScore.scaleFactor;


 
    }

    private void LateUpdate()
    {
        //Animator for score
        if (currentScoreUITime < timeToGoToScore) {
            scoreElement.anchoredPosition = Vector2.Lerp(currentScoreOriginPoint, scoreBox.GetComponent<RectTransform>().pivot /*scoreEndPoint.anchoredPosition*/, currentScoreUITime / timeToGoToScore);
            currentScoreUITime += Time.deltaTime;
            if(currentScoreUITime > timeToGoToScore)
            {
                scoreBox.SetTrigger("Pop");
                highScoreText.text = currentHighScore.ToString();
                scoreElement.gameObject.SetActive(false);
            }
        }
    }

    public void AddToScore(int scoreAdd, Vector3 position)
    {
        Vector2 initialPosition = cam.WorldToScreenPoint(position);
        currentScoreOriginPoint = new Vector2(initialPosition.x / scaleFactor, initialPosition.y / scaleFactor); 
        scoreElement.anchoredPosition = initialPosition;
        scoreElement.GetComponent<TextMeshProUGUI>().text = scoreAdd.ToString();
        scoreElement.gameObject.SetActive(true);
        currentHighScore += scoreAdd;
        currentScoreUITime = 0f;
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
                    AddIteration();
                }
                break;
            case ChallengeType.cars:
                if(currentCountedCars >= maxCountedCars)
                {
                    
                    currentChallenge = ChallengeType.obstacles;
                    currentCountedObstacles = numberOfTilesToSpawnPerIteration;
                    if(currentActiveCar) Destroy(currentActiveCar.gameObject, 6f); //Destroy current incarnation of cars
                    AddIteration();
                }
                break;
        }
    }

    public bool AddMoneyToLevel(int qtity, Vector3 position)
    {
        AddToScore(qtity, position);
        if (audiosource) audiosource.PlayOneShot(Resources.Load("Sounds/CashRegisterSound") as AudioClip);
        return true;
    }

    public bool AddIteration()
    {
        currentIteration += 1;
        if (currentIteration >= maxNumberOfIterations)
        {
            currentLevelIndex += 1;
            StartCoroutine(SwitchLevels(currentLevelIndex));
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator SwitchLevels(int index)
    {
        finishedInitialiazingLevel = false;
        if (currentLevelIndex <= allLevels.Count)
        {
            currentIteration = 0;
            
            currentLevel = allLevels[currentLevelIndex];
            maxNumberOfIterations = currentLevel.numberOfIterations;
            possibleWavePrefabs = currentLevel.possibleWavePrefabs;
            emptyTilePrefabs = currentLevel.emptyTilePrefabs;
            tilePrefabs = currentLevel.tilePrefabs;
            //Index the wave ressource ids so we can instantiate from prefabs
            foreach (Wave go in possibleWavePrefabs)
            {
                waveRessourceIds.Add(go.prefab.name);
            }
            
        }
        yield return new WaitForSeconds(3f);
        trackAnimator.SetTrigger("TrackAppear");
        trackTitle.text = currentLevel.nomDeLaTrack;
        if (currentLevel.ravitaillementPrefab)
        {
            if (currentRavitaillement)
            {
                currentRavitaillement.ExitLevel();
            }
            GameObject go = Instantiate(currentLevel.ravitaillementPrefab, playerTransform.position, playerTransform.rotation);
            currentRavitaillement = go.GetComponentInChildren<RavitailleOnDetect>();
        }
        yield return new WaitForSeconds(1f);
        finishedInitialiazingLevel = true;
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

    void SpawnInitialWave()
    {
        currentCountedCars = 0;
        maxCountedCars = 6;
        SpawnWave(0);
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

[System.Serializable]
public class Level
{
    public string nomDeLaTrack;
    public GameObject ravitaillementPrefab;
    public int numberOfIterations;
    public List<GameObject> tilePrefabs = new List<GameObject>();
    public List<GameObject> emptyTilePrefabs = new List<GameObject>();
    public List<Wave> possibleWavePrefabs;
}



public enum ChallengeType { obstacles = 0, cars = 1 };
