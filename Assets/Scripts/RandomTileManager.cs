using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LMM_Movement;
using System;
using Cinemachine;
using TMPro;

public class RandomTileManager : MonoBehaviour
{
    public Transform playerTransform;
    public CarCollisionManager playerCollisionManager;
    [Header("Levels")]
    private Level currentLevel;
    private int currentLevelIndex;
    [SerializeField] private List<Level> allLevels = new List<Level>();
    private bool finishedInitialiazingLevel = true;
    [SerializeField] private int currentIteration = 0;
    private int maxNumberOfIterations;
    private RavitailleOnDetect currentRavitaillement;
    [SerializeField] private int currentSeed;
    [Header("Camera")]
    public Camera cam;
    [Header("Score")]

    public Animator scoreBox;
    public Animator scoreFillCircle;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI scoreMultiplierText;
    public RectTransform scoreElement;
    public TextMeshProUGUI formulaScoreText;
    const float timeToGoToScore = 0.5f;
    private Vector3 currentScoreOriginPoint;
    private float scaleFactor;
    private float currentScoreUITime;
    private int currentHighScore;
    [Header("Different tile prefabs")]
    public GameObject tileIntro;
    public List<GameObject> tilePrefabs = new List<GameObject>();
    [SerializeField] private List<string> tilePrefabsIds = new List<string>();
    public List<GameObject> emptyTilePrefabs = new List<GameObject>();
    [SerializeField] private List<string> emptyTilePrefabsIds = new List<string>();
    public List<GameObject> activeTiles = new List<GameObject>();

    [Header("Wave prefabs")]
    public List<Wave> possibleWavePrefabs;
    private List<string> waveRessourceIds = new List<string>();

    //public ChallengeType currentTypeStandingOn;
    [Header("Placement options")]
    public float zSpawn = 0;
    public float tileLength = 10;
    public int numberOfTilesToSpawnPerIteration = 4;
    public ChallengeType currentChallenge;
    private bool triggeredEnding;
    [Header("NPC Placement")]
    public float maxTimeBeforeSpawn;
    
    private float timeBeforeSpawn;
    public int currentCountedObstacles;
    public int currentCountedCars;
    public int maxCountedCars;
    [SerializeField] private bool spawnedCars = true; //Since we spawn cop cars at the start
    private GameObject currentActiveCar;
    public List<ClipWithVol> onCarKillSounds = new List<ClipWithVol>();
    [Header("Track name & animations")]
    public Animator trackAnimator;
    public TextMeshProUGUI trackTitle;
    public AudioSource audiosource;
    public AudioSource uxAudioSource;
    [SerializeField] private AudioClip mugshotSound;
    //Snapshot
    public Animator snapShotAnimator;
    public Image snapshotImage;
    
    [Header("Gimmick de voix")]
    public List<GimmickVoix> gimmickList;
    [SerializeField] private TextMeshProUGUI gimmickText;
    [SerializeField] private Animator gimmickAnimator;
    [SerializeField] private float currentMaxTimeBeforeNextGimmick;
    [SerializeField] private float timeBeforeNextGimmick;
    private int currentScoreMultiplier = 1;
    private float currentGimmickTimeFrame;
    const float k_gimmickTimeFrame = 1.5f;
    //Singleton pattern

    public static RandomTileManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (!instance) instance = this;
        currentSeed = (int)System.DateTime.Now.DayOfYear; //Get the day for seed, so we can have a modicum of randomness
        UnityEngine.Random.InitState(currentSeed);
        currentChallenge = ChallengeType.cars;
        UnityEngine.Random.InitState(currentSeed);
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
        
