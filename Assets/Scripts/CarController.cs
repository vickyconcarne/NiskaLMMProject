using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LMM_Movement
{
    public class CarController : MonoBehaviour
    {
        private CharacterController m_CharacterController;
        [SerializeField] private CarCollisionManager m_collisionManager;
        public bool canMove = true;

        [SerializeField] private Transform carChild;

        [Header("Animation")]
        public Animator generalCarAnimator;
        

        [Header("Movement options")]
        [SerializeField] private float m_forwardMomentum;
        [SerializeField]private float lanePositionalDifferential;
        [SerializeField] private float timeToChangeLane;
        
        public lane chosenLane;
        private bool finishedLateralAction = true;
        private Vector3 newLanePosition;
        //Constants
        private Vector3 k_movementDirection;
        private float k_acceptableInputRange = 0.1f;
        public Vector3 leftLaneLocalPos;
        public Vector3 middleLaneLocalPos;
        public Vector3 rightLaneLocalPos;

        // Start is called before the first frame update
        void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            k_movementDirection.z = m_forwardMomentum;
            chosenLane = lane.middle;
            

            middleLaneLocalPos = carChild.transform.localPosition.x * Vector3.right + Vector3.forward * carChild.transform.localPosition.x;
            leftLaneLocalPos = middleLaneLocalPos - transform.right * lanePositionalDifferential;
            rightLaneLocalPos = middleLaneLocalPos + transform.right * lanePositionalDifferential;
            newLanePosition = middleLaneLocalPos;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (canMove) m_CharacterController.Move(k_movementDirection * Time.fixedDeltaTime);
        }

        private void Update()
        {
            if (canMove) GetInput();
        }

        private void GetInput()
        {
            float currentDirection = 0;
            //For now keyboard
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentDirection = -1;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentDirection = 1;
            }
            

            if(currentDirection != 0)
            {
                ProcessInput(currentDirection);
            }
            
        }

        public void ProcessInput(float currentLateralDirection)
        {
            if (!finishedLateralAction || !canMove) { return; } //Stop processing input
            if (Mathf.Abs(currentLateralDirection) > k_acceptableInputRange)
            {
                if (currentLateralDirection == -1f)
                {
                    switch (chosenLane)
                    {
                        case lane.left:
                            break;
                        case lane.middle:
                            chosenLane = lane.left;
                            finishedLateralAction = false;
                            newLanePosition = leftLaneLocalPos;
                            generalCarAnimator.SetTrigger("SwerveLeft");
                            StartCoroutine(SwitchLanes());

                            break;
                        case lane.right:
                            chosenLane = lane.middle;
                            finishedLateralAction = false;
                            newLanePosition = middleLaneLocalPos;
                            generalCarAnimator.SetTrigger("SwerveLeft");
                            StartCoroutine(SwitchLanes());
                            break;
                        default:
                            break;
                    }
                }
                else if (currentLateralDirection == 1f)
                {
                    switch (chosenLane)
                    {
                        case lane.left:
                            chosenLane = lane.middle;
                            finishedLateralAction = false;
                            newLanePosition = middleLaneLocalPos;
                            generalCarAnimator.SetTrigger("SwerveRight");
                            StartCoroutine(SwitchLanes());
                            break;
                        case lane.middle:
                            chosenLane = lane.right;
                            finishedLateralAction = false;
                            newLanePosition = rightLaneLocalPos;
                            generalCarAnimator.SetTrigger("SwerveRight");
                            StartCoroutine(SwitchLanes());
                            break;
                        case lane.right:
                            break;
                        default:
                            break;
                    }

                }
            }
        }


        private IEnumerator SwitchLanes()
        {
            float elapsedTime = 0f;
            var startingPos = carChild.localPosition;
            m_collisionManager.currentState = actorState.AggressiveSwerving;
            while (elapsedTime < timeToChangeLane)
            {
                carChild.localPosition = Vector3.Slerp(startingPos, newLanePosition, (elapsedTime / timeToChangeLane));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            carChild.localPosition = newLanePosition; //Snap to new position
            m_collisionManager.currentState = actorState.Idle;
            finishedLateralAction = true;
        }
    }

    public enum lane { left = -1, middle = 0, right = 1 , nolane = 2};

}


