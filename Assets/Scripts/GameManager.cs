using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class that stores all current game data.
 * Static because the values are saved between scenes.
 */
public static class GameManager
{

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

    public static readonly uint TURN_MOVE_POINTS = 100;

    public static readonly uint FORWARD_MOVE_POINTS = 50;

    public static readonly uint DIFFICULTY_ONE_POINTS = 0;
    /**
     * Min Num moves aiming for if all moves were turn moves
     */
    public static readonly uint DIFFICULTY_TWO_POINTS = DIFFICULTY_ONE_POINTS + (4 * TURN_MOVE_POINTS);
    public static readonly uint DIFFICULTY_THREE_POINTS = DIFFICULTY_TWO_POINTS + (6 * TURN_MOVE_POINTS);
    public static readonly uint DIFFICULTY_FOUR_POINTS = DIFFICULTY_THREE_POINTS + (7 * TURN_MOVE_POINTS);
    public static readonly uint DIFFICULTY_FIVE_POINTS = DIFFICULTY_FOUR_POINTS + (9 * TURN_MOVE_POINTS);
    public static readonly uint DIFFICULTY_SIX_POINTS = DIFFICULTY_FIVE_POINTS + (11 * TURN_MOVE_POINTS);
    public static readonly uint DIFFICULTY_SEVEN_POINTS = DIFFICULTY_SIX_POINTS + (14 * TURN_MOVE_POINTS);
    public static readonly uint DIFFICULTY_EIGHT_POINTS = DIFFICULTY_SEVEN_POINTS + (16 * TURN_MOVE_POINTS);
    public static readonly uint DIFFICULTY_NINE_POINTS = DIFFICULTY_EIGHT_POINTS + (18 * TURN_MOVE_POINTS);
    public static readonly uint DIFFICULTY_TEN_POINTS = DIFFICULTY_NINE_POINTS + (22 * TURN_MOVE_POINTS);



    public enum DIFFICULTY
    {
        MEDIUM,
        HARD
    }

    public enum PATTERN_DIRECTION
    {
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

    public static int GAME_DIFFICULTY;

    /**
     * Integer - Stores Current Level Number
     */
    private static int currentLevelNumber;

    /**
     * Stores the number of lives remaining for current game
     */
    private static int numLives;

    /**
     * Stores the level difficulty for current game
     */
    private static int levelDifficulty;

    /**
     * Stores the current game score.
     */
    private static uint gameScore;

    /**
     * Stores the current number of x platforms in plane.
     */
    private static int numXPlatforms;

    /**
     * Stores the current number of x platforms in plane.
     */
    private static int numZPlatforms;

    /**
     * Stores the current level's pattern direction
     */
    private static int levelPatternDirection;

    /**
     * Stores whether or not the player failed the previous level.
     */
    private static bool failedPreviousAttempt;

    /**
     * 2D List - Inner List = [X-Index, Z-Index]
     */
    private static List<List<int>> previousPattern;

    private static Mode mode = Mode.new_game_setup;

    /**
     * Sets up all data that needs to be stored for a brand new game.
     */
    public static void StartNewGame()
    {
        mode = Mode.pattern_animation_showing;
        GAME_DIFFICULTY = PlayerPrefs.GetInt("Difficulty", (int)DIFFICULTY.MEDIUM);
        numLives = 3;
        currentLevelNumber = 1;
        levelDifficulty = 1;
        gameScore = 0;
        failedPreviousAttempt = false;
        previousPattern = null;
        DetermineGridSize();
        DeterminePatternDirection();
    }