        Invoke("SpawnInitialWave", 7f);

    }

    public int GetMaxLevels()
    {
        return allLevels.Count;
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public Level GetLevel(int i)
    {
        return allLevels[i];
    }
    private void LateUpdate()
    {
        //Animator for score
        if (currentScoreUITime < timeToGoToScore) {
            scoreElement.transform.position = Vector2.Lerp(currentScoreOriginPoint, scoreBox.transform.position, currentScoreUITime / timeToGoToScore);
            currentScoreUITime += Time.deltaTime;
            if(currentScoreUITime > timeToGoToScore)
            {
                scoreBox.SetTrigger("Pop");
                highScoreText.text = currentHighScore.ToString();
                scoreElement.gameObject.SetActive(false);
            }
        }
    }

    public void AddToScore(int scoreAdd, Vector3 position, bool withGimmick = true)
    {
        if (withGimmick && scoreAdd > 0)
        {
            currentGimmickTimeFrame = k_gimmickTimeFrame;
            if (currentGimmickTimeFrame > 0)
            {
                currentScoreMultiplier += 1;
                //scoreMultiplierText.text = "x " + (currentScoreMultiplier);
                if (currentScoreMultiplier > 1 && timeBeforeNextGimmick > currentMaxTimeBeforeNextGimmick) CheckForGimmick(scoreAdd);
            }
        }
        Vector2 initialPosition = cam.WorldToScreenPoint(position);
        currentScoreOriginPoint = initialPosition;
        scoreElement.transform.position = initialPosition;
        //if(currentGimmickTimeFrame > 0 && currentScoreMultiplier > 1) scoreElement.GetComponent<TextMeshProUGUI>().text = (scoreAdd * currentScoreMultiplier).ToString();
        /*else*/ scoreElement.GetComponent<TextMeshProUGUI>().text = (scoreAdd).ToString();
        scoreElement.gameObject.SetActive(true);
        //if(currentGimmickTimeFrame > 0 && currentScoreMultiplier > 1) currentHighScore += scoreAdd * currentScoreMultiplier;
        /*else*/ currentHighScore += scoreAdd;
        currentScoreUITime = 0f;
    }

    public int GetScore()
    {
        return currentHighScore;
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
                //timeBeforeSpawn = 0f; 
                SpawnWave(UnityEngine.Random.Range(0, possibleWavePrefabs.Count));
                spawnedCars = true;
            }
            else
            {
                timeBeforeSpawn += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        CountdownGimmickTime();
    }

    public void SpawnObstacleTile(int index)
    {
        //GameObject go = Instantiate(tilePrefabs[index], transform.forward * zSpawn, transform.rotation);
        GameObject prefab = Resources.Load("Tiles/" + tilePrefabsIds[index]) as GameObject;
        GameObject go = Instantiate(prefab, transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(go);

        zSpawn += tileLength;
    }

    public void SpawnSimpleTile(int index)
    {
        //GameObject go = Instantiate(emptyTilePrefabs[index], transform.forward * zSpawn, transform.rotation);
        GameObject prefab = Resources.Load("Tiles/" + emptyTilePrefabsIds[index]) as GameObject;
        GameObject go = Instantiate(prefab, transform.forward * zSpawn, transform.rotation);
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
        if (triggeredEnding) return;
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
        if (triggeredEnding) return;
        switch (currentChallenge)
        {
            case ChallengeType.obstacles:
                if (currentCountedObstacles <= 0)
                {
                    currentChallenge = ChallengeType.cars;
                    spawnedCars = false;
                    timeBeforeSpawn = 0f;
                    AddIteration();
                }
                break;
            case ChallengeType.cars:
                if(currentCountedCars >= maxCountedCars)
                {
                    
                    currentChallenge = ChallengeType.obstacles;
                    currentCountedObstacles = numberOfTilesToSpawnPerIteration;
                    if (currentActiveCar)
                    {
                        var goToDestroy = currentActiveCar.gameObject; //Check if this goes into address
                        Destroy(goToDestroy, 6f); //Destroy current incarnation of cars
                    }
                    AddIteration();
                }
                break;
        }
    }

    public void PlaceMoneyFillOnPosition(Vector3 position)
    {
        Vector2 initialPosition = cam.WorldToScreenPoint(position);
        scoreFillCircle.transform.position = initialPosition;
    }

    public void HideMoneyFill()
    {
        scoreFillCircle.GetComponent<Image>().fillAmount = 0f;
        //scoreFillCircle.gameObject.SetActive(false);
    }

    public bool AddMoneyToLevel(int qtity, Vector3 position, float currentFill)
    {
        AddToScore(qtity, position, false);
        
        Vector2 initialPosition = cam.WorldToScreenPoint(position);
        scoreFillCircle.transform.position = initialPosition;
        scoreFillCircle.GetComponent<Image>().fillAmount = currentFill;
        scoreFillCircle.SetTrigger("Badump");
        if (audiosource) audiosource.PlayOneShot(Resources.Load("Sounds/CashRegisterSound") as AudioClip, 0.6f);
        return true;
    }

    /// <summary>
    /// Normally, should check if weve gotten alot of points in a short window of time
    /// </summary>
    /// <param name="qtity"></param>
    void CheckForGimmick(float qtity)
    {
        //Gimmick
        if (qtity > 0)
        {
            PlayRandomGimmick();
            timeBeforeNextGimmick = 0f;
            currentMaxTimeBeforeNextGimmick = UnityEngine.Random.Range(7f, 15f);
        }
    }

    public void PlayRandomGimmick()
    {
        var randGimmick = gimmickList[UnityEngine.Random.Range(0, gimmickList.Count)];
        gimmickText.text = randGimmick.affichage;
        
        gimmickAnimator.SetTrigger("Pop");
        if (audiosource) audiosource.PlayOneShot(randGimmick.clip,randGimmick.volume);
    }

    public void PlayRandomCarDeathSound()
    {
        var randClip = onCarKillSounds[UnityEngine.Random.Range(0, onCarKillSounds.Count)];
        if (audiosource) audiosource.PlayOneShot(randClip.clip, randClip.volume);
    }

    private void CountdownGimmickTime()
    {
        timeBeforeNextGimmick += Time.fixedDeltaTime;
        if (currentGimmickTimeFrame > 0f)
        {
            currentGimmickTimeFrame -= Time.fixedDeltaTime;
            if (currentGimmickTimeFrame <= 0f)
            {
                currentScoreMultiplier = 1;
            }
        }
        
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
        //Only keep going if there are more levels
        if (currentLevelIndex < allLevels.Count)
        {
            currentIteration = 0;
            
            currentLevel = allLevels[currentLevelIndex];
            waveRessourceIds =  new List<string>(); //Reinitialize lists
            tilePrefabsIds = new List<string>();
            emptyTilePrefabsIds = new List<string>();
            maxNumberOfIterations = currentLevel.numberOfIterations;
            possibleWavePrefabs = currentLevel.possibleWavePrefabs;
            emptyTilePrefabs = currentLevel.emptyTilePrefabs;
            tilePrefabs = currentLevel.tilePrefabs;
            //Index the wave ressource ids so we can instantiate from ressource folder
            foreach (Wave go in possibleWavePrefabs)
            {
                waveRessourceIds.Add(go.prefab.name);
            }
            foreach(GameObject prefab in tilePrefabs)
            {
                tilePrefabsIds.Add(prefab.name);
            }
            foreach(GameObject emptyprefab in emptyTilePrefabs)
            {
                emptyTilePrefabsIds.Add(emptyprefab.name);
            }
            yield return new WaitForSeconds(5f);
            trackAnimator.SetTrigger("TrackAppear");
            string number = currentLevelIndex < 9 ? "0" + (currentLevelIndex+1).ToString() : (currentLevelIndex+1).ToString();
            trackTitle.text = number + " - " + currentLevel.nomDeLaTrack;
            if (currentRavitaillement)
            {
                currentRavitaillement.canGiveMoney = false;
                HideMoneyFill();
                currentRavitaillement.ExitLevel();
            }
            if (currentLevel.ravitaillementPrefab)
            {
                GameObject go = Instantiate(currentLevel.ravitaillementPrefab, playerTransform.position, playerTransform.rotation);
                currentRavitaillement = go.GetComponentInChildren<RavitailleOnDetect>();
            }
            yield return new WaitForSeconds(1f);
            if(currentLevel.imageDeFeaturing != null)
            {
                snapshotImage.sprite = currentLevel.imageDeFeaturing;
                snapShotAnimator.SetTrigger("Show");
                uxAudioSource.PlayOneShot(mugshotSound,0.75f);
            }
        }
        else
        {
            if (!triggeredEnding)
            {
                while (WhatTypeStandingOn() != ChallengeType.cars) yield return null; //Wait till we're in the clear
                triggeredEnding = true;
                highScoreText.gameObject.SetActive(false);
                formulaScoreText.text = currentHighScore.ToString();
                if (currentRavitaillement)
                {
                    currentRavitaillement.ExitLevel(); //Tell ravitaillement to beat it
                }
                currentChallenge = ChallengeType.cars; //so that we only spawn non obstacle tiles
                playerCollisionManager.EndlessPlayer();
            }
            
        }
        
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
    public Sprite imageDeFeaturing;
    public GameObject ravitaillementPrefab;
    public int numberOfIterations;
    public List<GameObject> tilePrefabs = new List<GameObject>();
    public List<GameObject> emptyTilePrefabs = new List<GameObject>();
    public List<Wave> possibleWavePrefabs;
}

[System.Serializable]
public class ClipWithVol
{
    public AudioClip clip;
    public float volume;
}

public enum ChallengeType { obstacles = 0, cars = 1 };
