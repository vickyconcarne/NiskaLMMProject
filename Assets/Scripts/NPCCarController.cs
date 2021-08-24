using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LMM_Movement
{
    public class NPCCarController : MonoBehaviour
    {
        public CharacterController m_CharacterController;
        private Collider currentCollider;
        public bool canMove = true;

        [SerializeField] private Transform carChild;

        [Header("Animation")]
        public Animator carAnimator;
        public string explodeLeftTrigger;
        public string explodeRightTrigger;
        public string generalExplodeTrigger = "Explode";
        [Header("Movement options")]
        [SerializeField] private actorState movementState;
        [SerializeField] private float m_forwardMomentum;
        [SerializeField] private float lanePositionalDifferential;
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
            k_movementDirection.z = m_forwardMomentum;

            middleLaneLocalPos = carChild.transform.localPosition.x * Vector3.right + Vector3.forward * carChild.transform.localPosition.x;
            leftLaneLocalPos = middleLaneLocalPos - transform.right * lanePositionalDifferential;
            rightLaneLocalPos = middleLaneLocalPos + transform.right * lanePositionalDifferential;
            newLanePosition = middleLaneLocalPos;
            currentCollider = GetComponent<Collider>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (canMove) m_CharacterController.Move(k_movementDirection * Time.fixedDeltaTime);
        }

        public void Kill(lane attackingLane)
        {
            Debug.Log("Killing my truck");
            currentCollider.enabled = false;
            canMove = false;
            if (carAnimator)
            {
                switch (attackingLane)
                {
                    case lane.nolane:
                        carAnimator.SetTrigger(generalExplodeTrigger);
                        break;
                    default:
                        int differential = (int)chosenLane - (int)attackingLane;
                        if(differential < 0)
                        {
                            carAnimator.SetTrigger(explodeLeftTrigger);
                        }
                        else
                        {
                            carAnimator.SetTrigger(explodeRightTrigger);
                        }
                        break;
                }
                
            }
            Invoke("DeinstantiateAfterTime", 4f);
        }

        public void DeinstantiateAfterTime()
        {
            Destroy(m_CharacterController.gameObject);
        }

        private void OnTriggerEnter(Collider col)
        {
            string currentTag = col.gameObject.tag;
            //Debug.Log("npc car has found " + currentTag + " for " + col.gameObject.name);
            switch (currentTag)
            {
                case "Obstacle":
                    Kill(lane.nolane);
                    break;
                default:
                    break;
            }
        }
    }
}


