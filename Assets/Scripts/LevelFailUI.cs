using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFailUI : MonoBehaviour
{

    AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    /** 
     * Loads Main Menu Scene
     */
    public void GoToMainMenu()
    {
        audioManager.Stop("Menu_Music");
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
        audioManager.Stop("Menu_Music");
        GameManager.SetMode(GameManager.Mode.new_game_setup);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
