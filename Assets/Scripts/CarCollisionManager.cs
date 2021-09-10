using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LMM_Movement;
using Cinemachine;
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

    [Header("Cinemachine ")]
    public Transform deathTarget;
    public CinemachineSmoothPath circularPath;

    [Header("End screen")]
    [SerializeField] private GameObject bombCanvas;

    [SerializeField] private GameObject endScreen;

    // Start is called before the first frame update
    void Start()
    {
        currentCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string currentTag = other.gameObject.tag;
        #if UNITY_EDITOR
            Debug.Log("Collided with tag " + currentTag + " " + other.gameObject.name);
        #endif

        switch (currentTag)
        {
            case "Obstacle":
                /*cinemachineCam.DestroyCinemachineComponent<CinemachineTrackedDolly>();
                cinemachineCam.LookAt = null;
                cinemachineCam.Follow = null;*/
                StopPlayer(other);
                break;
            case "OtherCar":
                if(currentState == actorState.AggressiveSwerving)
                {

                    NPCCarController otherCar = other.GetComponent<NPCCarController>();
                    if(otherCar) otherCar.Kill(playerController.chosenLane);
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
        bombCanvas.SetActive(false);
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

    public IEnumerator ReloadScreenAfterWait(float val)
    {
        yield return new WaitForSeconds(val);
        endScreen.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene("PlayScene");
    }

}

public enum actorState { AggressiveSwerving, Idle, Immovable};
