using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /**
     * Text Game Object representing High Score
     */
    private GameObject highScoreText;

    /**
     * Called when MainMenu Object is created
     */
    private void Start()
    {
        FillInHighScoreText();
    }

    /**
     * Changes Scene to Game Scene
     */
    public void PlayGame()
    {
        GameManager.SetMode(GameManager.Mode.new_game_setup);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
    }

    /**
     * Plays Button Press Sound when toggling Options Menu
     */
    public void OptionsMenuToggle()
    {
        FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
    }

    /**
     * Retrieves High Score and fills in Text Object
     */
    public void FillInHighScoreText ()
    {
        highScoreText = GameObject.Find("HighScoreText");
        highScoreText.GetComponent<Text>().text = "HighScore: " + PlayerPrefs.GetInt("highscore", 0).ToString() + " pts";
    }

}
