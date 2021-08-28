using UnityEngine;
using System.Collections;
using LMM_Movement;
/* THIS CODE IS JUST FOR PREVIEW AND TESTING */
public class ExplodingProjectileExplosion1 : MonoBehaviour
{

    public GameObject impactPrefab;
    public GameObject explosionPrefab;
    public float thrust;
    public float lifeDuration = 2f;
    public Rigidbody thisRigidbody;

    public GameObject particleKillGroup;
    private Collider thisCollider;

    public bool LookRotation = true;
    public bool Missile = false;
    public Transform missileTarget;
    public float projectileSpeed;
    public float projectileSpeedMultiplier;


    public bool explodeOnTimer = false;
    public float explosionTimer;
    float timer;

    // Use this for initialization
    void Start ()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        if (Missile)
        {
            missileTarget = GameObject.FindWithTag("Target").transform;
        }
        thisCollider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {

        
        if (LookRotation)
        { 
            transform.rotation = Quaternion.LookRotation(thisRigidbody.velocity);
        }
    }

    void FixedUpdate()
    {
        if(Missile)
        {
            projectileSpeed += projectileSpeed * projectileSpeedMultiplier;
            //   transform.position = Vector3.MoveTowards(transform.position, missileTarget.transform.position, 0);

            transform.LookAt(missileTarget);

            thisRigidbody.AddForce(transform.forward * projectileSpeed);
        }
        timer += Time.fixedDeltaTime;
        if (timer >= explosionTimer && explodeOnTimer == true)
        {
            Explode();
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "OtherCar")
        {
            NPCCarController otherCar = collision.gameObject.GetComponent<NPCCarController>();
            if (otherCar)
            {
                if (otherCar.canMove) otherCar.Kill(lane.nolane);
            }

            Quaternion rot = Quaternion.LookRotation(Vector3.up);
            Vector3 pos = collision.transform.position;
            Instantiate(impactPrefab, pos, rot);
            if (!explodeOnTimer && Missile == false)
            {
                Destroy(gameObject);
            }
            else if(Missile == true)
            {

                thisCollider.enabled = false;
                particleKillGroup.SetActive(false);
                thisRigidbody.velocity = Vector3.zero;

                Destroy(gameObject, 5);

            }
        }
    }

    void Explode()
    {
        Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
    
}
