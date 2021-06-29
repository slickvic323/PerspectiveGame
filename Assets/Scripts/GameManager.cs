using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that stores all current game data
 */
public static class GameManager
{
    private static int numLives;

    public static int currentLevelNumber;

    private static int levelDifficulty;

    private static uint gameScore;

    private static int numXPlatforms;

    private static int numZPlatforms;

    private static bool failedPreviousAttempt;

    /*
     * 2D List - Inner List = [X-Index, Z-Index]
     */
    private static List<List<int>> previousPattern;

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
        mode = Mode.pattern_animation_showing;
        numLives = 3;
        currentLevelNumber = 1;
        levelDifficulty = 1;
        gameScore = 0;
        failedPreviousAttempt = false;
        previousPattern = null;
        DetermineGridSize();
    }

    // This sets up a new level, but is continuing an already existing game
    public static void StartNewLevel()
    {
        mode = Mode.pattern_animation_showing;
        if (!failedPreviousAttempt)
        {
            if (levelDifficulty < 5)
            {
                levelDifficulty++;
            }
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
        AddPoints(50);
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

    /*
     * Returns number of lives remaining from current game
     */
    public static int GetNumLivesRemaining()
    {
        return numLives;
    }

    /*
     * Subtracts one life.
     */
    public static void SubtractNumLivesRemaining()
    {
        numLives--;
    }

    /*
     * Getter - Failed Previous Attempt
     */
    public static bool GetFailedPrevAttempt()
    {
        return failedPreviousAttempt;
    }

    /*
     * Setter - Failed Previous Attempt
     */
    public static void SetFailedPrevAttempt(bool failed)
    {
        failedPreviousAttempt = failed;
    }

    public static void SetPreviousPattern(List<Platform> thePattern)
    {

        previousPattern = new List<List<int>>();
        for (int i=0;i<thePattern.Count;i++)
        {
            List<int> tempInnerList = new List<int>();
            tempInnerList.Add(thePattern[i].GetXIndex());
            tempInnerList.Add(thePattern[i].GetZIndex());
            previousPattern.Add(tempInnerList);

            //Platform deepCopyPlatform = new Platform();
            //deepCopyPlatform.CreateGameObject();
            //Vector3 deepCopyPosition = new Vector3(thePattern[i].GetXIndex(), 0, thePattern[i].GetZIndex());
            ////deepCopyPlatform.SetPosition(thePattern[i].GetPosition());
            //deepCopyPlatform.SetPosition(deepCopyPosition);
            //previousPattern.Add(deepCopyPlatform);
        }
    }

    public static List<List<int>> GetPreviousPattern()
    {
        return previousPattern;
    }

    public static bool IsHighScore()
    {
        if (gameScore > PlayerPrefs.GetInt("highscore", 0))
        {
            return true;
        }
        return false;
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("highscore");
    }

    public static void SetHighScore(uint score)
    {
        if (score >= 0 && score <= 1000000)
        {
            PlayerPrefs.SetInt("highscore", (int)score);
        }
    }

    public static void HandleEndGamePoints()
    {
        // If current score is greater than highscore, then replace highscore with current score
        if (IsHighScore())
        {
            SetHighScore(gameScore);
        }
    }

}
