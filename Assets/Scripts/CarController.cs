using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LMM_Movement
{
    public class CarController : MonoBehaviour
    {
        private CharacterController m_CharacterController;
        [SerializeField] private Transform appearanceChild;

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
            

            middleLaneLocalPos = appearanceChild.transform.localPosition.x * Vector3.right + Vector3.forward * appearanceChild.transform.localPosition.x;
            leftLaneLocalPos = middleLaneLocalPos - transform.right * lanePositionalDifferential;
            rightLaneLocalPos = middleLaneLocalPos + transform.right * lanePositionalDifferential;
            newLanePosition = middleLaneLocalPos;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            m_CharacterController.Move(k_movementDirection * Time.fixedDeltaTime);
        }

        private void Update()
        {
            GetInput();
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
            if (!finishedLateralAction) { return; } //Stop processing input

            if(Mathf.Abs(currentDirection) > k_acceptableInputRange)
            {
                if(currentDirection == -1f)
                {
                    switch (chosenLane)
                    {
                        case lane.left:
                            break;
                        case lane.middle:
                            chosenLane = lane.left;
                            finishedLateralAction = false;
                            newLanePosition = leftLaneLocalPos;
                            Debug.Log("Went to left");
                            StartCoroutine(SwitchLanes());
                            break;
                        case lane.right:
                            chosenLane = lane.middle;
                            finishedLateralAction = false;
                            newLanePosition = middleLaneLocalPos;
                            Debug.Log("Went to middle");
                            StartCoroutine(SwitchLanes());
                            break;
                        default:
                            break;
                    }
                }
                else if(currentDirection == 1f)
                {
                    switch (chosenLane)
                    {
                        case lane.left:
                            chosenLane = lane.middle;
                            finishedLateralAction = false;
                            newLanePosition = middleLaneLocalPos;
                            Debug.Log("Went to middle");
                            StartCoroutine(SwitchLanes());
                            break;
                        case lane.middle:
                            chosenLane = lane.right;
                            finishedLateralAction = false;
                            newLanePosition = rightLaneLocalPos;
                            Debug.Log("Went to right");
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
            var startingPos = appearanceChild.localPosition;
            while (elapsedTime < timeToChangeLane)
            {
                appearanceChild.localPosition = Vector3.Slerp(startingPos, newLanePosition, (elapsedTime / timeToChangeLane));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            appearanceChild.localPosition = newLanePosition; //Snap to new position
            finishedLateralAction = true;
        }
    }

    public enum lane { left = -1, middle = 0, right = 1 };

}


