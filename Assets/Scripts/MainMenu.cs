using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /**
     * Text Game Object representing High Score
     */
    GameObject highScoreText;

    GameObject soundEffectsToggleButton;

    GameObject musicToggleButton;

    GameObject optionsMenu;

    AudioManager audioManagerInstance;

    Sprite toggleOffSprite;

    Sprite toggleOnSprite;


    /**
     * Boolean value representing if sound effects are currently enabled
     */
    private bool soundEffectsEnabled;

    private bool musicEnabled;

    /**
     * Called when MainMenu Object is created
     */
    private void Start()
    {
        optionsMenu = GameObject.Find("OptionsMenu");
        audioManagerInstance = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        audioManagerInstance.Play("Menu_Music", 1);
        soundEffectsEnabled = (PlayerPrefs.GetInt("SoundEffectsEnabled", 1) == 1);
        musicEnabled = (PlayerPrefs.GetInt("MusicEnabled", 1) == 1);
        toggleOnSprite = Resources.Load<Sprite>("Images/Toggle_On");
        toggleOffSprite = Resources.Load<Sprite>("Images/Toggle_Off");
        soundEffectsToggleButton = GameObject.FindWithTag("ToggleSoundEffects");
        musicToggleButton = GameObject.FindWithTag("ToggleMusic");
        optionsMenu.SetActive(false);
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
        FindObjectOfType<AudioManager>().Stop("Menu_Music");
        //FindObjectOfType<AudioManager>().SetVolume("Menu_Music", 0.1f);
    }

    /**
     * Plays Button Press Sound when toggling Options Menu
     */
    public void OptionsMenuToggle()
    {
        if (soundEffectsEnabled)
        {
            FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
        }
        RefreshSoundEffectsButton();
        RefreshMusicButton();
    }

    /**
     * Retrieves High Score and fills in Text Object
     */
    public void FillInHighScoreText ()
    {
        highScoreText = GameObject.Find("HighScoreText");
        highScoreText.GetComponent<Text>().text = "HighScore: " + PlayerPrefs.GetInt("highscore", 0).ToString() + " pts";
    }

    public void ToggleSoundEffects()
    {
        soundEffectsEnabled = !soundEffectsEnabled;
        if (soundEffectsEnabled)
        {
            PlayerPrefs.SetInt("SoundEffectsEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SoundEffectsEnabled", 0);
        }

        RefreshSoundEffectsButton();
        audioManagerInstance.RefreshSoundEffectsEnabled();

        if (soundEffectsEnabled)
        {
            FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
        }

    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;

        if (musicEnabled)
        {
            PlayerPrefs.SetInt("MusicEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("MusicEnabled", 0);
        }

        RefreshMusicButton();
        audioManagerInstance.RefreshMusicEnabled();

        if (musicEnabled)
        {
            FindObjectOfType<AudioManager>().Play("Menu_Music", 1f);
        }
        else
        {
            FindObjectOfType<AudioManager>().Stop("Menu_Music");
        }

        if (soundEffectsEnabled)
        {
            FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
        }
    }

    public void RefreshSoundEffectsButton()
    {
        if (optionsMenu.activeSelf)
        {
            if (soundEffectsEnabled)
            {
                soundEffectsToggleButton.GetComponent<Image>().sprite = toggleOnSprite;
            }
            else
            {
                soundEffectsToggleButton.GetComponent<Image>().sprite = toggleOffSprite;
            }
        }
    }

    public void RefreshMusicButton()
    {
        if (optionsMenu.activeSelf)
        {
            if (musicEnabled)
            {
                musicToggleButton.GetComponent<Image>().sprite = toggleOnSprite;
            }
            else
            {
                musicToggleButton.GetComponent<Image>().sprite = toggleOffSprite;
            }
        }
    }

}
