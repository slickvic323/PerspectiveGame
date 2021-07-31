using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCameraBehaviour : MonoBehaviour
{

    enum VIEW
    {
        TOP,
        FRONT,
        SIDE
    };

    enum TRANSITIONING_TO
    {
        NONE, // Not transitioning
        TOP,
        FRONT,
        SIDE
    };

    bool camNoMove;

    readonly float CAMERA_SPEED = 27.0f;

    private GameObject centerTitleBlock;


    readonly float TOP_POS_X = 29.8f;
    readonly float TOP_POS_Y = 120f;
    readonly float TOP_POS_Z = -30f;
    

    readonly float TOP_ANGLE_X = 90f;
    readonly float TOP_ANGLE_Y = 0f;
    readonly float TOP_ANGLE_Z = 0f;

    readonly float FRONT_POS_X = 29.8f;
    readonly float FRONT_POS_Y = -30;
    readonly float FRONT_POS_Z = -110f;

    readonly float FRONT_ANGLE_X = 0f;
    readonly float FRONT_ANGLE_Y = 0f;
    readonly float FRONT_ANGLE_Z = 0f;

    readonly float SIDE_POS_X = -40f;
    readonly float SIDE_POS_Y = 0f;
    readonly float SIDE_POS_Z = 3;

    readonly float SIDE_ANGLE_X = 15f;
    readonly float SIDE_ANGLE_Y = 90f;
    readonly float SIDE_ANGLE_Z = 0f;

    float newPositionStartTime;
    float transitionStartTime;

    Vector3 transitionPositionStart, transitionPositionEnd;
    Vector3 transitionAngleStart, transitionAngleEnd;
    float transitionPositionDiff;

    int transitionStatus;
    int viewStatus;

    readonly float HOLD_TIME = 7f;

    private Camera mainCamera;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        centerTitleBlock = GameObject.FindWithTag("CenterTitleBlock");
        camNoMove = false;

        // Set camera to top view to begin with
        mainCamera.transform.position = new Vector3(TOP_POS_X, TOP_POS_Y, TOP_POS_Z);
        mainCamera.transform.eulerAngles = new Vector3(TOP_ANGLE_X, TOP_ANGLE_Y, TOP_ANGLE_Z);
        //mainCamera.transform.LookAt(new Vector3(centerTitleBlock.transform.position.x,
        //    centerTitleBlock.transform.position.y-6000f,
        //    centerTitleBlock.transform.position.z));



        viewStatus = (int)VIEW.TOP;
        transitionStatus = (int)TRANSITIONING_TO.NONE;
        newPositionStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!camNoMove)
        {
            // If not transitioning between camera views, check if should be
            if (transitionStatus == (int)TRANSITIONING_TO.NONE)
            {
                if (Time.time - newPositionStartTime >= HOLD_TIME)
                {
                    transitionPositionStart = mainCamera.transform.position;
                    transitionAngleStart = mainCamera.transform.eulerAngles;
                    // determine which view to transition to
                    if (viewStatus == 0)
                    {
                        viewStatus = 1;
                    }
                    else if (viewStatus == 1)
                    {
                        viewStatus = 0;
                    }
                    switch (viewStatus)
                    {
                        case ((int)VIEW.TOP):
                            {
                                transitionStatus = (int)TRANSITIONING_TO.TOP;
                                transitionPositionEnd = new Vector3(TOP_POS_X, TOP_POS_Y, TOP_POS_Z);
                                transitionAngleEnd = new Vector3(TOP_ANGLE_X, TOP_ANGLE_Y, TOP_ANGLE_Z);
                                break;
                            }
                        case ((int)VIEW.FRONT):
                            {
                                transitionStatus = (int)TRANSITIONING_TO.FRONT;
                                transitionPositionEnd = new Vector3(FRONT_POS_X, FRONT_POS_Y, FRONT_POS_Z);
                                transitionAngleEnd = new Vector3(FRONT_ANGLE_X, FRONT_ANGLE_Y, FRONT_ANGLE_Z);
                                break;
                            }
                        case ((int)VIEW.SIDE):
                            {
                                transitionStatus = (int)TRANSITIONING_TO.SIDE;
                                transitionPositionEnd = new Vector3(SIDE_POS_X, SIDE_POS_Y, SIDE_POS_Z);
                                transitionAngleEnd = new Vector3(SIDE_ANGLE_X, SIDE_ANGLE_Y, SIDE_ANGLE_Z);
                                break;
                            }
                        default:
                            {
                                Debug.Log("Something went wrong with camera");
                                break;
                            }
                    }

                    transitionPositionDiff = Vector3.Distance(transitionPositionStart, transitionPositionEnd);
                    transitionStartTime = Time.time;
                }
            }

            if (transitionStatus != (int)TRANSITIONING_TO.NONE)
            {
                TransitionBetweenViews();
            }
        }
    }

    void TransitionBetweenViews()
    {
        float distCovered = (Time.time - transitionStartTime) * CAMERA_SPEED;
        float percentageComplete = distCovered / transitionPositionDiff;
        if (percentageComplete < 1.0f)
        {
            transform.position = Vector3.Lerp(transitionPositionStart, transitionPositionEnd, percentageComplete);
            transform.eulerAngles = Vector3.Lerp(transitionAngleStart, transitionAngleEnd, percentageComplete);
            //mainCamera.transform.LookAt(centerTitleBlock.transform.position);
        }
        else
        {
            transitionStatus = (int)TRANSITIONING_TO.NONE;
            newPositionStartTime = Time.time;
        }
    }

    public void TutorialMenuOpen()
    {
        mainCamera.transform.position = new Vector3(TOP_POS_X, TOP_POS_Y, TOP_POS_Z);
        mainCamera.transform.eulerAngles = new Vector3(TOP_ANGLE_X, TOP_ANGLE_Y, TOP_ANGLE_Z);
        camNoMove = true;
    }

    public void TutorialMenuClose()
    {
        Start();
    }
}
