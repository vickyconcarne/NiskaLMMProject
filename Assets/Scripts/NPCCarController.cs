using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LMM_Movement
{
    public class NPCCarController : MonoBehaviour
    {
        private CharacterController m_CharacterController;
        public bool canMove = true;

        [SerializeField] private Transform carChild;

        [Header("Animation")]
        public Animator carAnimator;

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
            m_CharacterController = GetComponent<CharacterController>();
            k_movementDirection.z = m_forwardMomentum;

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

        public void Kill(lane attackingLane)
        {
            canMove = false;
            if(carAnimator) carAnimator.SetTrigger("Explode");
            Invoke("DeinstantiateAfterTime", 2f);
        }

        public void DeinstantiateAfterTime()
        {
            Destroy(this.gameObject);
        }

        private void OnCollisionEnter(Collision col)
        {
            string currentTag = col.gameObject.tag;
            Debug.Log("npc car has found " + currentTag);
            switch (currentTag)
            {
                case "Obstacle":
                    Kill(lane.middle);
                    break;
                case "OtherCar":
                    lane otherCarLane = col.collider.GetComponent<NPCCarController>().chosenLane;
                    Kill(chosenLane);
                    break;
                default:
                    break;
            }
        }
    }
}


