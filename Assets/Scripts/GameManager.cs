using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that stores all current game data
 */
public static class GameManager
{
    public static int numLives;

    public static int currentLevelNumber;

    private static int levelDifficulty;

    private static uint gameScore;

    private static int numXPlatforms;

    private static int numZPlatforms;

    private static Mode mode = Mode.new_game_setup;

    /**
     * Game is always in one of these pre-set Modes 
     **/
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

    /**
     * Sets up all data that needs to be stored for a brand new game.
     */
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

    /*
     * Sets the grid size based on the given level difficulty
     */
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

    /*
     * Returns Current Level Difficulty
     */
    public static int GetLevelDifficulty()
    {
        return levelDifficulty;
    }

    /*
     * Adds given points to current score
     */
    private static void AddPoints (uint additionalPoints)
    {
        gameScore += additionalPoints;
    }

    /*
     * Subtracts given points from current score
     */
    private static void SubtractPoints (uint numPointsLess)
    {
        gameScore -= numPointsLess;
    }

    /*
     * Calculates score change for a forward move
     */
    public static void ForwardMove ()
    {
        AddPoints(75);
    }

    /*
     * Calculates score change for a turn move
    */
    public static void TurnMove()
    {
        AddPoints(100);
    }

    /*
     * Returns the current number of points
     */
    public static uint GetCurrentNumPoints ()
    {
        return gameScore;
    }

    /*
     * Returns the number of platforms in the X Direction
     */
    public static int GetNumXPlatforms ()
    {
        return numXPlatforms;
    }

    /*
     * Returns the number of platforms in the Z Direction
     */
    public static int GetNumZPlatforms ()
    {
        return numZPlatforms;
    }

    /*
     * Returns the current Game Mode
     */
    public static Mode GetMode ()
    {
        return mode;
    }

    /*
     * Sets the game mode.
     */
    public static void SetMode(Mode newMode)
    {
        mode = newMode;
    }
}
