using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    /**
     * Detect Swipe After Releasing from screen
     */
    public bool detectSwipeOnlyAfterRelease = false;

    /**
     * Are we checking swipes periodically
     */
    private bool swipeDetectorActivated;

    /**
     * The Swipe threshold is the amount that user must move finger on screen to record a swipe
     */
    public readonly float SWIPE_THRESHOLD = 20f;

    /*
     * Tell if a swipe has occured in up, right, or left directions
     */
    public bool upSwipe;
    public bool leftSwipe;
    public bool rightSwipe;

    /**
     * Records the time that swipe occurred.
     */
    public float timeOfSwipe;

    /**
     * Records X and Y coordinates of where finger pressed down on screen.
     */
    private Vector2 fingerDown;

    /**
     * Records X and Y coordinates of where finger lifted up from screen.
     */
    private Vector2 fingerUp;
    
    /*
     * Instance of Arrow Handler Object
     */
    private ArrowHandler arrowHandler;


    /**
     * Constructor
     */
    //public SwipeDetector (ArrowHandler arrowHandler)
    //{
    //    this.arrowHandler = arrowHandler;
    //}

    // Start is called before the first frame update
    void Start()
    {
        upSwipe = false;
        leftSwipe = false;
        rightSwipe = false;
        swipeDetectorActivated = false;
    }

    // Update is called once per frame
    public void Update()
    {
        if (swipeDetectorActivated)
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

                // Detects swipe after finger is released
                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            CheckGameDetection();
        }
    }

    public void SetArrowHandler(ArrowHandler arrowHandler)
    {
        this.arrowHandler = arrowHandler;
    }

    /*
     * Checks for Vertical or Horizontal Swipe
     */
    private void CheckSwipe()
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

    /**
     * Returns the amount that finger moved vertically across screen.
     */
    private float VerticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    /**
     * Returns the amount that finer moved horizontally across screen.
     */
    private float HorizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    /////////////////////////Callback Functions/////////////////
    /**
     * Called when swipe up is detected
     */
    private void OnSwipeUp()
    {
        Debug.Log("Swipe UP");
        upSwipe = true;
        timeOfSwipe = Time.time;

        // UI
        arrowHandler.HideLeftArrow();
        arrowHandler.HideRightArrow();
        arrowHandler.ShowUpArrow();
    }

    /**
     * Called when swipe down is detected
     */
    private void OnSwipeDown()
    {
        Debug.Log("Swipe DOWN");
    }

    /**
     * Called when swipe left is detected
     */
    private void OnSwipeLeft()
    {
        Debug.Log("Swipe LEFT");
        leftSwipe = true;
        timeOfSwipe = Time.time;

        // UI
        arrowHandler.HideRightArrow();
        arrowHandler.HideUpArrow();
        arrowHandler.ShowLeftArrow();
    }

    /**
     * Called when swipe right is detected
     */
    private void OnSwipeRight()
    {
        Debug.Log("Swipe RIGHT");
        rightSwipe = true;
        timeOfSwipe = Time.time;

        // UI
        arrowHandler.HideLeftArrow();
        arrowHandler.HideUpArrow();
        arrowHandler.ShowRightArrow();
    }
    
    /**
     * Ensures that swipes are detected in a timely manner
     */
    private void CheckGameDetection()
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

    public void SetSwipeDetectorActivated(bool activated)
    {
        swipeDetectorActivated = activated;
    }
}