    // This sets up a new level, but is continuing an already existing game
    public static void StartNewLevel()
    {
        mode = Mode.pattern_animation_showing;
        if (!failedPreviousAttempt)
        {
            currentLevelNumber++;
            if (levelDifficulty < 10)
            {
                switch (levelDifficulty)
                {
                    case (1):
                        if (gameScore >= DIFFICULTY_TWO_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    case (2):
                        if (gameScore >= DIFFICULTY_THREE_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    case (3):
                        if (gameScore >= DIFFICULTY_FOUR_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    case (4):
                        if (gameScore >= DIFFICULTY_FIVE_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    case (5):
                        if (gameScore >= DIFFICULTY_SIX_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    case (6):
                        if (gameScore >= DIFFICULTY_SEVEN_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    case (7):
                        if (gameScore >= DIFFICULTY_EIGHT_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    case (8):
                        if (gameScore >= DIFFICULTY_NINE_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    case (9):
                        if (gameScore >= DIFFICULTY_TEN_POINTS)
                        {
                            levelDifficulty++;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        DetermineGridSize();
        DeterminePatternDirection();
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
                numZPlatforms = 4;
                break;
            case (2):
                numXPlatforms = 4;
                numZPlatforms = 4;
                break;
            case (3):
                numXPlatforms = 4;
                numZPlatforms = 5;
                break;
            case (4):
                numXPlatforms = 5;
                numZPlatforms = 5;
                break;
            case (5):
                numXPlatforms = 5;
                numZPlatforms = 6;
                break;
            case (6):
                numXPlatforms = 6;
                numZPlatforms = 6;
                break;
            case (7):
                numXPlatforms = 6;
                numZPlatforms = 7;
                break;
            case (8):
                numXPlatforms = 7;
                numZPlatforms = 7;
                break;
            case (9):
                numXPlatforms = 7;
                numZPlatforms = 8;
                break;
            case (10):
                numXPlatforms = 8;
                numZPlatforms = 8;
                break;
            default:
                numXPlatforms = 8;
                numZPlatforms = 8;
                break;
        }
    }

    private static void DeterminePatternDirection()
    {
        if (GAME_DIFFICULTY == (int)DIFFICULTY.HARD)
        {
            int randInt = (int)Mathf.Floor(Random.Range(0, 1));
            if (randInt == 0)
            {
                levelPatternDirection = (int)PATTERN_DIRECTION.UP;
            }
            else
            {
                levelPatternDirection = (int)PATTERN_DIRECTION.DOWN;
            }
        }
        else
        {
            levelPatternDirection = (int)PATTERN_DIRECTION.UP;
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
     * Calculates score change for a forward move
     */
    public static void ForwardMove ()
    {
        AddPoints(FORWARD_MOVE_POINTS);
    }

    /*
     * Calculates score change for a turn move
    */
    public static void TurnMove()
    {
        AddPoints(TURN_MOVE_POINTS);
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

    /**
     * Saves the current pattern off in case it needs to be repeated (in case of player failure of level)
     */
    public static void SetPreviousPattern(List<Platform> thePattern)
    {

        previousPattern = new List<List<int>>();
        for (int i=0;i<thePattern.Count;i++)
        {
            List<int> tempInnerList = new List<int>();
            tempInnerList.Add(thePattern[i].GetXIndex());
            tempInnerList.Add(thePattern[i].GetZIndex());
            previousPattern.Add(tempInnerList);
        }
    }

    /**
     * Returns previous pattern
     */
    public static List<List<int>> GetPreviousPattern()
    {
        return previousPattern;
    }

    /**
     * Sets new high score with passed in value
     */
    public static int SetHighScore()
    {
        int newHighscoreSet = -1;
        uint score = GetCurrentNumPoints();
        int difficulty = GAME_DIFFICULTY;
        if (score > 0 && score <= 1000000)
        {
            uint[] currentLeaderboard = new uint[3];
            for (int i=0;i<3;i++)
            {
                currentLeaderboard[i] = (uint)PlayerPrefs.GetInt(difficulty.ToString() + "highscore" + (i+1), 0);
            }

            if (score >= currentLeaderboard[0])
            {
                // new high score
                currentLeaderboard[2] = currentLeaderboard[1];
                currentLeaderboard[1] = currentLeaderboard[0];
                currentLeaderboard[0] = score;
                newHighscoreSet = 1;
            }
            else if (score >= currentLeaderboard[1])
            {
                // new second high score
                currentLeaderboard[2] = currentLeaderboard[1];
                currentLeaderboard[1] = score;
                newHighscoreSet = 2;
            }
            else if (score >= currentLeaderboard[2])
            {
                currentLeaderboard[2] = score;
                newHighscoreSet = 3;
            }

            if (newHighscoreSet!=-1)
            {
                for (int i = 0; i < 3; i++)
                {
                    PlayerPrefs.SetInt(difficulty.ToString() + "highscore" + (i + 1), (int)currentLeaderboard[i]);
                }
            }
        }
        return newHighscoreSet;
    }

    public static void SetAndBumpHighScoreName(string highscoreName, int newRanking)
    {
        string[] currentNames = new string[3];
        int difficulty = GAME_DIFFICULTY;
        for (int i=0;i<3;i++)
        {
            currentNames[i] = PlayerPrefs.GetString(difficulty.ToString() + "highscoreName" + (i + 1), "***");
        }

        if (newRanking == 1)
        {
            currentNames[2] = currentNames[1];
            currentNames[1] = currentNames[0];
            currentNames[0] = highscoreName;
        }
        else if (newRanking == 2)
        {
            currentNames[2] = currentNames[1];
            currentNames[1] = highscoreName;
        }
        else if (newRanking == 3)
        {
            currentNames[2] = highscoreName;
        }

        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetString(difficulty.ToString() + "highscoreName" + (i + 1), currentNames[i]);
        }
    }

    public static int GetCurrentLevelNumber()
    {
        return currentLevelNumber;
    }

    public static int GetLevelPatternDirection()
    {
        return levelPatternDirection;
    }
}
