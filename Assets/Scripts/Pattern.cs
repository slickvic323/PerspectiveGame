using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    private List<Platform> pattern;

    private int NUM_X_PLATFORMS;
    private int NUM_Z_PLATFORMS;

    private int MIN_POSSIBLE_NUM_MOVES;
    private int MAX_POSSIBLE_NUM_MOVES;

    private int MIN_AIMING_FOR;
    private int MAX_AIMING_FOR;

    private int currentNumMoves;

    private bool reachedValidEndpoint;

    private bool[,] invalidSpot;

    int currentXIndex;
    int currentZIndex;

    private enum DirectionFacing
    {
        POS_Z = 0,
        POS_X = 1,
        NEG_Z = 2,
        NEG_X = 3
    }

    private int directionFacing;

    private bool canAddPlatform;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CreatePattern(List<List<Platform>> planeOfPlatforms, int numXPlatforms, int numZPlatforms)
    {
        currentNumMoves = 0;
        reachedValidEndpoint = false;

        invalidSpot = new bool[numZPlatforms, numXPlatforms];
        
        NUM_X_PLATFORMS = numXPlatforms;
        NUM_Z_PLATFORMS = numZPlatforms;

        MIN_POSSIBLE_NUM_MOVES = NUM_Z_PLATFORMS - 1;
        MAX_POSSIBLE_NUM_MOVES = (numXPlatforms * numZPlatforms) - 1;

        MIN_AIMING_FOR = (int) Mathf.Floor((float)MAX_POSSIBLE_NUM_MOVES / 2.5f);
        MAX_AIMING_FOR = (int) Mathf.Floor((float)MAX_POSSIBLE_NUM_MOVES / 1.25f);

        pattern = new List<Platform>();
        // Get Starting Point X value (Z is 0)
        int startingX = (int) Mathf.Floor(Random.Range(0, numXPlatforms));
        pattern.Add(planeOfPlatforms[startingX][0]);
        canAddPlatform = true;
        directionFacing = (int)DirectionFacing.POS_Z;

        currentXIndex = startingX;
        currentZIndex = 0;
        invalidSpot[0, currentXIndex] = true;

        // Add platforms to the pattern until reached the minimum length of pattern wanted
        while (canAddPlatform && currentNumMoves < MIN_AIMING_FOR)
        {
            canAddPlatform = AddPlatformToPattern(planeOfPlatforms);
        }

        while (canAddPlatform && !reachedValidEndpoint && currentNumMoves <= MAX_AIMING_FOR)
        {
            canAddPlatform = AddPlatformToPattern(planeOfPlatforms);
        }

        if (!reachedValidEndpoint || currentNumMoves < MIN_AIMING_FOR)
        {
            // Handle Creating a new pattern since the old one did not make it to the endpoint
            return false;
        }

        foreach (Platform platform in pattern)
        {
            // platform.GetGameObject().GetComponent<Renderer>().material.color = Color.blue;
            platform.SetInPattern(true);
        }

        return true;
    }

    public List<Platform> GetPattern()
    {
        return pattern;
    }

    private bool AddPlatformToPattern(List<List<Platform>> planeOfPlatforms)
    {
        bool upMoveValid = false;
        bool downMoveValid = false;
        bool rightMoveValid = false;
        bool leftMoveValid = false;

        // Check Up Move Validity
        if (currentZIndex < NUM_Z_PLATFORMS-1)
        {
            if (!invalidSpot[currentZIndex+1, currentXIndex])
            {
                upMoveValid = true;
            }
        }

        // Check Down Move Validity
        if (currentZIndex > 0)
        {
            if (!invalidSpot[currentZIndex - 1, currentXIndex])
            {
                downMoveValid = true;
            }
        }

        // Check Right Move Validity
        if (currentXIndex < NUM_X_PLATFORMS - 1)
        {
            if (!invalidSpot[currentZIndex, currentXIndex+1])
            {
                rightMoveValid = true;
            }
        }

        // Check Left Move Validity
        if (currentXIndex > 0)
        {
            if (!invalidSpot[currentZIndex, currentXIndex - 1])
            {
                leftMoveValid = true;
            }
        }

        bool hasValidMove = false;

        switch (directionFacing)
        {
            case (int)DirectionFacing.POS_Z:
                // Check Up, Left, and Right
                if (upMoveValid || leftMoveValid || rightMoveValid)
                {
                    hasValidMove = true;
                }
                break;
            case (int)DirectionFacing.POS_X:
                // Check Right, Up, and Down
                if (rightMoveValid || upMoveValid || downMoveValid)
                {
                    hasValidMove = true;
                }
                break;
            case (int)DirectionFacing.NEG_Z:
                // Check Down, Left, and Right
                if (downMoveValid || leftMoveValid || rightMoveValid)
                {
                    hasValidMove = true;
                }
                break;
            case (int)DirectionFacing.NEG_X:
                // Check Left, Up, and Down
                if (leftMoveValid|| upMoveValid|| downMoveValid)
                {
                    hasValidMove = true;
                }
                break;
            default:
                break;
        }

        if (hasValidMove)
        {
            float chooseUp = 0.0f;
            float chooseDown = 0.0f;
            float chooseLeft = 0.0f;
            float chooseRight = 0.0f;
            // The next portion of code ensure three different numbers between 0.0 and 1.0 that represent up, down, left, and right
            if (upMoveValid)
            {
                do
                {
                    chooseUp = Random.Range(0.0f, 1.0f);
                } while (chooseUp == chooseLeft || chooseUp == chooseRight || chooseUp == chooseDown);
            }
            if (downMoveValid)
            {
                do
                {
                    chooseDown = Random.Range(0.0f, 1.0f);
                } while (chooseDown == chooseUp || chooseDown == chooseLeft || chooseDown == chooseRight);
            }
            if (leftMoveValid)
            {
                do
                {
                    chooseLeft = Random.Range(0.0f, 1.0f);
                } while (chooseLeft == chooseUp || chooseLeft == chooseDown || chooseLeft == chooseRight);
            }
            if (rightMoveValid)
            {
                do
                {
                    chooseRight = Random.Range(0.0f, 1.0f);
                } while (chooseRight == chooseUp || chooseRight == chooseDown || chooseRight == chooseLeft);
            }


            // Choose the biggest number of the four (At least one will be greater than 0.0)
            float biggerNum1 = Mathf.Max(chooseUp, chooseDown);
            float biggerNum2 = Mathf.Max(biggerNum1, chooseLeft);
            float biggestNum = Mathf.Max(biggerNum2, chooseRight);

            // Match the biggest Num with the direction
            if (chooseUp == biggestNum)
            {
                currentZIndex++;
                directionFacing = (int)DirectionFacing.POS_Z;

            }
            else if (chooseDown == biggestNum)
            {
                currentZIndex--;
                directionFacing = (int)DirectionFacing.NEG_Z;
            }
            else if (chooseLeft == biggestNum)
            {
                currentXIndex--;
                directionFacing = (int)DirectionFacing.NEG_X;
            }
            else if (chooseRight == biggestNum)
            {
                currentXIndex++;
                directionFacing = (int)DirectionFacing.POS_X;
            }
            else
            {
                // Should not go through this path
                return false;
            }

            pattern.Add(planeOfPlatforms[currentXIndex][currentZIndex]);
            invalidSpot[currentZIndex, currentXIndex] = true;
            currentNumMoves++;

            if (currentZIndex == NUM_Z_PLATFORMS-1)
            {
                reachedValidEndpoint = true;
            }
            else
            {
                reachedValidEndpoint = false;
            }

            return true;
        }

        return false;
    }
}
