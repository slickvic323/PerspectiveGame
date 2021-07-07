using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 */
public class Ball : MonoBehaviour
{
    /**
     * Constant Values representing Direction Ball is moving.
     */
    public const int NOT_MOVING = 0;
    public const int MOVING_FORWARD = 1;
    public const int MOVING_RIGHT = 2;
    public const int MOVING_LEFT = 3;

    /**
     * Constant Values representing Direction Ball(Camera) is facing
     */
    public const int FACING_POS_Z = 1;
    public const int FACING_NEG_Z = 2;
    public const int FACING_POS_X = 3;
    public const int FACING_NEG_X = 4;

    /**
     * Sphere Game Object representing Ball.
     */
    private new GameObject gameObject;

    /**
     * RigidBody Physics element for the ball.
     */
    private Rigidbody rigidBody;

    /**
     * Int - X-Index that ball is bouncing on
     */
    private int whichPlatformOnX; // 0-based
    
    /**
     * Int - Z-Index that ball is bouncing on.
     */
    private int whichPlatformOnZ; // 0-based

    /**
     * Int - X-Index that ball was previously bouncing on.
     */
    private int previousPlatformX;

    /**
     * Int - Z-Index that ball was previously bounching on.
     */
    private int previousPlatformZ;

    /**
     * Maximum height (Y-Axis) that ball can reach.
     */
    private const float MAXY = 1.5f;
    
    /**
     * Current Direction Ball is Moving
     */
    private int directionMoving;

    /**
     * Current Direction Ball(Camera) is Facing
     */
    private int directionFacing;

    // Start is called before the first frame update
    void Start()
    {
        whichPlatformOnX = -1;
        whichPlatformOnZ = -1;
        previousPlatformX = -1;
        previousPlatformZ = -1;

        directionMoving = NOT_MOVING;
        directionFacing = FACING_POS_Z;
    }

    // Update is called once per frame
    void Update()
    {
    }

    /**
     * Create Sphere Game Object and set up physics for Ball.
     */
    public void CreateGameObject()
    {
        gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        gameObject.transform.position = new Vector3((float)GetWhichPlatfromOnX(), MAXY, (float)GetWhichPlatfromOnZ());

        gameObject.GetComponent<Renderer>().material.color = Color.red;

        rigidBody = gameObject.AddComponent<Rigidbody>();
        Collider collider = gameObject.GetComponent<Collider>();
        collider.material.bounciness = 1f;
        collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
        collider.material.bounceCombine = PhysicMaterialCombine.Maximum;
    }

    /**
     * Return Game Object
     */
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    /**
     * Return Physics Rigid Body for Ball
     */
    public Rigidbody GetRigidbody()
    {
        return rigidBody;
    }

    /**
     * Return Direction Moving
     */
    public int GetDirectionMoving()
    {
        return directionMoving;
    }

    /**
     * Set direction moving
     */
    public void SetDirectionMoving(int directionMoving)
    {
        this.directionMoving = directionMoving;
    }

    /**
     * Return Direction Facing
     */
    public int GetDirectionFacing()
    {
        return directionFacing;
    }

    /**
     * Set Direction Facing
     */
    public void SetDirectionFacing(int directionFacing)
    {
        this.directionFacing = directionFacing;
    }

    /**
     * Return X-Index of Platform the Ball is on.
     */
    public int GetWhichPlatfromOnX()
    {
        return whichPlatformOnX;
    }

    /**
     * Set X and Z Indeces of platform the Ball is on.
     */
    public void SetWhichPlatformOn(int whichX, int whichZ)
    {
        if (whichPlatformOnX != -1 && whichPlatformOnZ != -1)
        {
            previousPlatformX = whichPlatformOnX;
            previousPlatformZ = whichPlatformOnZ;
        }

        whichPlatformOnX = whichX;
        whichPlatformOnZ = whichZ;
    }

    /**
     * Get Z Index of platform the Ball is on
     */
    public int GetWhichPlatfromOnZ()
    {
        return whichPlatformOnZ;
    }

    /**
     * Get X-Index of previous platform ball was bouncing on.
     */
    public int GetPreviousPlatformX()
    {
        return previousPlatformX;
    }

    /**
     * Get Z-Index of previous platform ball was bouncing on.
     */
    public int GetPreviousPlatformZ()
    {
        return previousPlatformZ;
    }
}
