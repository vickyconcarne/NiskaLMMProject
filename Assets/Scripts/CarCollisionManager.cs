using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Anims")]
    public CinemachineVirtualCamera cinemachineCam;
    public CinemachineBrain cinemachineBrain;
    public GameObject explosionEffect;
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
        Debug.Log("Collided with tag " + currentTag);
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

                    NPCCarController otherCar = GetComponent<NPCCarController>();
                    if (otherCar) otherCar.Kill(playerController.chosenLane);
                }
                else
                {
                    StopPlayer(other);
                }
                break;
            default:
                break;
        }
    }


    private void StopPlayer(Collider other)
    {
        //Cam
        cinemachineCam.enabled = false;
        Destroy(cinemachineCam);
        cinemachineBrain.enabled = false;
        //Boom
        explosionEffect.transform.position = this.transform.position;
        explosionEffect.SetActive(true);
        explosionEffect.SetActive(true);
        playerController.canMove = false;
        currentCollider.isTrigger = false;
        ragDollRB.isKinematic = false;
        Vector3 randomizedNormal = Vector3.Normalize(transform.position - other.transform.position) + Random.Range(0, 1f) * Vector3.up + Random.Range(-1f, 1f) * Vector3.right;
        ragDollRB.AddForce(randomizedNormal * explosionForce, ForceMode.Impulse);
    }

}

public enum actorState { AggressiveSwerving, Idle, Immovable};
