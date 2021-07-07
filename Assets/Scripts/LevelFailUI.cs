using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFailUI : MonoBehaviour
{
    /** 
     * Loads Main Menu Scene
     */
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /** 
     * Re-loads current level with same pattern
     */
    public void ReloadSameLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /**
     * Loads New Game
     */
    public void RestartGame()
    {
        GameManager.SetMode(GameManager.Mode.new_game_setup);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
