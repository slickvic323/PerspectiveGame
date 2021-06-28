using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 */
public class Ball : MonoBehaviour
{
    private GameObject gameObject;

    private Rigidbody rigidBody;

    private int whichPlatformOnX; // 0-based
    private int whichPlatformOnZ; // 0-based

    private int previousPlatformX;
    private int previousPlatformZ;

    public const float MAXY = 1.5f;

    public const float MINY = 0f;

    public const int NOT_MOVING = 0;
    public const int MOVING_FORWARD = 1;
    public const int MOVING_RIGHT = 2;
    public const int MOVING_LEFT = 3;

    private int directionMoving = 0;

    public const int FACING_POS_Z = 1;
    public const int FACING_NEG_Z = 2;
    public const int FACING_POS_X = 3;
    public const int FACING_NEG_X = 4;

    private int directionFacing = 1;

    // Start is called before the first frame update
    void Start()
    {
        whichPlatformOnX = -1;
        whichPlatformOnZ = -1;
        previousPlatformX = -1;
        previousPlatformZ = -1;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateGameObject()
    {
        gameObject = gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        gameObject.transform.position = new Vector3((float)GetWhichPlatfromOnX(), MAXY, (float)GetWhichPlatfromOnZ());

        gameObject.GetComponent<Renderer>().material.color = Color.red;

        rigidBody = gameObject.AddComponent<Rigidbody>();
        Collider collider = gameObject.GetComponent<Collider>();
        collider.material.bounciness = 1f;
        collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
        collider.material.bounceCombine = PhysicMaterialCombine.Maximum;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Rigidbody GetRigidbody()
    {
        return rigidBody;
    }

    public int GetDirectionMoving()
    {
        return directionMoving;
    }

    public void SetDirectionMoving(int directionMoving)
    {
        this.directionMoving = directionMoving;
    }

    public int GetDirectionFacing()
    {
        return directionFacing;
    }

    public void SetDirectionFacing(int directionFacing)
    {
        this.directionFacing = directionFacing;
    }

    public int GetWhichPlatfromOnX()
    {
        return whichPlatformOnX;
    }

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

    public int GetWhichPlatfromOnZ()
    {
        return whichPlatformOnZ;
    }

    public int GetPreviousPlatformX()
    {
        return previousPlatformX;
    }

    public int GetPreviousPlatformZ()
    {
        return previousPlatformZ;
    }
}
