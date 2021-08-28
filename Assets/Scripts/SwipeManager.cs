using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;
public class SwipeManager : MonoBehaviour
{


    [Header("Player")]
    public CarController playerController;

    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 startTouch, swipeDelta;
    private bool isDragging = false;
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return SwipeLeft;  } }
    public bool SwipeRight { get { return SwipeRight;  } }
    public bool SwipeUp { get { return SwipeUp;  } }
    public bool SwipeDown { get { return SwipeDown;  } }

    public float swipeValue = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        #region Mobile Inputs

        if(Input.touches.Length > 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                isDragging = true;
                startTouch = Input.touches[0].position;
            }
            else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDragging = false;
                Reset();
            }
        }

        #endregion

        //Calculate the distance

        swipeDelta = Vector2.zero;
        if (isDragging)
        {
            if (Input.touches.Length > 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
        }

        //Did we cross the deadzone?
        if(swipeDelta.magnitude > swipeValue)
        {
            //Which direction is the swipe?
            float x = swipeDelta.x;
            float y = swipeDelta.y;
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                //Left or right
                if (x < 0) swipeLeft = true;
                else swipeRight = true;
            }
            else{
                if (y < 0) swipeDown = true;
                else swipeUp = true;
                
            }
            Reset();

            if (swipeLeft)
            {
                playerController.ProcessInput(-1);
                return;
            }
            else if (swipeRight)
            {
                playerController.ProcessInput(1);
                return;
            }
        }
        

        //TESTING FOR PC
        /*
        if (!fingerDown && Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
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
                Debug.Log("Swipe left");
            }

            if (fingerDown && Input.GetMouseButtonUp(0))
            {
                fingerDown = false;
            }
        }*/
    }
}
