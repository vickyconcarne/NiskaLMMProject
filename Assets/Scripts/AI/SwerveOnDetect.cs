using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;
public class SwerveOnDetect : AOnSideDetection
{

    public float speedToMatchPlayer;

    public float shootCooldown;
    private float currentCooldown;

    public Vector3 swerveDirection;

    [SerializeField] private float lanePositionalDifferential;
    [SerializeField] private float timeToChangeLane;

    public Animator carAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SwitchLanes()
    {
        /*
        float elapsedTime = 0f;
        var startingPos = tran
        m_collisionManager.currentState = actorState.AggressiveSwerving;
        while (elapsedTime < timeToChangeLane)
        {
            Vector3 currentPosition = transform.position;
            transform.position = Vector3.Slerp(startingPos, newLanePosition, (elapsedTime / timeToChangeLane));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        carChild.localPosition = newLanePosition; //Snap to new position
        .currentState = actorState.Idle;
        finishedLateralAction = true;
        */
    }

    public override void LeftAction(RaycastHit hitInfo)
    {
        SetSpeed(speedToMatchPlayer);
    }

    public override void RightAction(RaycastHit hitInfo)
    {
        SetSpeed(speedToMatchPlayer);
    }

}

[System.Serializable]
public class MovementAction
{
    public lane direction;
    public float speed;
    public float durationOfAction;
}

