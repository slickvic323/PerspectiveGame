using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains Logic for a single platform in the game.
*/
public class Platform
{
    /*
     * Stores the maximum number of times that a ball can bounce on a platform.
     */
    private static readonly int MAX_NUM_BOUNCES = 5;

    /*
     * Stores the number of times remaining that a ball can bounce on a platform.
    */
    private int numBouncesRemaining = MAX_NUM_BOUNCES; // Really are MAX_NUM_BOUNCES - 1 bounces total, but subtracts right off the first bounce

    private bool tapped;

    private bool litUp;

    private bool ballOnPlatform;

    private int xDimmension;

    private int yDimmension;

    private int zDimmension;

    private GameObject gameObject;

    private bool inPattern;

    private int xIndex;
    private int zIndex;

    // Constructor
    public Platform()
    {
        tapped = false;
        litUp = false;
        ballOnPlatform = false;
        inPattern = false;
    }

    public int GetNumBouncesRemaining ()
    {
        return numBouncesRemaining;
    }

    public void SubtractOneBounce()
    {
        numBouncesRemaining--;
    }

    /*
     * Creates the platform object. 3-D Rectangular Object. Initializes visual properties of platform object.
     */
    public void CreateGameObject()
    {
        gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.transform.localScale = new Vector3(0.7f, 0.1f, 0.7f);
        gameObject.GetComponent<Renderer>().material.color = Color.black;
    }

    /*
     * Returns Platform Game Object
     */
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    /*
     * Sets the position of the platform.
     */
    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
        xIndex = (int) Mathf.Floor(position.x);
        zIndex = (int)Mathf.Floor(position.z);
    }

    /*
     * Returns the position of the platform.
     */
    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    // Returns whether or not the platform has been tapped
    public bool GetTapped()
    {
        return tapped;
    }

    // Return the X Dimmension
    public int GetXDimmension()
    {
        return xDimmension;
    }

    // Return the Y Dimmension
    public int GetYDimmension()
    {
        return yDimmension;
    }

    // Return the Z Dimmension
    public int GetZDimmension()
    {
        return zDimmension;
    }

    public void SetLitUp(bool isLitUp)
    {
        litUp = isLitUp;
    }

    public bool GetLitUp()
    {
        return litUp;
    }

    public bool IsBallOnPlatform()
    {
        return ballOnPlatform;
    }

    public void SetBallOnPlatform(bool ballOnPlatform, bool ballLandingOnWrongPlatform)
    {
        this.ballOnPlatform = ballOnPlatform;
        if (ballOnPlatform && !ballLandingOnWrongPlatform)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (!ballOnPlatform && !ballLandingOnWrongPlatform)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.black;
        }
        else
        {
            litUp = true;
        }
    }

    public void SetInPattern(bool inPattern)
    {
        this.inPattern = inPattern;
    }

    public bool IsInPattern()
    {
        return inPattern;
    }

    public int GetXIndex()
    {
        return xIndex;
    }

    public int GetZIndex()
    {
        return zIndex;
    }
}
