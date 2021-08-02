using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteUI : MonoBehaviour
{

    AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    /** 
     * Loads New Level Scene
     */
    public void ProceedToNextLevel()
    {
        audioManager.Stop("Between_Levels_Drums");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
