﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern
{
    /**
     * Stores the ArrayList of Platforms in the order of the pattern.
     */
    private List<Platform> pattern;

    /**
     * Stores the number of x platforms that exist in plane.
     */
    private int NUM_X_PLATFORMS;

    /**
     * Stores the number of z platforms that exist in plane.
     */
    private int NUM_Z_PLATFORMS;

    /**
     * Stores the minimum number of possible moves in a pattern for the current plane of platforms.
    */
    private int MAX_POSSIBLE_NUM_MOVES;

    /**
     * Stores integer value for the minimum number of platforms in pattern that is allowed.
     */
    private int MIN_AIMING_FOR;

    /**
     * Stores integer value for the minimum number of platforms in pattern that is allowed.
     */
    private int MAX_AIMING_FOR;

    /**
     * Stores the current number of moves in the pattern.
     */
    private int currentNumMoves;

    /**
     * Boolean value is true if a valid end point to the pattern has been reached.
     */
    private bool reachedValidEndpoint;

    /**
     * Array of booleans representing if a platform is invalid to be added to the pattern.
     */
    private bool[,] invalidSpot;

    /**
     * Stores X Index of current platform in pattern.
     */
    private int currentXIndex;

    /**
     * Stores Z Index of current platform in pattern.
     */
    private int currentZIndex;

    /**
     * Enum representing the different directions that user is facing.
     */
    private enum DirectionFacing
    {
        POS_Z = 0,
        POS_X = 1,
        NEG_Z = 2,
        NEG_X = 3
    }

    /**
     * Stores integer that represents current direction facing.
     */
    private int directionFacing;

    /**
     * Boolean representing if platform can be added.
     */
    private bool canAddPlatform;

    /**
     * Creates pattern when given the number of x and z platforms.
     * Returns true if pattern is valid.
     * Returns false if pattern is invalid.
     */
    public bool CreatePattern(int patternDirection, List<List<Platform>> planeOfPlatforms, int numXPlatforms, int numZPlatforms)
    {
        currentNumMoves = 0;
        reachedValidEndpoint = false;

        invalidSpot = new bool[numZPlatforms, numXPlatforms];
        
        NUM_X_PLATFORMS = numXPlatforms;
        NUM_Z_PLATFORMS = numZPlatforms;

        MAX_POSSIBLE_NUM_MOVES = (numXPlatforms * numZPlatforms) - 1;

        MIN_AIMING_FOR = (int) Mathf.Floor((float)MAX_POSSIBLE_NUM_MOVES / 2.5f);
        MAX_AIMING_FOR = (int) Mathf.Floor((float)MAX_POSSIBLE_NUM_MOVES / 1.25f);

        pattern = new List<Platform>();

        int startingX, startingZ;
        if (patternDirection == (int)GameManager.PATTERN_DIRECTION.UP)
        {
            // Get Starting Point X value (Z=0)
            startingX = (int)Mathf.Floor(Random.Range(0, NUM_X_PLATFORMS));
            startingZ = 0;
            directionFacing = (int)DirectionFacing.POS_Z;
        }
        else if (patternDirection == (int)GameManager.PATTERN_DIRECTION.DOWN)
        {
            // Get Starting Point X value (Z=NUM_Z_PLATFORMS - 1)
            startingX = (int)Mathf.Floor(Random.Range(0, NUM_X_PLATFORMS));
            startingZ = NUM_Z_PLATFORMS - 1;
            directionFacing = (int)DirectionFacing.NEG_Z;
        }
        else if (patternDirection == (int)GameManager.PATTERN_DIRECTION.RIGHT)
        {
            // Get Starting Point Z value (X=0)
            startingX = 0;
            startingZ = (int)Mathf.Floor(Random.Range(0, NUM_Z_PLATFORMS));
            directionFacing = (int)DirectionFacing.POS_X;
        }
        else
        {
            // Pattern direction is LEFT
            // Get Starting Point Z value (X=NUM_X_PLATFORMS - 1)
            startingX = NUM_X_PLATFORMS - 1;
            startingZ = (int)Mathf.Floor(Random.Range(0, NUM_Z_PLATFORMS));
            directionFacing = (int)DirectionFacing.NEG_X;
        }

        canAddPlatform = true;
        pattern.Add(planeOfPlatforms[startingX][startingZ]);
        currentXIndex = startingX;
        currentZIndex = startingZ;
        invalidSpot[currentZIndex, currentXIndex] = true;

        // Add platforms to the pattern until reached the minimum length of pattern wanted
        while (canAddPlatform && currentNumMoves < MIN_AIMING_FOR)
        {
            canAddPlatform = AddPlatformToPattern(patternDirection, planeOfPlatforms);
        }

        while (canAddPlatform && !reachedValidEndpoint && currentNumMoves <= MAX_AIMING_FOR)
        {
            canAddPlatform = AddPlatformToPattern(patternDirection, planeOfPlatforms);
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

    /**
     * Sets up pattern to match given previous pattern values.
     * Returns true if pattern is valid.
     * Returns false, otherwise.
     */
    public bool ResetPreviousPattern(List<List<Platform>> planeOfPlatforms, int numXPlatforms, int numZPlatforms, List<List<int>> previousPattern)
    {
        currentNumMoves = 0;
        reachedValidEndpoint = false;

        invalidSpot = new bool[numZPlatforms, numXPlatforms];

        NUM_X_PLATFORMS = numXPlatforms;
        NUM_Z_PLATFORMS = numZPlatforms;

        MAX_POSSIBLE_NUM_MOVES = (numXPlatforms * numZPlatforms) - 1;

        MIN_AIMING_FOR = (int)Mathf.Floor((float)MAX_POSSIBLE_NUM_MOVES / 2.5f);
        MAX_AIMING_FOR = (int)Mathf.Floor((float)MAX_POSSIBLE_NUM_MOVES / 1.25f);

        pattern = new List<Platform>();
        for (int i = 0; i < previousPattern.Count; i++)
        {
            pattern.Add(planeOfPlatforms[previousPattern[i][0]][previousPattern[i][1]]);
        }

        foreach (Platform platform in pattern)
        {
            // platform.GetGameObject().GetComponent<Renderer>().material.color = Color.blue;
            platform.SetInPattern(true);
        }

        return true;
    }

    /**
     * Returns pattern.
     */
    public List<Platform> GetPattern()
    {
        return pattern;
    }

    /**
     * Adds a new platform to the already existing platform.
     */
    private bool AddPlatformToPattern(int patternDirection, List<List<Platform>> planeOfPlatforms)
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

            // See if reached valid endpoint (on opposite side from grid start point) based on pattern direction
            if (patternDirection == (int)GameManager.PATTERN_DIRECTION.UP)
            {
                if (currentZIndex == NUM_Z_PLATFORMS - 1)
                {
                    reachedValidEndpoint = true;
                }
                else
                {
                    reachedValidEndpoint = false;
                }
            }
            else if (patternDirection == (int)GameManager.PATTERN_DIRECTION.DOWN)
            {
                if (currentZIndex == 0)
                {
                    reachedValidEndpoint = true;
                }
                else
                {
                    reachedValidEndpoint = false;
                }
            }
            else if (patternDirection == (int)GameManager.PATTERN_DIRECTION.RIGHT)
            {
                if (currentXIndex == NUM_X_PLATFORMS - 1)
                {
                    reachedValidEndpoint = true;
                }
                else
                {
                    reachedValidEndpoint = false;
                }
            }
            else
            {
                // Pattern Direction is LEFT
                if (currentXIndex == 0)
                {
                    reachedValidEndpoint = true;
                }
                else
                {
                    reachedValidEndpoint = false;
                }
            }

            return true;
        }

        return false;
    }
}
