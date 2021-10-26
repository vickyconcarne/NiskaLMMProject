using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    [SerializeField] private Animator m_obstacleAnimator;
    public string nameOfAnimationTrigger;
    private Collider m_collider;
    public bool canBeTriggeredByNPC = true;
    public bool addExplosionSound;
    // Start is called before the first frame update
    void Start()
    {
        if(!m_obstacleAnimator) m_obstacleAnimator = GetComponent<Animator>();
        m_collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            m_collider.enabled = false;
            RandomTileManager.instance.AddToScore(10, transform.position, false);
            if(addExplosionSound) RandomTileManager.instance.PlayRandomCarDeathSound();
            m_obstacleAnimator.SetTrigger(nameOfAnimationTrigger);
        }
        else if(canBeTriggeredByNPC && other.gameObject.tag == "OtherCar")
        {
            m_collider.enabled = false;
            RandomTileManager.instance.AddToScore(10, transform.position, false);
            if (addExplosionSound) RandomTileManager.instance.PlayRandomCarDeathSound();
            m_obstacleAnimator.SetTrigger(nameOfAnimationTrigger);
        }
    }
}
