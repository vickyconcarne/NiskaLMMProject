using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;
public class SwerveOnDetect : AOnSideDetection
{

    public float speedToMatchPlayer;
    private float originalSpeed;
    public float swerveCooldown;
    private float currentCooldown;

    public float swerveDirection; //used only to decide which actions to do (left or right)

    [SerializeField] private float lanePositionalDifferential = 5f;

    public Animator carAnimator;
    private bool finishedLateralAction = true;
    private bool finishedAllActions = true;
    private int currentActionIndex = 0;
    [Header("Actions on detect")]
    public List<MovementAction> leftActionsToDo;
    public List<MovementAction> rightActionsToDo;
    private float movementDirection; //useful for - or + sign on movements x or y

    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = npcController.k_movementDirection.z;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SideDetection();
        ActionSequencer();

        if (isLookingFor)
        {
            Debug.DrawRay(transform.position, -transform.right * distanceToDetect, Color.yellow);
            Debug.DrawRay(transform.position, transform.right * distanceToDetect, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, -transform.right * distanceToDetect, Color.green);
            Debug.DrawRay(transform.position, transform.right * distanceToDetect, Color.green);
        }
    }

    void ActionSequencer()
    {
        if (!finishedAllActions)
        {
            if (currentCooldown >= swerveCooldown && finishedLateralAction)
            {
                switch (swerveDirection)
                {
                    //Left
                    case -1f:
                        if (currentActionIndex < leftActionsToDo.Count)
                        {
                            Interpret(leftActionsToDo[currentActionIndex]);
                            currentActionIndex += 1;
                        }
                        else
                        {
                            currentActionIndex = 0;
                            finishedAllActions = true;
                            currentCooldown = -2f;
                        }
                        break;
                    //Right
                    case 1f:
                        if (currentActionIndex < rightActionsToDo.Count)
                        {
                            Interpret(rightActionsToDo[currentActionIndex]);
                            currentActionIndex += 1;
                        }
                        else
                        {
                            currentActionIndex = 0;
                            finishedAllActions = true;
                            currentCooldown = -2f;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                currentCooldown += Time.fixedDeltaTime;
            }
        }
    }

    void Interpret(MovementAction action)
    {
        finishedLateralAction = false;
        switch (action.direction)
        {
            case lane.left:
                Debug.Log("switching lane");
                movementDirection = -1;
                StartCoroutine(SwitchXLanes(action.durationOfAction));
                break;
            case lane.right:
                Debug.Log("switching lane");
                movementDirection = 1;
                StartCoroutine(SwitchXLanes(action.durationOfAction));
                break;
            default:
                Debug.Log("changing speed");
                StartCoroutine(SwitchZLanes(action.speed, action.durationOfAction));
                break;
        }
    }

    private IEnumerator SwitchXLanes(float duration)
    {
        Debug.Log("switching lane");
        float elapsedTime = 0f;
        float startingX = transform.position.x;
        float endX = startingX + movementDirection * lanePositionalDifferential;
        npcController.movementState = actorState.AggressiveSwerving;
        while (elapsedTime < duration)
        {
            Vector3 currentPosition = transform.position;
            currentPosition.x = Mathf.SmoothStep(startingX, endX, (elapsedTime / duration));
            transform.position = currentPosition; //Just change the x
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //Snap to correct x
        Vector3 finalPosition = transform.position;
        finalPosition.x = endX;
        transform.position = finalPosition; 
        npcController.movementState = actorState.Idle;
        currentCooldown = 0f;
        finishedLateralAction = true;
        yield return null;
    }

    private IEnumerator SwitchZLanes(float speed, float duration)
    {
        
        float elapsedTime = 0f;
        npcController.movementState = actorState.AggressiveSwerving;
        SetSpeed(speed);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //SetSpeed(npcController.k_movementDirection.z);
        npcController.movementState = actorState.Idle;
        currentCooldown = 0f;
        finishedLateralAction = true;
        yield return null;
    }

    public override void LeftAction(RaycastHit hitInfo)
    {
        if (finishedAllActions)
        {
            swerveDirection = -1f;
            SetSpeed(speedToMatchPlayer);
            finishedAllActions = false;
        }
        
    }

    public override void RightAction(RaycastHit hitInfo)
    {
        if (finishedAllActions)
        {
            swerveDirection = 1f;
            SetSpeed(speedToMatchPlayer);
            finishedAllActions = false;
        }
        
    }

}

[System.Serializable]
public class MovementAction
{
    public lane direction;
    public float speed;
    public float durationOfAction;
}

