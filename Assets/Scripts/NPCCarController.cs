using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LMM_Movement
{
    public class NPCCarController : MonoBehaviour
    {
        public CharacterController m_CharacterController;
        private Collider currentCollider;
        public Collider secondaryCollider;
        public bool canMove = true;

        [SerializeField] private Transform carChild;
        [Header("Score")]
        public int scoreToAddOnDeath = 0;
        [Header("Animation")]
        public Wiggler optionalWiggler;
        public Animator carAnimator;
        public string explodeLeftTrigger;
        public string explodeRightTrigger;
        public string generalExplodeTrigger = "Explode";
        public GameObject trailRenderers;
        private float currentBrakeBlend = 0f;
        [SerializeField] private bool brakesOnPlayerDeath = false;
        const float maxTimeToBrake = 2f;
        [Header("Movement options")]

        public actorState movementState;
        [SerializeField] private float m_forwardMomentum;
        [SerializeField] private float lanePositionalDifferential;
        [SerializeField] private float timeToChangeLane;
        [SerializeField] private SwerveOnDetect possibleDetector;
        

        public lane chosenLane;
        private bool finishedLateralAction = true;
        private Vector3 newLanePosition;
        //Constants
        public Vector3 k_movementDirection;
        private float k_acceptableInputRange = 0.1f;
        public Vector3 leftLaneLocalPos;
        public Vector3 middleLaneLocalPos;
        public Vector3 rightLaneLocalPos;

        //Events
        private UnityAction playerDeathListener;
        public Renderer baseRenderer;

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
            k_movementDirection.z = m_forwardMomentum;

            middleLaneLocalPos = carChild.transform.localPosition.x * Vector3.right + Vector3.forward * carChild.transform.localPosition.x;
            leftLaneLocalPos = middleLaneLocalPos - transform.right * lanePositionalDifferential;
            rightLaneLocalPos = middleLaneLocalPos + transform.right * lanePositionalDifferential;
            newLanePosition = middleLaneLocalPos;
            currentCollider = GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (canMove) m_CharacterController.Move(k_movementDirection * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Destroy and animate based on attacking lane
        /// </summary>
        /// <param name="attackingLane"></param>
        public void Kill(lane attackingLane, bool silent = false)
        {
            RandomTileManager.instance.currentCountedCars += 1;
            RandomTileManager.instance.CheckState();
            
            currentCollider.enabled = false;
            if (secondaryCollider) secondaryCollider.enabled = false;
            m_CharacterController.detectCollisions = false;
            if(GetComponent<Rigidbody>())Destroy(this.GetComponent<Rigidbody>());
            m_CharacterController.enabled = false;
            canMove = false;
            if (possibleDetector)
            {
                possibleDetector.CallStopCoroutinesFromOutside();
                possibleDetector.warningObject.GetComponent<Animation>().enabled = false;
                possibleDetector.warningObject.SetActive(false);
            }
            if (!silent)
            {
                RandomTileManager.instance.PlayRandomCarDeathSound();
                RandomTileManager.instance.AddToScore(scoreToAddOnDeath, transform.position);
                if (carAnimator)
                {
                    switch (attackingLane)
                    {
                        case lane.nolane:
                            carAnimator.SetTrigger(generalExplodeTrigger);
                            break;
                        default:

                            int differential = (int)chosenLane + (int)attackingLane;

                            if (differential < 0)
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
            else
            {
                Destroy(m_CharacterController.gameObject);
            }
        }

        public void DeinstantiateAfterTime()
        {
            Destroy(m_CharacterController.gameObject);
        }

        private void OnTriggerEnter(Collider col)
        {
            if (!canMove) return; //Already dead, dont call it twice
            string currentTag = col.gameObject.tag;
            if (gameObject.Equals(col.gameObject)) {
                return; //Dont detect urself
            }
            //Debug.Log("npc car has found " + currentTag + " for " + col.gameObject.name);
            if (movementState != actorState.Immovable /*|| movementState != actorState.AggressiveSwerving*/)
            {
                switch (currentTag)
                {
                    case "Obstacle":
                        if (baseRenderer.isVisible) Kill(lane.nolane);
                        else Kill(lane.nolane, true);

                        break;
                    case "OtherCar":

                        if (movementState != actorState.AggressiveSwerving)
                        {
                            var carComp = col.GetComponent<NPCCarController>();
                            if (carComp)
                            {
                                if (baseRenderer.isVisible) Kill(carComp.chosenLane);
                                else Kill(carComp.chosenLane, true);

                            }
                        }
                        break;
                    default:
                        break;
                }
            }

        }

        

        private void StartBrake()
        {
            if(brakesOnPlayerDeath) StartCoroutine(Brake());
        }



        private IEnumerator Brake()
        {
            if (optionalWiggler)
            {
                optionalWiggler.framerate = 0f;
                optionalWiggler.enabled = false;
            }
            currentCollider.enabled = false;
            if (secondaryCollider) secondaryCollider.enabled = false;
            
            trailRenderers.SetActive(true);
            float timeToBrake = Random.Range(0f, maxTimeToBrake);
            float chosenBrakeBlend = Random.Range(-1f, 1f);
            float currentTime = 0f;
            while (currentTime < timeToBrake)
            {
                k_movementDirection.z = m_forwardMomentum - (m_forwardMomentum * (currentTime / timeToBrake));
                carAnimator.SetFloat("BrakeBlend", chosenBrakeBlend * (currentTime / timeToBrake));
                currentTime += Time.fixedDeltaTime;
                yield return null;
            }
            m_CharacterController.enabled = false;
            canMove = false;
            if (possibleDetector)
            {
                possibleDetector.CallStopCoroutinesFromOutside();
                possibleDetector.warningObject.GetComponent<Animation>().enabled = false;
                possibleDetector.warningObject.SetActive(false);
            }
        }

    }
}


