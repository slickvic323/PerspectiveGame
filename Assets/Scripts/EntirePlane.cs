using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntirePlane : MonoBehaviour
{

    //private static GameManager myGameManager;

    private int numberPlatformsX;

    private int numberPlatformsZ;

    private List<List<Platform>> platforms;

    SwipeDetector mySwipeDetector;

    private Ball ball;

    private bool ballExists = false;

    float beforeAngle, smallerAngle, biggerAngle;

    ArrowHandler arrowHandler;

    private float lastYVelocity = 0f;

    private bool firstBounceYet = false;

    private float firstBounceVelocity = 0f;

    private bool changingPlatforms = false;
    private bool changePlatformsOnNextBounce = false;

    private Vector3 endPlatform;

    private Pattern pattern;

    private int ballMovingAlongPatternIndex;

    private bool ballWillLandOnWrongPlatform;

    private Camera mainCamera;

    private CameraInfo cameraInfo;

    private MovementUI movementUI;

    private Vector3 cameraStartPosition;
    private Vector3 cameraEndPosition;
    private float cameraSpeed = 5.0f;
    private float startTime;
    private float journeyLength;

    private float endAngle;

    private bool ballOnNewPlatform;

    private float PLATFORM_GRID_X_DISTANCE;
    private float PLATFORM_GRID_Z_DISTANCE;

    private bool aerialView;

    private bool showingPatternAnimation = false;
    private float patternAnimationPrevTriggerTime;
    private int patternAnimationIndex;

    // Moving camera from aerial to beginning
    private Vector3 cameraStartPositionAerial;
    private Vector3 cameraEndPositionAerial;
    private float aerialCameraSpeed = 5.0f;
    private float startTimeAerialTransition;
    private float journeyLengthAerialTransition;
    private bool transitioningFromAerialToInitial = false;

    // Aerial view of platform grid

    GameObject myCanvas;
    GameObject levelCompleteUI;
    GameObject levelFailUI;
    GameObject bouncesLeftUI;
    GameObject pointScoreUI;
    Text levelCompleteText;
    Text levelFailText;
    TextMeshProUGUI numBouncesLeftText;
    Text pointsText;

    private bool ballWillLandOnFinalPlatform;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.GetMode() == GameManager.Mode.new_game_setup)
        {
            GameManager.StartNewGame();
        }
        else
        {
            GameManager.StartNewLevel();
        }

        mainCamera = Camera.main;
        cameraInfo = new CameraInfo();
        aerialView = true;
        cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_Z);
        cameraInfo.SetMode(CameraInfo.STAGNANT);

        arrowHandler = new ArrowHandler();
        arrowHandler.SetUpArrows();
        mySwipeDetector = new SwipeDetector(arrowHandler);

        movementUI = new MovementUI();
        movementUI.InitializeMovementUI();

        myCanvas = GameObject.FindWithTag("InGameCanvas");
        if (myCanvas != null)
        {
            levelCompleteUI = GameObject.FindWithTag("LevelComplete");
            levelCompleteUI.SetActive(false);
            levelCompleteText = levelCompleteUI.GetComponentInChildren<Text>();

            levelFailUI = GameObject.FindWithTag("LevelFail");
            levelFailUI.SetActive(false);
            levelFailText = levelFailUI.GetComponentInChildren<Text>();

            bouncesLeftUI = GameObject.FindWithTag("BouncesLeftUI");
            numBouncesLeftText = bouncesLeftUI.GetComponentInChildren<TextMeshProUGUI>();

            pointScoreUI = GameObject.Find("PointScoreUI");
            pointsText = pointScoreUI.GetComponentInChildren<Text>();
            pointsText.text = GameManager.GetCurrentNumPoints().ToString() + " pts";
        }


        numberPlatformsX = GameManager.GetNumXPlatforms();
        numberPlatformsZ = GameManager.GetNumZPlatforms();
        PLATFORM_GRID_X_DISTANCE = numberPlatformsX + 0.7f; //TODO replace with X Dimension Value of platform
        PLATFORM_GRID_Z_DISTANCE = numberPlatformsZ + 0.7f; //TODO replace with Z Dimension Value of platform
        ballOnNewPlatform = true;

        CreatePlatforms();

        if (aerialView)
        {
            PutCamInAerialView();
        }

    }

    // Update is called once per frame
    void Update()
    {
        mySwipeDetector.Update();
        if (ballExists)
        {
            ProperBounce();
            if (!transitioningFromAerialToInitial)
            {
                UpdateThirdPersonCamera();
            }
        }

        if (showingPatternAnimation)
        {
            ShowPatternAnimation();
        }
        else if (transitioningFromAerialToInitial)
        {
            MoveCamFromAerialToStart();
        }
    }

    void CreatePlatforms()
    {
        platforms = new List<List<Platform>>();
        for (int i=0;i<numberPlatformsX;i++)
        {
            List<Platform> temp = new List<Platform>();
            for (int j=0;j<numberPlatformsZ;j++)
            {
                Platform platform = new Platform();
                platform.CreateGameObject();
                platform.SetPosition(new Vector3(i, 0, j));
                temp.Add(platform);
            }
            platforms.Add(temp);
        }

        pattern = new Pattern();
        bool createdPattern = pattern.CreatePattern(platforms, numberPlatformsX, numberPlatformsZ);
        // Called from here to prevent possible stack overflow error (Avoiding Recursion from within Pattern class)
        while (!createdPattern)
        {
            createdPattern = pattern.CreatePattern(platforms, numberPlatformsX, numberPlatformsZ);
        }

        // Now that the pattern has been decide, show the pattern animation
        showingPatternAnimation = true;
        patternAnimationPrevTriggerTime = Time.time;
        patternAnimationIndex = 0;
    }

    private void ProperBounce()
    {
        if (!firstBounceYet && lastYVelocity < 0 && ball.GetRigidbody().velocity.y > 0)
        {
            firstBounceVelocity = lastYVelocity * -1f;
            firstBounceYet = true;
        }

        if (firstBounceYet && lastYVelocity < 0 && ball.GetRigidbody().velocity.y > 0)
        {
            // Check if there are bounces remaining on this platform
            if (platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].GetNumBouncesRemaining() > 0)
            {
                platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].SubtractOneBounce();
            }
            else
            {
                BallMadeWrongMove();
            }

            //Debug.Log("Bounce: " + ball.GetRigidbody().velocity.y);
            Debug.Log("Num bounces Left : " + platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].GetNumBouncesRemaining() + "("+ ball.GetWhichPlatfromOnX() + ", " + ball.GetWhichPlatfromOnZ() + ")");
            numBouncesLeftText.text = platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].GetNumBouncesRemaining().ToString();

            ball.GetRigidbody().velocity = new Vector3(ball.GetRigidbody().velocity.x, firstBounceVelocity, ball.GetRigidbody().velocity.z);

            // Check if made incorrect move
            if (ballWillLandOnWrongPlatform)
            {
                BallMadeWrongMove();
            }
            else if (ballWillLandOnFinalPlatform)
            {
                ShowCompletedLevelText();
            }
            
            if (changePlatformsOnNextBounce && !changingPlatforms)
            {
                changePlatformsOnNextBounce = false;

                cameraStartPosition = mainCamera.transform.position;
                if (ball.GetDirectionMoving() == Ball.MOVING_FORWARD)
                {
                    switch(ball.GetDirectionFacing())
                    {
                        case Ball.FACING_POS_Z:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x, 0f, ball.GetGameObject().transform.position.z + 1));
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX(), ball.GetWhichPlatfromOnZ() + 1);
                            break;
                        case Ball.FACING_NEG_Z:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x, 0f, ball.GetGameObject().transform.position.z - 1));
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX(), ball.GetWhichPlatfromOnZ() - 1);
                            break;
                        case Ball.FACING_POS_X:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x + 1, 0f, ball.GetGameObject().transform.position.z));
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX() + 1, ball.GetWhichPlatfromOnZ());
                            break;
                        case Ball.FACING_NEG_X:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x - 1, 0f, ball.GetGameObject().transform.position.z));
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX() - 1, ball.GetWhichPlatfromOnZ());
                            break;
                        default:
                            Debug.Log("Error: direction facing not set");
                            break;
                    }
                    // Don't need to change the camera angle and ball still facing same direction
                }
                else if (ball.GetDirectionMoving() == Ball.MOVING_RIGHT)
                {
                    switch (ball.GetDirectionFacing())
                    {
                        case Ball.FACING_POS_Z:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x + 1, 0f, ball.GetGameObject().transform.position.z));
                            cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_X);
                            ball.SetDirectionFacing(Ball.FACING_POS_X);
                            beforeAngle = 0f;
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX() + 1, ball.GetWhichPlatfromOnZ());
                            break;
                        case Ball.FACING_NEG_Z:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x - 1, 0f, ball.GetGameObject().transform.position.z));
                            cameraInfo.SetDirectionFacing(CameraInfo.FACING_NEG_X);
                            ball.SetDirectionFacing(Ball.FACING_NEG_X);
                            beforeAngle = 180f;
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX() - 1, ball.GetWhichPlatfromOnZ());
                            break;
                        case Ball.FACING_POS_X:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x, 0f, ball.GetGameObject().transform.position.z - 1));
                            cameraInfo.SetDirectionFacing(CameraInfo.FACING_NEG_Z);
                            ball.SetDirectionFacing(Ball.FACING_NEG_Z);
                            beforeAngle = 90f;
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX(), ball.GetWhichPlatfromOnZ() - 1);
                            break;
                        case Ball.FACING_NEG_X:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x, 0f, ball.GetGameObject().transform.position.z + 1));
                            cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_Z);
                            ball.SetDirectionFacing(Ball.FACING_POS_Z);
                            beforeAngle = 270f;
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX(), ball.GetWhichPlatfromOnZ() + 1);
                            break;
                        default:
                            Debug.Log("Error: direction facing not set");
                            break;
                    }
                    StartCameraTransition(Ball.MOVING_RIGHT);
                }
                else if (ball.GetDirectionMoving() == Ball.MOVING_LEFT)
                {
                    switch (ball.GetDirectionFacing())
                    {
                        case Ball.FACING_POS_Z:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x - 1, 0f, ball.GetGameObject().transform.position.z));
                            cameraInfo.SetDirectionFacing(CameraInfo.FACING_NEG_X);
                            ball.SetDirectionFacing(Ball.FACING_NEG_X);
                            beforeAngle = 0f;
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX() - 1, ball.GetWhichPlatfromOnZ());
                            break;
                        case Ball.FACING_NEG_Z:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x + 1, 0f, ball.GetGameObject().transform.position.z));
                            cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_X);
                            ball.SetDirectionFacing(Ball.FACING_POS_X);
                            beforeAngle = 180f;
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX() + 1, ball.GetWhichPlatfromOnZ());
                            break;
                        case Ball.FACING_POS_X:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x, 0f, ball.GetGameObject().transform.position.z + 1));
                            cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_Z);
                            ball.SetDirectionFacing(Ball.FACING_POS_Z);
                            beforeAngle = 90f;
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX(), ball.GetWhichPlatfromOnZ() + 1);
                            break;
                        case Ball.FACING_NEG_X:
                            StartChangingPlatforms(ball.GetGameObject().transform.position, new Vector3(ball.GetGameObject().transform.position.x, 0f, ball.GetGameObject().transform.position.z - 1));
                            cameraInfo.SetDirectionFacing(CameraInfo.FACING_NEG_Z);
                            ball.SetDirectionFacing(Ball.FACING_NEG_Z);
                            beforeAngle = 270f;
                            ball.SetWhichPlatformOn(ball.GetWhichPlatfromOnX(), ball.GetWhichPlatfromOnZ() - 1);
                            break;
                        default:
                            Debug.Log("Error: direction facing not set");
                            break;
                    }
                    StartCameraTransition(Ball.MOVING_LEFT);
                }
                CheckIfValidMove();
                CalculateCameraEndPosition();
            }

            if (ballOnNewPlatform)
            {
                HandleNewPlatformBounce();
                ballOnNewPlatform = false;

                // Arrows UI. Now that ball landed on new platform, remove the arrows
                arrowHandler.HideUpArrow();
                arrowHandler.HideRightArrow();
                arrowHandler.HideLeftArrow();
            }
        }
        lastYVelocity = ball.GetRigidbody().velocity.y;

        if (changingPlatforms)
        {
            // Checks if ball has completely made it onto the next platform
            CheckIfShouldStopMoving();
        }
    }

    private void StartChangingPlatforms(Vector3 startPlatform, Vector3 endPlatform)
    {
        // Change x or z velocity so that the ball is at the next platform's x, z coordinates when it lands for next bounce

        float newXVelocity = 0f;
        float newZVelocity = 0f;
        // Moving positively in the x direction
        if (endPlatform.x > startPlatform.x)
        {
            newXVelocity = 1f;
        }
        // Moving negatively in the x direction
        else if (endPlatform.x < startPlatform.x)
        {
            newXVelocity = -1f;
        }

        // Moving positively in the z direction
        if (endPlatform.z > startPlatform.z)
        {
            newZVelocity = 1f;
        }
        else if (endPlatform.z < startPlatform.z)
        {
            newZVelocity = -1f;
        }
        ball.GetRigidbody().velocity = new Vector3(newXVelocity, ball.GetRigidbody().velocity.y, newZVelocity);

        this.endPlatform = endPlatform;
        changingPlatforms = true;

    }

    private void CheckIfShouldStopMoving()
    {
        bool movingInX = false;
        // Check if moving positively in x direction
        if (ball.GetRigidbody().velocity.x > 0f)
        {
            movingInX = true;

            if (ball.GetGameObject().transform.position.x >= endPlatform.x)
            {
                // Make ball stop moving in x direction. It has arrived
                ball.GetRigidbody().velocity = new Vector3(0f, ball.GetRigidbody().velocity.y, ball.GetRigidbody().velocity.z);

                // Set ball's x position at exactly endPlatform's x
                ball.GetGameObject().transform.position = new Vector3(endPlatform.x, ball.GetGameObject().transform.position.y, ball.GetGameObject().transform.position.z);

                // ball is not moving in x direction anymore
                movingInX = false;
            }
        }
        // Check if moving negatively in x direction
        else if (ball.GetRigidbody().velocity.x < 0f)
        {
            movingInX = true;

            if (ball.GetGameObject().transform.position.x <= endPlatform.x)
            {
                // Make ball stop moving in x direction. It has arrived
                ball.GetRigidbody().velocity = new Vector3(0f, ball.GetRigidbody().velocity.y, ball.GetRigidbody().velocity.z);

                // Set ball's x position at exactly endPlatform's x
                ball.GetGameObject().transform.position = new Vector3(endPlatform.x, ball.GetGameObject().transform.position.y, ball.GetGameObject().transform.position.z);

                // ball is not moving in x direction anymore
                movingInX = false;
            }
        }

        bool movingInZ = false;
        // Check if moving positively in z direction
        if (ball.GetRigidbody().velocity.z > 0f)
        {
            movingInZ = true;

            if (ball.GetGameObject().transform.position.z >= endPlatform.z)
            {
                // Make ball stop moving in z direction. It has arrived
                ball.GetRigidbody().velocity = new Vector3(ball.GetRigidbody().velocity.x, ball.GetRigidbody().velocity.y, 0f);

                // Set ball's x position at exactly endPlatform's z
                ball.GetGameObject().transform.position = new Vector3(ball.GetGameObject().transform.position.x, ball.GetGameObject().transform.position.y, endPlatform.z);

                // ball is not moving in z direction anymore
                movingInZ = false;
            }
        }
        // Check if moving negatively in z direction
        else if (ball.GetRigidbody().velocity.z < 0f)
        {
            movingInZ = true;

            if (ball.GetGameObject().transform.position.z <= endPlatform.z)
            {
                // Make ball stop moving in z direction. It has arrived
                ball.GetRigidbody().velocity = new Vector3(ball.GetRigidbody().velocity.x, ball.GetRigidbody().velocity.y, 0f);

                // Set ball's x position at exactly endPlatform's z
                ball.GetGameObject().transform.position = new Vector3(ball.GetGameObject().transform.position.x, ball.GetGameObject().transform.position.y, endPlatform.z);

                // ball is not moving in z direction anymore
                movingInZ = false;
            }
        }

        if (!movingInX && !movingInZ)
        {
            changingPlatforms = false;
            ball.SetDirectionMoving(Ball.NOT_MOVING);
            // Ensure that the camera is in the correct position
            transform.position = cameraEndPosition;

            // Ball has finished moving set ballOnNewPlatform to true so that the next time it bounces it handles the new platform bounce
            ballOnNewPlatform = true;

            // Iterate the index in the pattern that the ball has completed
            ballMovingAlongPatternIndex++;
        }

        // Check if moving in z direction
    }

    void CreateBall()
    {
        ball = new Ball();
        ballExists = true;

        List < Platform > patternList = pattern.GetPattern();
        // Put ball on first platform in pattern
        Platform firstPlatform = patternList[0];
        ball.SetWhichPlatformOn(firstPlatform.GetXIndex(), firstPlatform.GetZIndex());
        ball.CreateGameObject();
        platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].SetBallOnPlatform(true, false);
        ballMovingAlongPatternIndex = 1; // Start at 1 because the first move will be to the 1st index in pattern
        ballWillLandOnWrongPlatform = false;
        ballWillLandOnFinalPlatform = false;

        // Begin moving camera from aerial view to the initial position
        cameraStartPositionAerial = mainCamera.transform.position;
        cameraEndPositionAerial = new Vector3(ball.GetGameObject().transform.position.x, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z - 2.5f);
        startTimeAerialTransition = Time.time;
        journeyLengthAerialTransition = Vector3.Distance(cameraStartPositionAerial, cameraEndPositionAerial);

        transitioningFromAerialToInitial = true;
    }

    private void UpdateThirdPersonCamera()
    {
        // Always look at the ball (Don't move along the Y-Axis
        Vector3 targetPosition = new Vector3(ball.GetGameObject().transform.position.x,
                                        transform.position.y,
                                        ball.GetGameObject().transform.position.z);
        transform.LookAt(targetPosition);

        if (cameraInfo.GetMode() == CameraInfo.FORWARD_MOVE)
        {
            if (cameraInfo.GetDirectionFacing() == CameraInfo.FACING_POS_Z)
            {
                mainCamera.transform.position = new Vector3(ball.GetGameObject().transform.position.x, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z - 2.5f);
                mainCamera.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else if(cameraInfo.GetDirectionFacing() == CameraInfo.FACING_POS_X)
            {
                mainCamera.transform.position = new Vector3(ball.GetGameObject().transform.position.x - 2.5f, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z);
                mainCamera.transform.eulerAngles = new Vector3(0f, 90f, 0f);
            }
            else if (cameraInfo.GetDirectionFacing() == CameraInfo.FACING_NEG_Z)
            {
                mainCamera.transform.position = new Vector3(ball.GetGameObject().transform.position.x, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z + 2.5f);
                mainCamera.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
            else if (cameraInfo.GetDirectionFacing() == CameraInfo.FACING_NEG_X)
            {
                mainCamera.transform.position = new Vector3(ball.GetGameObject().transform.position.x + 2.5f, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z);
                mainCamera.transform.eulerAngles = new Vector3(0f, 270f, 0f);
            }
            else if (ball.GetDirectionMoving() == Ball.NOT_MOVING)
            {
                cameraInfo.SetMode(CameraInfo.STAGNANT);
            }
        }
        else if (cameraInfo.GetMode() == CameraInfo.ROTATE_MOVE)
        {
            if (ball.GetDirectionMoving() == Ball.MOVING_RIGHT || ball.GetDirectionMoving() == Ball.MOVING_LEFT)
            {
                float distCovered = (Time.time - startTime) * cameraSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(cameraStartPosition, cameraEndPosition, fractionOfJourney);
            }
            else if (ball.GetDirectionMoving() == Ball.NOT_MOVING)
            {
                cameraInfo.SetMode(CameraInfo.STAGNANT);
            }
        }
    }

    private void StartCameraTransition(int ballMovingDirection)
    {
        cameraInfo.SetMode(CameraInfo.ROTATE_MOVE);

        endAngle = 0f;
        switch (ballMovingDirection)
        {
            case Ball.MOVING_RIGHT:
                // 90 degree clockwise turn
                endAngle = cameraInfo.GetDesiredEndAngle() + 90f;
                endAngle = ZeroTo360(endAngle);
                cameraInfo.SetDesiredEndAngle(endAngle);
                break;
            case Ball.MOVING_LEFT:
                // 90 degree counterclockwise turn
                endAngle = cameraInfo.GetDesiredEndAngle() - 90f;
                endAngle = ZeroTo360(endAngle);
                cameraInfo.SetDesiredEndAngle(endAngle);
                break;
            default:
                break;
        }
        
        //smallerAngle = ZeroTo360(Mathf.Min(beforeAngle, cameraInfo.GetDesiredEndAngle()));
        //biggerAngle = ZeroTo360(Mathf.Max(beforeAngle, cameraInfo.GetDesiredEndAngle()));

        smallerAngle = Mathf.Min(beforeAngle, endAngle);
        biggerAngle = Mathf.Max(beforeAngle, endAngle);
    }

    // Returns the total number of platforms in the x direction
    public int GetNumberPlatformsX()
    {
        return numberPlatformsX;
    }

    // Returns the total number of platforms in the z direction
    public int GetNumberPlatformsZ()
    {
        return numberPlatformsZ;
    }

    public void CalculateCameraEndPosition()
    {
        if (cameraInfo.GetDirectionFacing() == CameraInfo.FACING_POS_Z)
        {
            cameraEndPosition = new Vector3(ball.GetGameObject().transform.position.x, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z - 2.5f + 1f);
        }
        else if (cameraInfo.GetDirectionFacing() == CameraInfo.FACING_POS_X)
        {
            cameraEndPosition = new Vector3(ball.GetGameObject().transform.position.x - 2.5f + 1f, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z);
        }
        else if (cameraInfo.GetDirectionFacing() == CameraInfo.FACING_NEG_Z)
        {
            cameraEndPosition = new Vector3(ball.GetGameObject().transform.position.x, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z + 2.5f - 1f);
        }
        else if (cameraInfo.GetDirectionFacing() == CameraInfo.FACING_NEG_X)
        {
            cameraEndPosition = new Vector3(ball.GetGameObject().transform.position.x + 2.5f - 1f, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z);
        }

        journeyLength = Vector3.Distance(cameraStartPosition, cameraEndPosition);
        startTime = Time.time;
    }

    private float ZeroTo360 (float originalVal)
    {
        // Zero inclusive 360 Exclusive
        if (originalVal >= 0f && originalVal < 360f)
            return originalVal;

        float newVal = originalVal;
        while (newVal<0)
        {
            newVal += 360f;
        }
        while (newVal>=360f)
        {
            newVal -= 360f;
        }
        return newVal;
    }

    private void OnGUI()
    {
        if (mySwipeDetector.upSwipe)
        {
            if (!changingPlatforms)
            {
                changePlatformsOnNextBounce = true;
                ball.SetDirectionMoving(Ball.MOVING_FORWARD);
                cameraInfo.SetMode(CameraInfo.FORWARD_MOVE);
            }
            mySwipeDetector.upSwipe = false;
        }

        if (mySwipeDetector.leftSwipe)
        {
            if (!changingPlatforms)
            {
                changePlatformsOnNextBounce = true;
                ball.SetDirectionMoving(Ball.MOVING_LEFT);
            }
            mySwipeDetector.leftSwipe = false;
        }

        if (mySwipeDetector.rightSwipe)
        {
            if (!changingPlatforms)
            {
                changePlatformsOnNextBounce = true;
                ball.SetDirectionMoving(Ball.MOVING_RIGHT);
            }
            mySwipeDetector.rightSwipe = false;
        }

        if (GUI.Button(new Rect(300, 1000, 100, 100), "FORWARD"))
        {
            Debug.Log("Clicked");
            if (!changingPlatforms)
            {
                changePlatformsOnNextBounce = true;
                ball.SetDirectionMoving(Ball.MOVING_FORWARD);
                cameraInfo.SetMode(CameraInfo.FORWARD_MOVE);
            }
        }

        if (GUI.Button(new Rect(500, 1000, 100, 100), "RIGHT"))
        {
            Debug.Log("Clicked");
            if (!changingPlatforms)
            {
                changePlatformsOnNextBounce = true;
                ball.SetDirectionMoving(Ball.MOVING_RIGHT);
            }
        }

        if (GUI.Button(new Rect(100, 1000, 100, 100), "LEFT"))
        {
            Debug.Log("Clicked");
            if (!changingPlatforms)
            {
                changePlatformsOnNextBounce = true;
                ball.SetDirectionMoving(Ball.MOVING_LEFT);
            }
        }
    }

    public void CheckIfValidMove()
    {
        // Works for square grid
        if (ballExists)
        {
            int currentX = ball.GetWhichPlatfromOnX();
            int currentZ = ball.GetWhichPlatfromOnZ();
            // Check if falling off the edge
            if (currentX > numberPlatformsX-1 || currentX < 0 || currentZ > numberPlatformsZ || currentZ < 0)
            {
                ballWillLandOnWrongPlatform = true;
                return;
            }

            // Check if landing on platform that is not in the pattern
            if (!platforms[currentX][currentZ].IsInPattern())
            {
                ballWillLandOnWrongPlatform = true;
                return;
            }
            else
            {
                // Check if landing on platform that is in the pattern but not in the correct order
                List<Platform> patternList = pattern.GetPattern();
                Platform correctPlatform = patternList[ballMovingAlongPatternIndex];
                int correctX = correctPlatform.GetXIndex();
                int correctZ = correctPlatform.GetZIndex();
                if (correctX != currentX || correctZ != currentZ)
                {
                    ballWillLandOnWrongPlatform = true;
                    return;
                }
            }


            ballWillLandOnWrongPlatform = false;
            // Now that it is established that the CORRECT move was made, we can check if the player made it to the final platform of level
            CheckIfMadeCorrectFinalMove(currentX, currentZ);

            // Since CORRECT move was made, add points for correct move
            GameManager.ForwardMove();
            pointsText.text = GameManager.GetCurrentNumPoints().ToString() + " pts";
        }
    }

    public void CheckIfMadeCorrectFinalMove(int currentX, int currentZ)
    {
        List<Platform> patternList = pattern.GetPattern();
        // 1. Check if the correct total number of moves were made
        if (ballMovingAlongPatternIndex == patternList.Count-1)
        {
            // 2. Check if the platform the ball is moving to is the final platform
            Platform correctPlatform = patternList[ballMovingAlongPatternIndex];
            int correctX = correctPlatform.GetXIndex();
            int correctZ = correctPlatform.GetZIndex();
            if (correctX == currentX || correctZ == currentZ)
            {
                // The ball is moving correctly to the final platform of the level
                ballWillLandOnFinalPlatform = true;
            }
        }

    }

    public void HandleNewPlatformBounce()
    {
        // Set New Platform to have ball on platform
        platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].SetBallOnPlatform(true, ballWillLandOnWrongPlatform);

        // Set previous platform the ball was on to false for ballOnPlatform attribute
        platforms[ball.GetPreviousPlatformX()][ball.GetPreviousPlatformZ()].SetBallOnPlatform(false, ballWillLandOnWrongPlatform);
    }

    public void PutCamInAerialView()
    {
        mainCamera.transform.position = new Vector3((numberPlatformsX /2f) - 0.5f, PLATFORM_GRID_X_DISTANCE + (0.6f*numberPlatformsX) , numberPlatformsZ / 2f - 0.5f);
        mainCamera.transform.eulerAngles = new Vector3(90f, 0f, 0f);
    }

    public void MoveCamFromAerialToStart()
    {
        // Deals with camera's position
        float distCovered = (Time.time - startTimeAerialTransition) * aerialCameraSpeed;
        float fractionOfJourney = distCovered / journeyLengthAerialTransition;
        transform.position = Vector3.Lerp(cameraStartPositionAerial, cameraEndPositionAerial, fractionOfJourney);

        // Deals with camera's angle
        Vector3 angle1 = new Vector3(90f, 0f, 0f);
        Vector3 angle2 = new Vector3(0f, 0f, 0f);
        transform.eulerAngles = Vector3.Lerp(angle1, angle2, fractionOfJourney);

        //Vector3 targetPosition = new Vector3(ball.GetGameObject().transform.position.x,
        //                        ball.GetGameObject().transform.position.y,
        //                        ball.GetGameObject().transform.position.z);
        //transform.LookAt(targetPosition, Vector3.forward);

        // Check if moving camera to initial point is done
        if (transform.position == cameraEndPositionAerial)
        {
            transitioningFromAerialToInitial = false;

        }
    }

    public void BallMadeWrongMove()
    {
        // Set all of the platforms' colors to red
        for (int i=0;i<numberPlatformsX;i++)
        {
            for (int j=0;j<numberPlatformsZ;j++)
            {
                platforms[i][j].GetGameObject().GetComponent<Renderer>().material.color = Color.red;
            }
        }

        // Show Fail UI
        levelFailText.text = "You Made the Wrong Move";
        levelFailUI.SetActive(true);

        ResetOnFail();
        GameManager.SetMode(GameManager.Mode.failed_level);
    }

    public void ResetOnFail()
    {
        GameManager.numLives--;
        if (GameManager.numLives > 0)
        {
            //ballExists = false;
            //mainCamera = Camera.main;
            //cameraInfo = new CameraInfo();
            //aerialView = true;
            //cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_Z);
            //cameraInfo.SetMode(CameraInfo.STAGNANT);

            //mySwipeDetector = new SwipeDetector();
            //arrowHandler = new ArrowHandler();
            //arrowHandler.SetUpArrow(true);

            //movementUI = new MovementUI();
            //movementUI.InitializeMovementUI();

            //numberPlatformsX = 4;
            //numberPlatformsZ = 4;
            //PLATFORM_GRID_X_DISTANCE = numberPlatformsX + 0.7f; //TODO replace with X Dimension Value of platform
            //PLATFORM_GRID_Z_DISTANCE = numberPlatformsZ + 0.7f; //TODO replace with Z Dimension Value of platform
            //ballOnNewPlatform = true;

            //CreatePlatforms();

            //if (aerialView)
            //{
            //    PutCamInAerialView();
            //}
        }
        else
        {
            Debug.Log("GAME OVER");
        }
    }

    public void ShowPatternAnimation()
    {
        float currentTime = Time.time;
        // If it has been at least 1.2 seconds since the previous platform in the pattern turned blue for the animation
        if (currentTime - patternAnimationPrevTriggerTime >= 1.2f)
        {
            List<Platform> patternList = pattern.GetPattern();
            if (patternAnimationIndex < patternList.Count)
            {
                Platform nextPlatformInAnimation = patternList[patternAnimationIndex];
                nextPlatformInAnimation.GetGameObject().GetComponent<Renderer>().material.color = Color.blue;
                patternAnimationIndex++;
                patternAnimationPrevTriggerTime = Time.time;
            }
            else
            {
                // Reached the end of the animation
                showingPatternAnimation = false;

                // Create the Ball once the pattern animation has finished
                CreateBall();
            }
        }
    }

    public void ShowCompletedLevelText()
    {
        // Show the Level Complete Text
        if (levelCompleteText != null)
        {
            levelCompleteText.text = "Level Complete!!!";
            levelCompleteUI.SetActive(true);
        }
        GameManager.SetMode(GameManager.Mode.completed_level);
    }
}