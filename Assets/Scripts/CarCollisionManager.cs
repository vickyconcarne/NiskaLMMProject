using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;

public class CarCollisionManager : MonoBehaviour
{
    public PlayerState currentState;
    public CarController playerController;
    public Rigidbody ragDollRB;
    private Collider currentCollider;
    public float explosionForce;
    public float explosionRadius;
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
                StopPlayer(other);
                break;
            case "OtherCar":
                if(currentState == PlayerState.AggressiveSwerving)
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
        playerController.canMove = false;
        currentCollider.isTrigger = false;
        ragDollRB.isKinematic = false;
        Vector3 randomizedNormal = Vector3.Normalize(transform.position - other.transform.position) + Random.Range(0, 1f) * Vector3.up + Random.Range(-1f, 1f) * Vector3.right;
        ragDollRB.AddForce(randomizedNormal * explosionForce, ForceMode.Impulse);
    }

}

public enum PlayerState { AggressiveSwerving, Idle};
