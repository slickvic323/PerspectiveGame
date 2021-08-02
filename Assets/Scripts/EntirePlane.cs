using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntirePlane : MonoBehaviour
{

    //private static GameManager myGameManager;

    /*
     * Stores the Number of Platforms on the X-Axis
     */
    private int numberPlatformsX;

    /*
     * Stores the Number of Platforms on the Z-Axis
     */
    private int numberPlatformsZ;

    /*
     * Stores the Platform Game Objects in a 2-D ArrayList. 
     * X Axis = Outer ArrayList, Z-Axis = Inner ArrayList
    */
    private List<List<Platform>> platforms;

    private List<Platform> fakePlatformRefs;

    /*
     * Swipe Detector Object
     */
    SwipeDetector mySwipeDetector;

    /*
     * Ball Game Object
     */
    private Ball ball;

    /*
     * Stores whether or not the ball exists in Game at current moment
     */
    private bool ballExists = false;

    /**
     * Pattern Animation Speed value (Normal)
     */
    private readonly float NORMAL_ANIMATION_SPEED = 1.2f;

    /** 
     * Pattern Animation Speed value (Fast)
     */
    private readonly float FAST_FORWARD_ANIMATION_SPEED = 0.4f;

    /**
     * Actual Pattern Animation Speed
     */
    private float animationSpeed;

    /**
     * Camera Angles
     */
    private float beforeAngle, smallerAngle, biggerAngle;

    /**
     * Arrow Handler Instance
     */
    private ArrowHandler arrowHandler;

    /**
     * Previous Y Velocity
     */
    private float lastYVelocity = 0f;

    /**
     * Has the first bounce occurred yet
     */
    private bool firstBounceYet = false;

    /**
     * Velocity of the first bounce
     */
    private float firstBounceVelocity = 0f;

    /**
     * Is the ball currently changing platforms
     */
    private bool changingPlatforms = false;

    /**
     * Is the ball going to begin changing platforms on the next bounce
     */
    private bool changePlatformsOnNextBounce = false;

    /**
     * What Platform is ball moving to
     */
    private Vector3 endPlatform;

    /**
     * Pattern Instance
     */
    private Pattern pattern;

    /**
     * Index of pattern progression that ball is on.
     */
    private int ballMovingAlongPatternIndex;

    /**
     * Is the ball going to land on the wrong platform
     */
    private bool ballWillLandOnWrongPlatform;

    /**
     * Is the ball going to fall of the edge
     */
    private bool ballWillFallOffEdge;

    /**
     * Was a valid move made
     */
    private bool validMoveMade;

    /**
     * Previous move ball made
     */
    private int previousMove;

    /**
     * Main Camera Instance
     */
    private Camera mainCamera;

    /**
     * Camera Data
     */
    private CameraInfo cameraInfo;

    /**
     * Camera's Initial Position
     */
    private Vector3 cameraStartPosition;

    /**
     * Camera's Final Position
     */
    private Vector3 cameraEndPosition;

    /**
     * Speed at which camera moves
     */
    private readonly float cameraSpeed = 5.0f;

    /**
     * The initial time of camera move
     */
    private float startTime;

    /**
     * Distance camera moves
     */
    private float journeyLength;

    /**
     * End Angle of camera
     */
    private float endAngle;

    /**
     * Is the ball on a new platform
     */
    private bool ballOnNewPlatform;

    /**
     * Ball landing on final platform of pattern
     */
    private bool ballWillLandOnFinalPlatform;

    /**
     * X Distance between one platform to another
     */
    private float PLATFORM_GRID_X_DISTANCE;

    /**
     * Z Distance betwen one platform to another
     */
    private float PLATFORM_GRID_Z_DISTANCE;

    /**
     * Is camera in aerial View
     */
    private bool aerialView;

    /**
     * Is Pattern animation showing
     */
    private bool showingPatternAnimation = false;

    /**
     * The previous time that the pattern revealed the subsequent platform.
     */
    private float patternAnimationPrevTriggerTime;

    /**
     * Index holding the pattern platform number currently.
     */
    private int patternAnimationIndex;

    private bool camouflageFakePlatformsInProgress = false;

    private float camoulfageStartingTime;
    private float totalCamoTime;

    // Moving camera from aerial to beginning
    private Vector3 cameraStartPositionAerial;
    private Vector3 cameraEndPositionAerial;
    private float aerialCameraSpeed = 5.0f;
    private float startTimeAerialTransition;
    private float journeyLengthAerialTransition;
    private bool transitioningFromAerialToInitial;

    GameObject myCanvas;
    GameObject levelCompleteUI;
    GameObject levelFailUI;
    GameObject gameFailUI;
    GameObject bouncesLeftUI;
    GameObject pointScoreUI;
    GameObject livesRemainingUI;
    GameObject fastForwardButton;
    GameObject quitGameUI;
    AudioManager audioManager;

    Text gameFailScoreText;
    TextMeshProUGUI numBouncesLeftText;
    Text pointsText;
    Text livesText;

    private GameObject newHighScoreFolder;
    private Text newRankText;
    private TextMeshProUGUI difficultyTitleText;
    private TMP_InputField highScoreNameInput;
    private GameObject[] hsDataRankNumsText, hsDataPlayersText, hsDataScoresText;
    private int highscoreSet = -1;

    private GameObject completeMenuLevelNum, completeMenuGridSize, completeMenuPoints;
    private GameObject failMenuLevelNum, failMenuGridSize, failMenuPoints;
    private GameObject levelInfoUI, levelInfoText;

    private float timerBetweenLevelsMusic;
    private bool waitingToPlayBetweenLevelMusic;


    // Start is called before the first frame update
    void Start()
    {
        //CleanAllHighScores();
        if (GameManager.GetMode() == GameManager.Mode.new_game_setup)
        {
            GameManager.StartNewGame();
        }
        else
        {
            GameManager.StartNewLevel();
        }

        mainCamera = Camera.main;
        //cameraInfo = new CameraInfo();
        cameraInfo = gameObject.AddComponent(typeof(CameraInfo)) as CameraInfo;
        aerialView = true;
        previousMove = 0;

        waitingToPlayBetweenLevelMusic = false;

        switch (GameManager.GetLevelPatternDirection())
        {
            case ((int)GameManager.PATTERN_DIRECTION.UP):
                {
                    cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_Z);
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.DOWN):
                {
                    cameraInfo.SetDirectionFacing(CameraInfo.FACING_NEG_Z);
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.RIGHT):
                {
                    cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_X);
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.LEFT):
                {
                    cameraInfo.SetDirectionFacing(CameraInfo.FACING_NEG_X);
                    break;
                }
            default:
                {
                    Debug.Log("Error with setting Camera Direction Facing");
                    cameraInfo.SetDirectionFacing(CameraInfo.FACING_POS_Z);
                    break;
                }

        }

        cameraInfo.SetMode(CameraInfo.STAGNANT);

        //arrowHandler = new ArrowHandler();
        arrowHandler = gameObject.AddComponent(typeof(ArrowHandler)) as ArrowHandler;
        arrowHandler.SetUpArrows();
        //mySwipeDetector = new SwipeDetector(arrowHandler);
        mySwipeDetector = gameObject.AddComponent(typeof(SwipeDetector)) as SwipeDetector;
        mySwipeDetector.SetArrowHandler(arrowHandler);

        myCanvas = GameObject.FindWithTag("InGameCanvas");
        if (myCanvas != null)
        {
            levelCompleteUI = GameObject.FindWithTag("LevelComplete");
            completeMenuLevelNum = GameObject.Find("LevelNumberTextComplete");
            completeMenuGridSize = GameObject.Find("GridSizeTextComplete");
            completeMenuPoints = GameObject.Find("TotalPointsTextComplete");
            levelCompleteUI.SetActive(false);

            levelFailUI = GameObject.FindWithTag("LevelFail");
            failMenuLevelNum = GameObject.Find("LevelNumberTextFail");
            failMenuGridSize = GameObject.Find("GridSizeTextFail");
            failMenuPoints = GameObject.Find("TotalPointsTextFail");
            levelFailUI.SetActive(false);

            // Game Fail Instantiation
            gameFailUI = GameObject.FindWithTag("GameFail");
            gameFailScoreText = GameObject.Find("GameScoreText").GetComponent<Text>();

            newHighScoreFolder = GameObject.Find("NewHighScoreUI");
            newRankText = GameObject.Find("NewHighScoreText").GetComponent<Text>();
            difficultyTitleText = GameObject.Find("DifficultyTitleText").GetComponent<TextMeshProUGUI>();
            highScoreNameInput = GameObject.Find("PlayerInput").GetComponent<TMP_InputField>();
            highScoreNameInput.onValueChanged.AddListener(delegate { HighScoreNameChangeCheck(); });

            hsDataRankNumsText = new GameObject[3];
            hsDataPlayersText = new GameObject[3];
            hsDataScoresText = new GameObject[3];
            for (int i=0;i<3;i++)
            {
                hsDataRankNumsText[i] = GameObject.Find("Rank" + (i + 1) + "TextNewHighScores");
                hsDataPlayersText[i] = GameObject.Find("Player" + (i + 1) + "TextNewHighScores");
                hsDataScoresText[i] = GameObject.Find("Score" + (i + 1) + "TextNewHighScores");
            }

            newHighScoreFolder.SetActive(false);
            gameFailUI.SetActive(false);

            bouncesLeftUI = GameObject.FindWithTag("BouncesLeftUI");
            numBouncesLeftText = bouncesLeftUI.GetComponentInChildren<TextMeshProUGUI>();
            numBouncesLeftText.text = "";

            pointScoreUI = GameObject.Find("PointScoreUI");
            pointsText = pointScoreUI.GetComponentInChildren<Text>();
            if (GameManager.GetCurrentNumPoints().ToString().Length <= 7)
            {
                pointsText.text = GameManager.GetCurrentNumPoints().ToString() + " pts";
            }
            else
            {
                pointsText.text = "1000000 pts";
                Debug.Log(pointsText.text.Length);
            }

            livesRemainingUI = GameObject.Find("LivesRemainingUI");
            livesText = livesRemainingUI.GetComponentInChildren<Text>();
            if (GameManager.GetNumLivesRemaining() != 1)
            {
                livesText.text = GameManager.GetNumLivesRemaining() + " Lives";
            }
            else
            {
                livesText.text = GameManager.GetNumLivesRemaining() + " Life";
            }

            fastForwardButton = GameObject.Find("FastForwardAnimationButton");
            fastForwardButton.SetActive(false);

            quitGameUI = GameObject.Find("QuitGameUI");
            quitGameUI.SetActive(false);

            levelInfoUI = GameObject.Find("LevelInfoUI");
            levelInfoText = GameObject.Find("LevelInfoText");
            levelInfoUI.SetActive(false);

            audioManager = FindObjectOfType<AudioManager>();
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

        camouflageFakePlatformsInProgress = false;

        AddFakePlatforms();

        // Populate the Level Info
        levelInfoText.GetComponent<Text>().text = "Level: " + GameManager.GetCurrentLevelNumber() + ", Grid Size: " + numberPlatformsX + "x" + numberPlatformsZ;
        levelInfoUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetMode() == GameManager.Mode.gameplay)
        {
            mySwipeDetector.Update();
        }
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


        if (camouflageFakePlatformsInProgress)
        {
            CamouflageFakePlatforms();
        }

        if (waitingToPlayBetweenLevelMusic && Time.time - timerBetweenLevelsMusic >= 1.2f)
        {
            audioManager.Play("Between_Levels_Drums", 1f);
            waitingToPlayBetweenLevelMusic = false;
        }
    }

    /**
     * Create platforms and pattern for those platforms
     */
    void CreatePlatforms()
    {
        platforms = new List<List<Platform>>();
        for (int i = 0; i < numberPlatformsX; i++)
        {
            List<Platform> temp = new List<Platform>();
            for (int j = 0; j < numberPlatformsZ; j++)
            {
                Platform platform = new Platform();
                platform.CreateGameObject();
                platform.SetPosition(new Vector3(i, 0, j));
                temp.Add(platform);
            }
            platforms.Add(temp);
        }

        pattern = new Pattern();
        bool createdPattern = false;
        if (!GameManager.GetFailedPrevAttempt())
        {
            createdPattern = pattern.CreatePattern(GameManager.GetLevelPatternDirection(), platforms, numberPlatformsX, numberPlatformsZ);
        }
        else
        {
            // Re-use previous pattern
            createdPattern = pattern.ResetPreviousPattern(platforms, numberPlatformsX, numberPlatformsZ, GameManager.GetPreviousPattern());
            GameManager.SetFailedPrevAttempt(false);
        }
        // Called from here to prevent possible stack overflow error (Avoiding Recursion from within Pattern class)
        while (!createdPattern)
        {
            createdPattern = pattern.CreatePattern(GameManager.GetLevelPatternDirection(), platforms, numberPlatformsX, numberPlatformsZ);
        }

        // Save the previous pattern to re-use in case player fails level
        //GameManager.CopyPreviousPlaneOfPlatforms(platforms);
        GameManager.SetPreviousPattern(pattern.GetPattern());

        // Now that the pattern has been decide, show the pattern animation
        showingPatternAnimation = true;
        animationSpeed = NORMAL_ANIMATION_SPEED;
        fastForwardButton.SetActive(true);
        patternAnimationPrevTriggerTime = Time.time;
        patternAnimationIndex = 0;
    }

    /**
     * Handles all ball bouncing events
     */
    private void ProperBounce()
    {
        if (GameManager.GetMode() == GameManager.Mode.gameplay)
        {
            if (ballWillFallOffEdge)
            {
                BallMadeWrongMove();
            }
        }

        if (!firstBounceYet && lastYVelocity < 0 && ball.GetRigidbody().velocity.y > 0)
        {
            firstBounceVelocity = lastYVelocity * -1f;
            ball.GetRigidbody().velocity = new Vector3(ball.GetRigidbody().velocity.x, firstBounceVelocity, ball.GetRigidbody().velocity.z);
            firstBounceYet = true;

            if (GameManager.GetMode() == GameManager.Mode.gameplay)
            {
                audioManager.Play("Pattern_Single_Note", 1f + ((ballMovingAlongPatternIndex - 1) * 0.1f));
            }
        }
        else if (firstBounceYet && lastYVelocity < 0 && ball.GetRigidbody().velocity.y > 0)
        {
            if (GameManager.GetMode() == GameManager.Mode.gameplay)
            {
                audioManager.Play("Pattern_Single_Note", 1f + ((ballMovingAlongPatternIndex - 1) * 0.1f));
                // Check if there are bounces remaining on this platform
                if (ball.GetWhichPlatfromOnX() >= 0 && ball.GetWhichPlatfromOnX() < numberPlatformsX 
                    && ball.GetWhichPlatfromOnZ() >= 0 && ball.GetWhichPlatfromOnZ() < numberPlatformsZ)
                {
                    if (platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].GetNumBouncesRemaining() > 0)
                    {
                        // Bounces Remaining
                        platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].SubtractOneBounce();
                    }
                    else
                    {
                        // No Bounces Remaining
                        BallMadeWrongMove();
                    }
                    numBouncesLeftText.text = platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].GetNumBouncesRemaining().ToString();
                }
            }

            ball.GetRigidbody().velocity = new Vector3(ball.GetRigidbody().velocity.x, firstBounceVelocity, ball.GetRigidbody().velocity.z);

            // Check if made incorrect move
            if (ballWillLandOnWrongPlatform)
            {
                if (GameManager.GetMode() == GameManager.Mode.gameplay)
                {
                    BallMadeWrongMove();
                }
            }
            else if (ballWillLandOnFinalPlatform)
            {
                if (!levelCompleteUI.activeSelf)
                {
                    ShowCompletedLevelText();
                }
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

                // Handle Points Update
                if (validMoveMade)
                {
                    if (previousMove == Ball.MOVING_FORWARD)
                    {
                        GameManager.ForwardMove();
                    }
                    else if (previousMove == Ball.MOVING_LEFT || previousMove == Ball.MOVING_RIGHT)
                    {
                        GameManager.TurnMove();
                    }

                    if (GameManager.GetCurrentNumPoints().ToString().Length <= 7)
                    {
                        pointsText.text = GameManager.GetCurrentNumPoints().ToString() + " pts";
                    }
                    else
                    {
                        pointsText.text = "1000000 pts";
                    }
                    validMoveMade = false;
                }

            }
        }

        lastYVelocity = ball.GetRigidbody().velocity.y;

        if (changingPlatforms)
        {
            // Checks if ball has completely made it onto the next platform
            CheckIfShouldStopMoving();
        }
    }

    /**
     * Begin having ball switch platforms
     */
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

    /**
     * Should ball stop moving in X or Z directions.
     */
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
            // Save previous move direction for corresponding adding points
            previousMove = ball.GetDirectionMoving();
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

    /**
     * Creates Ball Object
     */
    private void CreateBall()
    {
        int ballDirectionFacing = -1;
        switch (GameManager.GetLevelPatternDirection())
        {
            case ((int)GameManager.PATTERN_DIRECTION.UP):
                {
                    ballDirectionFacing = Ball.FACING_POS_Z;
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.DOWN):
                {
                    ballDirectionFacing = Ball.FACING_NEG_Z;
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.RIGHT):
                {
                    ballDirectionFacing = Ball.FACING_POS_X;
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.LEFT):
                {
                    ballDirectionFacing = Ball.FACING_NEG_X;
                    break;
                }
            default:
                {
                    Debug.Log("Error with setting Ball Direction Facing");
                    ballDirectionFacing = Ball.FACING_POS_Z;
                    break;
                }

        }
        ball = new Ball(ballDirectionFacing);
        ballExists = true;

        List < Platform > patternList = pattern.GetPattern();
        // Put ball on first platform in pattern
        Platform firstPlatform = patternList[0];
        ball.SetWhichPlatformOn(firstPlatform.GetXIndex(), firstPlatform.GetZIndex());
        ball.CreateGameObject();
        platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].SetBallOnPlatform(true, false);
        ballMovingAlongPatternIndex = 1; // Start at 1 because the first move will be to the 1st index in pattern
        ballWillLandOnWrongPlatform = false;
        ballWillFallOffEdge = false;
        ballWillLandOnFinalPlatform = false;

        // Begin moving camera from aerial view to the initial position
        cameraStartPositionAerial = mainCamera.transform.position;
        switch (GameManager.GetLevelPatternDirection())
        {
            case ((int)GameManager.PATTERN_DIRECTION.UP):
                {
                    cameraEndPositionAerial = new Vector3(ball.GetGameObject().transform.position.x, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z - 2.5f);
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.DOWN):
                {
                    cameraEndPositionAerial = new Vector3(ball.GetGameObject().transform.position.x, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z + 2.5f);
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.RIGHT):
                {
                    cameraEndPositionAerial = new Vector3(ball.GetGameObject().transform.position.x - 2.5f, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z);
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.LEFT):
                {
                    cameraEndPositionAerial = new Vector3(ball.GetGameObject().transform.position.x + 2.5f, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z);
                    break;
                }
            default:
                {
                    Debug.Log("Exception with setting camera end position");
                    break;
                }
        }
        //cameraEndPositionAerial = new Vector3(ball.GetGameObject().transform.position.x, CameraInfo.Y_FLOAT, ball.GetGameObject().transform.position.z - 2.5f);
        startTimeAerialTransition = Time.time;
        journeyLengthAerialTransition = Vector3.Distance(cameraStartPositionAerial, cameraEndPositionAerial);

        transitioningFromAerialToInitial = true;
    }

    /**
     * Update the camera
     */
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

    /**
     * Takes care of all camera data when transition begins
     */
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

    /**
     * Returns the total number of platforms in the x direction
     */
    public int GetNumberPlatformsX()
    {
        return numberPlatformsX;
    }

    /**
     * Returns the total number of platforms in the z direction
     */
    public int GetNumberPlatformsZ()
    {
        return numberPlatformsZ;
    }

    /**
     * Calculates Camera's ending position based on current position
     */
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

    /**
     * Angle Converter - prevents angles less than 0 or greater than 360
     */
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

    /**
     * Handles swipes
     */
    private void OnGUI()
    {
        if (GameManager.GetMode() == GameManager.Mode.gameplay)
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
    }

    /**
     * Ensures that a valid move was made
     */
    public void CheckIfValidMove()
    {
        validMoveMade = false;
        // Works for square grid
        if (ballExists)
        {
            int currentX = ball.GetWhichPlatfromOnX();
            int currentZ = ball.GetWhichPlatfromOnZ();
            // Check if falling off the edge
            if (currentX > numberPlatformsX-1 || currentX < 0 || currentZ > numberPlatformsZ || currentZ < 0)
            {
                ballWillFallOffEdge = true;
                Debug.Log("Ball falling off edge");
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

            ballWillFallOffEdge = false;
            ballWillLandOnWrongPlatform = false;
            validMoveMade = true;
            // Now that it is established that the CORRECT move was made, we can check if the player made it to the final platform of level
            CheckIfMadeCorrectFinalMove(currentX, currentZ);
        }
    }

    /**
     * Checks if made a correct final move
     */
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

    /**
     * Handles a ball bouncing on a platform for the first time
     */
    public void HandleNewPlatformBounce()
    {
        // Set New Platform to have ball on platform
        if (ball.GetWhichPlatfromOnX()>=0 && ball.GetWhichPlatfromOnX() < numberPlatformsX
            && ball.GetWhichPlatfromOnZ()>=0 && ball.GetWhichPlatfromOnZ() < numberPlatformsZ)
        {
            platforms[ball.GetWhichPlatfromOnX()][ball.GetWhichPlatfromOnZ()].SetBallOnPlatform(true, ballWillLandOnWrongPlatform);
        }

        // Set previous platform the ball was on to false for ballOnPlatform attribute
        if (ball.GetPreviousPlatformX() != int.MinValue && ball.GetPreviousPlatformZ() != int.MinValue)
        {
            if (ball.GetPreviousPlatformX() >= 0 && ball.GetPreviousPlatformX() < numberPlatformsX
                && ball.GetPreviousPlatformZ() >= 0 && ball.GetPreviousPlatformZ() < numberPlatformsZ)
            {
                platforms[ball.GetPreviousPlatformX()][ball.GetPreviousPlatformZ()].SetBallOnPlatform(false, ballWillLandOnWrongPlatform);
            }
        }
    }

    /**
     * Moves camera to aerial view
     */
    public void PutCamInAerialView()
    {
        mainCamera.transform.position = new Vector3((numberPlatformsX /2f) - 0.5f, PLATFORM_GRID_X_DISTANCE + (0.6f*numberPlatformsX) , numberPlatformsZ / 2f - 0.5f);
        mainCamera.transform.eulerAngles = new Vector3(90f, 0f, 0f);
    }

    /**
     * Moves camera from aerial view to 3rd person ball view for gameplay
     */
    public void MoveCamFromAerialToStart()
    {
        // Deals with camera's position
        float distCovered = (Time.time - startTimeAerialTransition) * aerialCameraSpeed;
        float fractionOfJourney = distCovered / journeyLengthAerialTransition;
        transform.position = Vector3.Lerp(cameraStartPositionAerial, cameraEndPositionAerial, fractionOfJourney);

        // Deals with camera's angle
        Vector3 angle1 = new Vector3(90f, 0f, 0f);
        Vector3 angle2;
        switch (GameManager.GetLevelPatternDirection())
        {
            case ((int)GameManager.PATTERN_DIRECTION.UP):
                {
                    angle2 = new Vector3(0f, 0f, 0f);
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.DOWN):
                {
                    if (ball.GetWhichPlatfromOnX() >= GameManager.GetNumXPlatforms()/2)
                    {
                        angle2 = new Vector3(0f, -180f, 0f);
                    }
                    else
                    {
                        angle2 = new Vector3(0f, 180f, 0f);
                    }
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.RIGHT):
                {
                    angle2 = new Vector3(0f, 90f, 0f);
                    break;
                }
            case ((int)GameManager.PATTERN_DIRECTION.LEFT):
                {
                    angle2 = new Vector3(0f, -90f, 0f);
                    break;
                }
            default:
                {
                    Debug.Log("Exception with setting camera end position");
                    angle2 = new Vector3(0f, 0f, 0f);
                    break;
                }
        }

        //Vector3 angle2 = new Vector3(0f, 0f, 0f);
        transform.eulerAngles = Vector3.Lerp(angle1, angle2, fractionOfJourney);

        //Vector3 targetPosition = new Vector3(ball.GetGameObject().transform.position.x,
        //                        ball.GetGameObject().transform.position.y,
        //                        ball.GetGameObject().transform.position.z);
        //transform.LookAt(targetPosition, Vector3.forward);

        // Check if moving camera to initial point is done
        if (transform.position == cameraEndPositionAerial)
        {
            transitioningFromAerialToInitial = false;
            GameManager.SetMode(GameManager.Mode.gameplay);
            mySwipeDetector.SetSwipeDetectorActivated(true);

            // Begin camouflaging Platforms if there are fake platforms

            if (GameManager.GAME_DIFFICULTY == (int)GameManager.DIFFICULTY.HARD)
            {
                camouflageFakePlatformsInProgress = true;
                totalCamoTime = 3.0f;
                camoulfageStartingTime = Time.time;
            }
        }
    }

    /**
     * Called if ball made incorrect move
     */
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

        if (GameManager.GAME_DIFFICULTY == (int)GameManager.DIFFICULTY.HARD)
        {
            for (int i=0;i<fakePlatformRefs.Count;i++)
            {
                camouflageFakePlatformsInProgress = false;
                fakePlatformRefs[i].GetGameObject().GetComponent<Renderer>().material.color = Color.red;
            }
        }


            // Handle Lives
            GameManager.SubtractNumLivesRemaining();
        if (GameManager.GetNumLivesRemaining() != 1)
        {
            livesText.text = GameManager.GetNumLivesRemaining() + " Lives";
        }
        else
        {
            livesText.text = GameManager.GetNumLivesRemaining() + " Life";
        }

        bouncesLeftUI.SetActive(false);
        if (GameManager.GetNumLivesRemaining() > 0)
        {
            audioManager.Play("Level_Fail_Sound", 1f);
            timerBetweenLevelsMusic = Time.time;
            waitingToPlayBetweenLevelMusic = true;
            // Show Level Fail UI
            failMenuLevelNum.GetComponent<Text>().text = "Level Number: " + GameManager.GetCurrentLevelNumber();
            failMenuGridSize.GetComponent<Text>().text = "Grid Size: " + numberPlatformsX + "x" + numberPlatformsZ;
            failMenuPoints.GetComponent<Text>().text = "Points: " + GameManager.GetCurrentNumPoints() + "pts";
            levelFailUI.SetActive(true);
        }
        else
        {
            // Play Music
            audioManager.Play("Menu_Music", 1f);

            // Show Game Fail UI
            gameFailScoreText.text = "Your Score: " + GameManager.GetCurrentNumPoints();
            if (GameManager.GAME_DIFFICULTY == 1)
            {
                difficultyTitleText.text = "Normal Mode HighScores";
            }
            else if (GameManager.GAME_DIFFICULTY == 2)
            {
                difficultyTitleText.text = "Hard Mode HighScores";
            }
            gameFailUI.SetActive(true);

            highscoreSet = GameManager.SetHighScore();
            if (highscoreSet != -1)
            {
                // New highscore was made
                newHighScoreFolder.SetActive(true);
                newRankText.text = "New #" + highscoreSet + " Rank!";
                hsDataRankNumsText[highscoreSet - 1].GetComponent<TextMeshProUGUI>().color = Color.green;
                hsDataPlayersText[highscoreSet - 1].GetComponent<TextMeshProUGUI>().color = Color.green;
                hsDataScoresText[highscoreSet - 1].GetComponent<TextMeshProUGUI>().color = Color.green;

                // Ask user for their three character name to go with highscore
                string defaultHighscoreString = "***";
                GameManager.SetAndBumpHighScoreName(defaultHighscoreString, highscoreSet);

                // Fill in high score data
                for (int i = 0; i < 3; i++)
                {
                    if (i == highscoreSet - 1)
                    {
                        // Grab from new score just created
                        hsDataPlayersText[i].GetComponent<TextMeshProUGUI>().text = "***";
                    }
                    else
                    {
                        // Grab from high score data
                        hsDataPlayersText[i].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString(((int)GameManager.GAME_DIFFICULTY).ToString() + "highscoreName" + (i + 1), "***");
                    }
                    hsDataScoresText[i].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt(((int)GameManager.GAME_DIFFICULTY).ToString() + "highscore" + (i + 1), 0).ToString();
                }
            }
            else
            {
                // Still fill in high score data from previous existing
                for (int i = 0; i < 3; i++)
                {
                    hsDataPlayersText[i].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString(((int)GameManager.GAME_DIFFICULTY).ToString() + "highscoreName" + (i + 1), "***");
                    hsDataScoresText[i].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt(((int)GameManager.GAME_DIFFICULTY).ToString() + "highscore" + (i + 1), 0).ToString();
                }
            }
        }


        GameManager.SetFailedPrevAttempt(true);
        GameManager.SetMode(GameManager.Mode.failed_level);
        mySwipeDetector.SetSwipeDetectorActivated(false);
    }

    /**
     * Plays button press sound
     */
    public void PlayButtonPressSound()
    {
        audioManager.Play("Button_Press", 0.5f);
    }

    /**
     * Called when beginning to show pattern animation
     */
    public void ShowPatternAnimation()
    {
        float currentTime = Time.time;
        // If it has been at least 1.2 seconds since the previous platform in the pattern turned blue for the animation
        if (currentTime - patternAnimationPrevTriggerTime >= animationSpeed)
        {
            List<Platform> patternList = pattern.GetPattern();
            if (patternAnimationIndex < patternList.Count)
            {
                if (patternList.Count <= 40)
                {
                    audioManager.Play("Pattern_Single_Note", 1f + (patternAnimationIndex * 0.1f));

                }
                else
                {
                    audioManager.Play("Pattern_Single_Note", 1f + (patternAnimationIndex * (4f/patternList.Count)));
                }
                Platform nextPlatformInAnimation = patternList[patternAnimationIndex];
                nextPlatformInAnimation.GetGameObject().GetComponent<Renderer>().material.color = new Color(0f, 1f, 1f, 1f);
                // If in hard mode, only show a maximum of 1/2 the platforms at a time of the pattern
                //if (GameManager.GAME_DIFFICULTY == (int)GameManager.DIFFICULTY.HARD)
                //{
                //    if (patternAnimationIndex > (patternList.Count/2)-1)
                //    {
                //        Platform backToBlack = patternList[patternAnimationIndex - (patternList.Count/2)];
                //        backToBlack.GetGameObject().GetComponent<Renderer>().material.color = Color.black;
                //    }
                //}
                patternAnimationIndex++;
                patternAnimationPrevTriggerTime = Time.time;
            }
            else
            {
                // Reached the end of the animation
                showingPatternAnimation = false;
                animationSpeed = NORMAL_ANIMATION_SPEED;
                fastForwardButton.SetActive(false);

                // Create the Ball once the pattern animation has finished
                CreateBall();

                // Set all platforms to black
                for (int i=0;i<numberPlatformsX;i++)
                {
                    List<Platform> currentRow = platforms[i];
                    for (int j=0;j<numberPlatformsZ;j++)
                    {
                        currentRow[j].GetGameObject().GetComponent<Renderer>().material.color = Color.black;
                    }
                }

                //// If on Hard Mode, create fake platforms that surround the real platforms
                //if (GameManager.GAME_DIFFICULTY == (int)GameManager.DIFFICULTY.HARD)
                //{
                //    for (int i = -5; i < 5 + numberPlatformsX; i++)
                //    {
                //        List<Platform> temp = new List<Platform>();
                //        for (int j = -5; j < 5 + numberPlatformsZ; j++)
                //        {
                //            if (i < 0 || i >= numberPlatformsX || j < 0 || j >= numberPlatformsZ)
                //            {
                //                Platform platform = new Platform();
                //                platform.CreateGameObject();
                //                platform.SetPosition(new Vector3(i, 0, j));
                //                platform.GetGameObject().GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0.0f);

                //                temp.Add(platform);
                //            }
                //        }
                //        platforms.Add(temp);
                //    }
                //}

                // Don't show level info UI anymore
                levelInfoUI.SetActive(false);
            }
        }
    }

    /**
     * Show Completed Level GUI
     */
    public void ShowCompletedLevelText()
    {
        audioManager.Play("Level_Complete_Sound", 1f);
        timerBetweenLevelsMusic = Time.time;
        waitingToPlayBetweenLevelMusic = true;
        // Show the Level Complete Text
        completeMenuLevelNum.GetComponent<Text>().text = "Level Number: " + GameManager.GetCurrentLevelNumber();
        completeMenuGridSize.GetComponent<Text>().text = "Grid Size: " + numberPlatformsX + "x" + numberPlatformsZ;
        completeMenuPoints.GetComponent<Text>().text = "Points: " + GameManager.GetCurrentNumPoints() + "pts";
        bouncesLeftUI.SetActive(false);
        levelCompleteUI.SetActive(true);
        GameManager.SetMode(GameManager.Mode.completed_level);
        mySwipeDetector.SetSwipeDetectorActivated(false);
    }

    /**
     * Called when fast forward button is pressed. Speeds up pattern animation
     */
    public void FastForwardButtonPressed()
    {
        if (animationSpeed == NORMAL_ANIMATION_SPEED)
        {
            animationSpeed = FAST_FORWARD_ANIMATION_SPEED;
        }
    }

    /**
     * Called when fast forward button is released. Return pattern animation speed to normal
     */
    public void FastForwardButtonReleased()
    {
        if (animationSpeed == FAST_FORWARD_ANIMATION_SPEED)
        {
            animationSpeed = NORMAL_ANIMATION_SPEED;
        }
    }

    /**
     * Quit game button pressed.
     */
    public void QuitGameButtonPressed()
    {
        if (!quitGameUI.activeSelf)
        {
            quitGameUI.SetActive(true);
        }
        else
        {
            quitGameUI.SetActive(false);
        }
    }

    /**
     * Closes Quit Game GUI
     */
    public void DoNotQuit()
    {
        quitGameUI.SetActive(false);
    }

    public void AddFakePlatforms()
    {
        // If on Hard Mode, create fake platforms that surround the real platforms
        if (GameManager.GAME_DIFFICULTY == (int)GameManager.DIFFICULTY.HARD)
        {
            fakePlatformRefs = new List<Platform>();
            for (int i = -5; i < 5 + numberPlatformsX; i++)
            {
                List<Platform> temp = new List<Platform>();
                for (int j = -5; j < 5 + numberPlatformsZ; j++)
                {
                    if (i < 0 || i >= numberPlatformsX || j < 0 || j >= numberPlatformsZ)
                    {
                        Platform platform = new Platform();
                        platform.CreateGameObject();
                        platform.SetPosition(new Vector3(i, 0, j));
                        platform.GetGameObject().GetComponent<Renderer>().material.color = Color.red;
                        fakePlatformRefs.Add(platform);
                        temp.Add(platform);
                    }
                }
                platforms.Add(temp);
            }
        }
    }

    public void CamouflageFakePlatforms()
    {
        // If on Hard Mode
        if (GameManager.GAME_DIFFICULTY == (int)GameManager.DIFFICULTY.HARD)
        {
            float timeDiff = Time.time - camoulfageStartingTime;

            for (int i = 0; i < fakePlatformRefs.Count; i++)
            {
                Platform platform = fakePlatformRefs[i];
                platform.GetGameObject().GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.black, timeDiff/totalCamoTime);

                if (timeDiff >= totalCamoTime)
                {
                    //Camouflage is complete
                    platform.GetGameObject().GetComponent<Renderer>().material.color = Color.black;
                    camouflageFakePlatformsInProgress = false;
                }
            }
        }
    }

    public void HighScoreNameChangeCheck()
    {
        string newHighScoreNameString = highScoreNameInput.text.ToString();
        // Don't allow player to input more than 3 characters
        if (newHighScoreNameString.Length > 3)
        {
            highScoreNameInput.text = newHighScoreNameString.Substring(0, 3);
        }

        // Ask user for their three character name to go with highscore
        string defaultHighscoreString = "***";
        if (newHighScoreNameString == null || newHighScoreNameString.Length <= 0 || newHighScoreNameString.Length > 3)
        {
            PlayerPrefs.SetString(GameManager.GAME_DIFFICULTY.ToString() + "highscoreName" + highscoreSet, defaultHighscoreString);
            hsDataPlayersText[highscoreSet - 1].GetComponent<TextMeshProUGUI>().text = defaultHighscoreString;
        }
        else
        {
            PlayerPrefs.SetString(GameManager.GAME_DIFFICULTY.ToString() + "highscoreName" + highscoreSet, newHighScoreNameString);
            hsDataPlayersText[highscoreSet - 1].GetComponent<TextMeshProUGUI>().text = newHighScoreNameString;
        }
    }

    public void CleanAllHighScores()
    {
        for (int i=0;i<3;i++)
        {
            PlayerPrefs.SetInt("1highscore" + (i + 1), 0);
            PlayerPrefs.SetString("1highscoreName" + (i + 1), "***");
            PlayerPrefs.SetInt("2highscore" + (i + 1), 0);
            PlayerPrefs.SetString("2highscoreName" + (i + 1), "***");
        }
    }
}
 