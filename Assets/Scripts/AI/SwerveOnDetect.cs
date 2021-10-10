using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LMM_Movement;
public class SwerveOnDetect : AOnSideDetection
{
    [Header("Movement")]
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
    [SerializeField] private float movementDirection; //useful for - or + sign on movements x or y
    private IEnumerator currentCoroutine;
    [Header("Warning")]
    public bool activateWarningOnDetect;
    public GameObject warningObject;

    //Events

    private UnityAction playerDeathListener;

    void Awake()
    {
        playerDeathListener = new UnityAction(StartBrake);
    }

    void OnEnable()
    {
        EventManager.StartListening("PlayerDeath", playerDeathListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("PlayerDeath", playerDeathListener);
    }



    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = npcController.k_movementDirection.z;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (npcController.canMove)
        {
            if (finishedAllActions) SideDetection();
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
        
    }

    void ActionSequencer()
    {
        if (!finishedAllActions)
        {
            if (finishedLateralAction)
            {
                if (currentCooldown >= swerveCooldown)
                {
                    switch (swerveDirection)
                    {
                        //Left
                        case -1f:
                            if (currentActionIndex < leftActionsToDo.Count)
                            {
                                finishedLateralAction = false;
                                Interpret(leftActionsToDo[currentActionIndex]);
                                currentActionIndex += 1;
                            }
                            else
                            {
                                currentActionIndex = 0;
                                finishedAllActions = true;
                                currentCooldown = 0f;
                            }
                            break;
                        //Right
                        case 1f:
                            if (currentActionIndex < rightActionsToDo.Count)
                            {
                                finishedLateralAction = false;
                                Interpret(rightActionsToDo[currentActionIndex]);
                                currentActionIndex += 1;
                            }
                            else
                            {
                                currentActionIndex = 0;
                                finishedAllActions = true;
                                currentCooldown = 0f;
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
    }

    void Interpret(MovementAction action)
    {
        StopAllCoroutines();
        switch (action.direction)
        {
            case lane.left:
                //Debug.Log("switching lane");
                movementDirection = -1;
                currentCoroutine = SwitchXLanes(action.speed, action.durationOfAction);
                StartCoroutine(currentCoroutine);
                break;
            case lane.right:
                //Debug.Log("switching lane");
                movementDirection = 1;
                currentCoroutine = SwitchXLanes(action.speed, action.durationOfAction);
                StartCoroutine(currentCoroutine);
                break;
            default:
                //Debug.Log("changing speed");
                currentCoroutine = SwitchZLanes(action.speed, action.durationOfAction);
                StartCoroutine(currentCoroutine);
                break;
        }
    }

    private IEnumerator SwitchXLanes(float speed,float duration)
    {
        if (activateWarningOnDetect && npcController.canMove) warningObject.SetActive(false); //Deactivate warning
        float elapsedTime = 0f;
        float startingX = transform.position.x;
        float endX = startingX + movementDirection * lanePositionalDifferential;
        npcController.movementState = actorState.AggressiveSwerving;
        SetSpeed(speed);
        npcController.k_movementDirection.x = movementDirection * 5f;
        while (elapsedTime < duration)
        {
            
            /*Vector3 currentPosition = transform.position;
            currentPosition.x = Mathf.SmoothStep(startingX, endX, (elapsedTime / duration));
            transform.position = currentPosition;*/
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        npcController.k_movementDirection.x = 0f;
        DecideNewLane();
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

    /// <summary>
    /// Because stop all coroutines only works on script it is attached to
    /// </summary>
    public void CallStopCoroutinesFromOutside()
    {
        StopAllCoroutines();
        /*if (currentCoroutine != null) StopCoroutine(currentCoroutine);*/
    }

    void DecideNewLane()
    {
        switch (npcController.chosenLane)
        {
            case lane.left:
                if(movementDirection == 1f)
                {
                    npcController.chosenLane = lane.middle;
                }
                break;
            case lane.right:
                if (movementDirection == -1f)
                {
                    npcController.chosenLane = lane.middle;
                }
                break;
            case lane.middle:
                if (movementDirection == 1f)
                {
                    npcController.chosenLane = lane.right;
                }
                else
                {
                    npcController.chosenLane = lane.left;
                }
                break;
        }
    }

    public override void LeftAction(RaycastHit hitInfo)
    {
        if (finishedAllActions)
        {
            swerveDirection = -1f;
            SetSpeed(speedToMatchPlayer);
            finishedAllActions = false;
            if (activateWarningOnDetect && npcController.canMove) warningObject.SetActive(true);
        }
        
    }

    public override void RightAction(RaycastHit hitInfo)
    {
        if (finishedAllActions)
        {
            swerveDirection = 1f;
            SetSpeed(speedToMatchPlayer);
            finishedAllActions = false;
            if (activateWarningOnDetect && npcController.canMove) warningObject.SetActive(true);
        }
        
    }

    private void StartBrake()
    {
        StopAllCoroutines();
        if (activateWarningOnDetect && npcController.canMove) warningObject.SetActive(false); //Deactivate warning
        this.enabled = false;
    }

}

[System.Serializable]
public class MovementAction
{
    public lane direction;
    public float speed;
    public float durationOfAction;
}

