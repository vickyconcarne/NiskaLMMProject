using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    private Animator m_obstacleAnimator;
    public string nameOfAnimationTrigger;
    private Collider m_collider;
    // Start is called before the first frame update
    void Start()
    {
        m_obstacleAnimator = GetComponent<Animator>();
        m_collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "OtherCar" || other.gameObject.tag == "Player")
        {
            m_collider.enabled = false;
            m_obstacleAnimator.SetTrigger(nameOfAnimationTrigger);
        }
    }
}
