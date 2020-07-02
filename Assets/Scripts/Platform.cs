using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    private int MAX_NUM_BOUNCES = 6;

    private int numBouncesRemaining = 6; // Really are 5 bounces total, but subtracts right off the first bounce

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

    // Start is called before the first frame update
    void Start()
    {
        tapped = false;
        litUp = false;
        ballOnPlatform = false;
        inPattern = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetNumBouncesRemaining ()
    {
        return numBouncesRemaining;
    }

    public void SubtractOneBounce()
    {
        numBouncesRemaining--;
    }

    public void CreateGameObject()
    {
        gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.transform.localScale = new Vector3(0.7f, 0.1f, 0.7f);
        gameObject.GetComponent<Renderer>().material.color = Color.black;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
        xIndex = (int) Mathf.Floor(position.x);
        zIndex = (int)Mathf.Floor(position.z);
    }

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
        return xDimmension;
    }

    // Return the Z Dimmension
    public int GetZDimmension()
    {
        return xDimmension;
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
