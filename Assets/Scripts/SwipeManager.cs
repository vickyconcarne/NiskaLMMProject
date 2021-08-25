using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    private Vector2 startPos;
    public float pixelDistToDetect = 20f;
    private bool fingerDown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(!fingerDown && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            startPos = Input.touches[0].position;
            fingerDown = true;
        }

        if (fingerDown)
        {
            if(Input.touches[0].position.x >= (startPos.x + pixelDistToDetect))
            {
                fingerDown = false;
                Debug.Log("Swipe right");
            }
            else if (Input.touches[0].position.x <= (startPos.x - pixelDistToDetect))
            {
                fingerDown = false;
                Debug.Log("Swipe right");
            }


        }
        */

        //TESTING FOR PC

        if (!fingerDown && Input.GetMouseButtonDown(0))
        {
            startPos = Input.touches[0].position;
            fingerDown = true;
        }

        if (fingerDown)
        {
            if (Input.mousePosition.x >= (startPos.x + pixelDistToDetect))
            {
                fingerDown = false;
                Debug.Log("Swipe right");
            }
            else if (Input.mousePosition.x <= (startPos.x - pixelDistToDetect))
            {
                fingerDown = false;
                Debug.Log("Swipe right");
            }

            if (fingerDown && Input.GetMouseButtonUp(0))
            {
                fingerDown = false;
            }
        }
    }
}
