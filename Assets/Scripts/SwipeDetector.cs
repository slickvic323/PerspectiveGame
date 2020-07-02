using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    public bool upSwipe;
    public bool leftSwipe;
    public bool rightSwipe;
    public float timeOfSwipe;

    private ArrowHandler arrowHandler;


    public SwipeDetector (ArrowHandler arrowHandler)
    {
        this.arrowHandler = arrowHandler;
    }


    // Start is called before the first frame update
    void Start()
    {
        upSwipe = false;
        leftSwipe = false;
        rightSwipe = false;
    }

    // Update is called once per frame
    public void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            // Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            // Detects swip after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                CheckSwipe();
            }
        }

        CheckGameDetection();
    }

    void CheckSwipe()
    {
        // Check if Vertical swipe
        if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove())
        {
            if (fingerDown.y - fingerUp.y > 0)
            {
                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)
            {
                OnSwipeDown();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal Swipe
        else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove())
        {
            if (fingerDown.x - fingerUp.x > 0)
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)
            {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }

        // No movement at all
        else
        {
            
        }
    }

    float VerticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float HorizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    /////////////////////////Callback Functions/////////////////
    void OnSwipeUp()
    {
        Debug.Log("Swipe UP");
        upSwipe = true;
        timeOfSwipe = Time.time;

        // UI
        arrowHandler.ShowUpArrow();
    }
    void OnSwipeDown()
    {
        Debug.Log("Swipe DOWN");
    }
    void OnSwipeLeft()
    {
        Debug.Log("Swipe LEFT");
        leftSwipe = true;
        timeOfSwipe = Time.time;

        // UI
        arrowHandler.ShowLeftArrow();
    }
    void OnSwipeRight()
    {
        Debug.Log("Swipe RIGHT");
        rightSwipe = true;
        timeOfSwipe = Time.time;

        // UI
        arrowHandler.ShowRightArrow();
    }

    public void CheckGameDetection()
    {
        if (upSwipe || leftSwipe || rightSwipe)
        {
            // If Entire Plane did not detect the swipe within 2.0 seconds, then there is a problem
            if (Time.time - timeOfSwipe > 2.0f)
            {
                upSwipe = false;
                leftSwipe = false;
                rightSwipe = false;
            }
        }
    }
}
