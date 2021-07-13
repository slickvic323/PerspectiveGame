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

    Button difficultyButtonEasy, difficultyButtonMedium, difficultyButtonHard;

    AudioManager audioManagerInstance;

    Sprite toggleOffSprite;

    Sprite toggleOnSprite;

    bool firstDifficultyClick;

    private enum DIFFICULTY
    {
        EASY,
        MEDIUM,
        HARD
    }

    private int difficultySelected;


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
        // This value is true until the difficulty button has been set. Prevents button click sound from playing on startup.
        firstDifficultyClick = true;

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


        difficultyButtonEasy = GameObject.Find("DifficultyButton(Easy)").GetComponent<Button>();
        difficultyButtonMedium = GameObject.Find("DifficultyButton(Medium)").GetComponent<Button>();
        difficultyButtonHard = GameObject.Find("DifficultyButton(Hard)").GetComponent<Button>();

        difficultySelected = PlayerPrefs.GetInt("Difficulty", (int)DIFFICULTY.EASY);
        switch(difficultySelected)
        {
            case ((int)DIFFICULTY.EASY):
                {
                    ClickDifficultyButtonEasy();
                    break;
                }
            case ((int)DIFFICULTY.MEDIUM):
                {
                    ClickDifficultyButtonMedium();
                    break;
                }
            case ((int)DIFFICULTY.HARD):
                {
                    ClickDifficultyButtonHard();
                    break;
                }
            default:
                {
                    ClickDifficultyButtonEasy();
                    break;
                }
        }
        // Allows button press sound to be played on all subsequent clicks of difficulty buttons
        firstDifficultyClick = false;

        difficultyButtonEasy.onClick.AddListener(ClickDifficultyButtonEasy);
        difficultyButtonMedium.onClick.AddListener(ClickDifficultyButtonMedium);
        difficultyButtonHard.onClick.AddListener(ClickDifficultyButtonHard);

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

    public void ClickDifficultyButtonEasy()
    {
        if (!firstDifficultyClick)
        {
            FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
        }
        difficultySelected = (int)DIFFICULTY.EASY;
        PlayerPrefs.SetInt("Difficulty", difficultySelected);

        SelectDifficultyButton(difficultyButtonEasy);
        DeselectDifficultyButton(difficultyButtonMedium);
        DeselectDifficultyButton(difficultyButtonHard);
    }

    public void ClickDifficultyButtonMedium()
    {
        if (!firstDifficultyClick)
        {
            FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
        }
        difficultySelected = (int)DIFFICULTY.MEDIUM;
        PlayerPrefs.SetInt("Difficulty", difficultySelected);

        DeselectDifficultyButton(difficultyButtonEasy);
        SelectDifficultyButton(difficultyButtonMedium);
        DeselectDifficultyButton(difficultyButtonHard);

    }

    public void ClickDifficultyButtonHard()
    {
        if (!firstDifficultyClick)
        {
            FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
        }
        difficultySelected = (int)DIFFICULTY.HARD;
        PlayerPrefs.SetInt("Difficulty", difficultySelected);

        DeselectDifficultyButton(difficultyButtonEasy);
        DeselectDifficultyButton(difficultyButtonMedium);
        SelectDifficultyButton(difficultyButtonHard);
    }

    private void SelectDifficultyButton(Button selected)
    {
        ColorBlock colorBlock = selected.colors;
        colorBlock.normalColor = new Color(50f / 255f, 220f / 255f, 50f / 255f);
        selected.colors = colorBlock;
    }

    private void DeselectDifficultyButton(Button deselected)
    {
        ColorBlock colorBlock = deselected.colors;
        colorBlock.normalColor = new Color(1f, 1f, 1f);
        deselected.colors = colorBlock;
    }

}
