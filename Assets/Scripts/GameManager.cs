using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{

    public static int numLives;

    public static int currentLevelNumber;

    private static int levelDifficulty;

    private static uint gameScore;

    private static int numXPlatforms;

    private static int numZPlatforms;

    private static Mode mode = Mode.new_game_setup;

    public enum Mode
    {
        main_menu,
        gameplay,
        pause_menu,
        failed_level,
        completed_level,
        new_game_setup,
        pattern_animation_showing
    }


    public static void StartNewGame()
    {
        mode = Mode.gameplay;
        numLives = 3;
        currentLevelNumber = 1;
        levelDifficulty = 1;
        gameScore = 0;
        DetermineGridSize();
    }

    // This sets up a new level, but is continuing an already existing game
    public static void StartNewLevel()
    {
        if (levelDifficulty<5)
        {
            levelDifficulty++;
        }
        DetermineGridSize();
    }

    private static void DetermineGridSize()
    {
        switch (levelDifficulty)
        {
            case (1):
                numXPlatforms = 3;
                numZPlatforms = 3;
                break;
            case (2):
                numXPlatforms = 3;
                numZPlatforms = 4;
                break;
            case (3):
                numXPlatforms = 4;
                numZPlatforms = 4;
                break;
            case (4):
                numXPlatforms = 4;
                numZPlatforms = 5;
                break;
            case (5):
                numXPlatforms = 5;
                numZPlatforms = 5;
                break;
            default:
                numXPlatforms = 5;
                numZPlatforms = 5;
                break;
        }
    }

    public static int GetLevelDifficulty()
    {
        return levelDifficulty;
    }

    private static void AddPoints (uint additionalPoints)
    {
        gameScore += additionalPoints;
    }

    private static void SubtractPoints (uint numPointsLess)
    {
        gameScore -= numPointsLess;
    }

    public static void ForwardMove ()
    {
        AddPoints(75);
    }

    public static void TurnMove()
    {
        AddPoints(100);
    }

    public static uint GetCurrentNumPoints ()
    {
        return gameScore;
    }

    public static int GetNumXPlatforms ()
    {
        return numXPlatforms;
    }

    public static int GetNumZPlatforms ()
    {
        return numZPlatforms;
    }

    public static Mode GetMode ()
    {
        return mode;
    }
}
