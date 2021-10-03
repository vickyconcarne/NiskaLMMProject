using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LMM_Movement;
using System.Linq;
using Cinemachine;
using TMPro;
public class CarCollisionManager : MonoBehaviour
{
    public actorState currentState;
    public CarController playerController;
    public Rigidbody ragDollRB;
    private Collider currentCollider;
    public float explosionForce;
    public float explosionRadius;

    [Header("Pickups")]
    public projectileActorExplosion1 bombThrower;


    [Header("Anims")]
    public CinemachineVirtualCamera cinemachineCam;
    public CinemachineBrain cinemachineBrain;
    public GameObject explosionEffect;
    public GameObject cashFlow;

    public CinemachineVirtualCamera cinematicCam;
    //Ending cinematic
    public GameObject endingCinematicPrefab;
    public CinemachineVirtualCamera endingCam;

    [Header("Niska model")]
    public Animator niskaAnimator;

    [Header("Cinemachine ")]
    public Transform deathTarget;
    public CinemachineSmoothPath circularPath;

    [Header("End screen")]
    [SerializeField] private GameObject bombCanvas;

    [SerializeField] private GameObject retryScreen;
    [SerializeField] private GameObject submitPanel;
    [SerializeField] private GameObject trackGrid;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        currentCollider = GetComponent<Collider>();
        yield return new WaitForSeconds(6f);
        StartCoroutine("GiveControl");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string currentTag = other.gameObject.tag;
        #if UNITY_EDITOR
            //Debug.Log("Collided with tag " + currentTag + " " + other.gameObject.name);
        #endif

        switch (currentTag)
        {
            case "Obstacle":
                StopPlayer(other);
                break;
            case "OtherCar":
                if(currentState == actorState.AggressiveSwerving)
                {

                    NPCCarController otherCar = other.GetComponent<NPCCarController>();
                    if(otherCar) otherCar.Kill(playerController.chosenLane);
                    niskaAnimator.SetTrigger("Cheer");
                }
                else
                {
                    StopPlayer(other);
                }
                break;
            case "BombPickup":
                bombThrower.AddBomb();
                Destroy(other.gameObject);
                break;
            case "Bullet":
                StopPlayer(other);
                Destroy(other.gameObject);
                break;
            default:
                break;
        }
    }


    private void StopPlayer(Collider other)
    {
        EventManager.TriggerEvent("PlayerDeath");
        bombCanvas.SetActive(false);
        RandomTileManager.instance.HideMoneyFill();
        niskaAnimator.gameObject.SetActive(false);
        //Cam
        //cinemachineCam.enabled = false;
        //Destroy(cinemachineCam);
        //cinemachineBrain.enabled = false;
        cinemachineCam.LookAt = deathTarget;
        cinemachineCam.Follow = deathTarget;
        cinemachineCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_Path = circularPath;
        //Boom
        cashFlow.SetActive(false);
        explosionEffect.transform.position = this.transform.position;
        explosionEffect.SetActive(true);
        explosionEffect.SetActive(true);
        playerController.canMove = false;
        currentCollider.isTrigger = false;
        ragDollRB.isKinematic = false;
        Vector3 randomizedNormal = Vector3.Normalize(transform.position - other.transform.position) + Random.Range(0.5f, 1f) * Vector3.up;
        ragDollRB.AddForce(randomizedNormal * explosionForce, ForceMode.Impulse);
        //Retry
        StartCoroutine(ReloadScreenAfterWait(5f));
    }

    public void EndlessPlayer()
    {
        EventManager.TriggerEvent("PlayerDeath");
        StartCoroutine("FinishGame");
        bombCanvas.SetActive(false);
        RandomTileManager.instance.HideMoneyFill();
        //Boom
        cashFlow.SetActive(false);
        currentCollider.enabled = false;
        //Retry
        Invoke("ActivateSubmitPanel", 15f);
    }

    public void ActivateSubmitPanel()
    {
        int maxLevel = RandomTileManager.instance.GetMaxLevels();
        int reachedLevel = RandomTileManager.instance.GetCurrentLevelIndex();
        if (reachedLevel >= (maxLevel - 1))
        {
            submitPanel.SetActive(true);
        }
    }

    public IEnumerator ReloadScreenAfterWait(float val)
    {
        
        int reachedLevel = RandomTileManager.instance.GetCurrentLevelIndex();
        
        GameObject trackInstancePrefab = Resources.Load("UI/TrackInstance") as GameObject;
        
        for (int i = 0;  i < RandomTileManager.instance.GetMaxLevels(); i++)
        {
            Level currentLevel = RandomTileManager.instance.GetLevel(i);
            string number = i+1<10 ? "0"+(i+1).ToString() : (i+1).ToString();
            string result ="";
            if (i > reachedLevel)
            {
                result = number + " - ";
                char[] trackName = currentLevel.nomDeLaTrack.ToArray();
                foreach(char c in trackName)
                {
                    if(c != ' ')
                    {
                        result += "X";
                    }
                    else
                    {
                        result += c;
                    }
                }
            }
            else
            {
                
                result = number + " - " + currentLevel.nomDeLaTrack;
            }
            var go = Instantiate(trackInstancePrefab, trackGrid.transform);
            go.GetComponentInChildren<TextMeshProUGUI>().text = result;
        }
        yield return new WaitForSeconds(val);
        retryScreen.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene("PlayScene");
    }

    /// <summary>
    /// Called after end of cinematic
    /// </summary>
    public IEnumerator GiveControl()
    {
        
        cinemachineCam.enabled = true;
        yield return new WaitForSeconds(2f);
        playerController.GiveControl();
        bombCanvas.SetActive(true);
        cinematicCam.enabled = false;
    }

    public IEnumerator FinishGame()
    {
        endingCinematicPrefab.transform.position = transform.position;
        endingCinematicPrefab.SetActive(true);
        endingCam.enabled = true;
        playerController.GiveControl(false);
        yield return new WaitForSeconds(2f);
        cinemachineCam.enabled = false;
    }

}

public enum actorState { AggressiveSwerving, Idle, Immovable};
