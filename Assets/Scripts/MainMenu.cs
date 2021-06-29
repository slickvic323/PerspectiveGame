using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private GameObject highScoreText;

    private void Start()
    {
        FillInHighScoreText();
    }

    public void PlayGame ()
    {
        GameManager.SetMode(GameManager.Mode.new_game_setup);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FillInHighScoreText ()
    {
        highScoreText = GameObject.Find("HighScoreText");
        highScoreText.GetComponent<Text>().text = "HighScore: " + PlayerPrefs.GetInt("highscore", 0).ToString() + " pts";
    }

}
