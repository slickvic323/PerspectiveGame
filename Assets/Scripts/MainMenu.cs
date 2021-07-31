using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    GameObject playButton;

    GameObject soundEffectsToggleButton;

    GameObject musicToggleButton;

    GameObject optionsMenu;

    GameObject highScoreMenu;

    GameObject tutorialMenu;

    GameObject titleFolder;

    Button difficultyButtonMedium, difficultyButtonHard;

    AudioManager audioManagerInstance;

    Sprite toggleOffSprite;

    Sprite toggleOnSprite;

    bool firstDifficultyClick;

    GameObject[] hsNormalPlayersText;
    GameObject[] hsNormalScoresText;
    GameObject[] hsHardPlayersText;
    GameObject[] hsHardScoresText;

    private enum DIFFICULTY
    {
        MEDIUM,
        HARD
    }

    private int difficultySelected;


    /**
     * Boolean value representing if sound effects are currently enabled
     */
    private bool soundEffectsEnabled;

    private bool musicEnabled;

    private GameObject patternVid, forwardVid, rightVid, leftVid, keepInMindImage;
    private GameObject slideNumText;
    private GameObject tutorialTextInstruction;
    private int currentTutorialSlideNum;

    private GameObject loadingVideoText;

    /**
     * Called when MainMenu Object is created
     */
    private void Start()
    {
        playButton = GameObject.Find("PlayButton");
        //playButton.GetComponent<Button>().GetComponent<Material>().color = Color.black;

        // This value is true until the difficulty button has been set. Prevents button click sound from playing on startup.
        firstDifficultyClick = true;

        optionsMenu = GameObject.Find("OptionsMenu");
        highScoreMenu = GameObject.Find("HighScoreMenu");
        tutorialMenu = GameObject.Find("TutorialMenu");
        titleFolder = GameObject.Find("Title Folder");
        audioManagerInstance = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        audioManagerInstance.Play("Menu_Music", 1f);
        soundEffectsEnabled = (PlayerPrefs.GetInt("SoundEffectsEnabled", 1) == 1);
        musicEnabled = (PlayerPrefs.GetInt("MusicEnabled", 1) == 1);
        toggleOnSprite = Resources.Load<Sprite>("Images/Toggle_On");
        toggleOffSprite = Resources.Load<Sprite>("Images/Toggle_Off");
        soundEffectsToggleButton = GameObject.FindWithTag("ToggleSoundEffects");
        musicToggleButton = GameObject.FindWithTag("ToggleMusic");
        optionsMenu.SetActive(false);

        hsNormalPlayersText = new GameObject[3];
        hsNormalScoresText = new GameObject[3];
        hsHardPlayersText = new GameObject[3];
        hsHardScoresText = new GameObject[3];
        // Set High Scores Game Objects
        for (int i = 0; i < 3; i++)
        {
            hsNormalPlayersText[i] = GameObject.Find("/Canvas/HighScoreMenu/NormalHighScores/Players/Player" + (i + 1) + "TextNormal");
        }
        for (int i = 0; i < 3; i++)
        {
            hsNormalScoresText[i] = GameObject.Find("Score" + (i + 1) + "TextNormal");
        }
        for (int i = 0; i < 3; i++)
        {
            hsHardPlayersText[i] = GameObject.Find("Player" + (i + 1) + "TextHard");
        }
        for (int i = 0; i < 3; i++)
        {
            hsHardScoresText[i] = GameObject.Find("Score" + (i + 1) + "TextHard");
        }
        highScoreMenu.SetActive(false);


        difficultyButtonMedium = GameObject.Find("DifficultyButton(Medium)").GetComponent<Button>();
        difficultyButtonHard = GameObject.Find("DifficultyButton(Hard)").GetComponent<Button>();

        difficultySelected = PlayerPrefs.GetInt("Difficulty", (int)DIFFICULTY.MEDIUM);
        switch(difficultySelected)
        {
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
                    ClickDifficultyButtonMedium();
                    break;
                }
        }
        // Allows button press sound to be played on all subsequent clicks of difficulty buttons
        firstDifficultyClick = false;

        difficultyButtonMedium.onClick.AddListener(ClickDifficultyButtonMedium);
        difficultyButtonHard.onClick.AddListener(ClickDifficultyButtonHard);

        // Tutorial Menu setup
        patternVid = GameObject.Find("Tutorial Pattern Video");
        forwardVid = GameObject.Find("Tutorial Go Forward Video");
        rightVid = GameObject.Find("Tutorial Go Right Video");
        leftVid = GameObject.Find("Tutorial Go Left Video");
        keepInMindImage = GameObject.Find("Image Keep in Mind");

        patternVid.SetActive(false);
        forwardVid.SetActive(false);
        rightVid.SetActive(false);
        leftVid.SetActive(false);
        keepInMindImage.SetActive(false);

        tutorialTextInstruction = GameObject.Find("Text Instruction");
        tutorialTextInstruction.GetComponent<TextMeshProUGUI>().text = "";

        slideNumText = GameObject.Find("Text Slide Number");
        loadingVideoText = GameObject.Find("Loading Video Text");
        loadingVideoText.SetActive(false);

        tutorialMenu.SetActive(false);
    }

    private void Update()
    {
        if (tutorialMenu.activeSelf)
        {
            // Check if should display loading text for video
            if (!loadingVideoText.activeSelf)
            {
                if ((patternVid.activeSelf && !patternVid.GetComponent<VideoPlayer>().isPlaying)
                    || (forwardVid.activeSelf && !forwardVid.GetComponent<VideoPlayer>().isPlaying)
                    || (rightVid.activeSelf && !rightVid.GetComponent<VideoPlayer>().isPlaying)
                    || (leftVid.activeSelf && !leftVid.GetComponent<VideoPlayer>().isPlaying))
                {
                    loadingVideoText.SetActive(true);
                }
            }
            else
            {
                // Check if should stop displaying loading text for video
                if ((patternVid.activeSelf && patternVid.GetComponent<VideoPlayer>().isPlaying)
                    || (forwardVid.activeSelf && forwardVid.GetComponent<VideoPlayer>().isPlaying)
                    || (rightVid.activeSelf && rightVid.GetComponent<VideoPlayer>().isPlaying)
                    || (leftVid.activeSelf && leftVid.GetComponent<VideoPlayer>().isPlaying))
                {
                    loadingVideoText.SetActive(false);
                }
            }

        }
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

    public void HighScoreMenuToggle()
    {
        if (soundEffectsEnabled)
        {
            FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
        }

        if (highScoreMenu.activeSelf)
        {
            //Load in the high score data
            for (int i = 0; i < 3; i++)
            {
                hsNormalPlayersText[i].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("1highscoreName" + (i + 1), "***");
            }
            for (int i = 0; i < 3; i++)
            {
                hsNormalScoresText[i].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("1highscore" + (i + 1), 0).ToString();
            }
            for (int i = 0; i < 3; i++)
            {
                hsHardPlayersText[i].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("2highscoreName" + (i + 1), "***");
            }
            for (int i = 0; i < 3; i++)
            {
                hsHardScoresText[i].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("2highscore" + (i + 1), 0).ToString();
            }
        }
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

    public void ClickDifficultyButtonMedium()
    {
        if (!firstDifficultyClick)
        {
            FindObjectOfType<AudioManager>().Play("Button_Press", 0.5f);
        }
        difficultySelected = (int)DIFFICULTY.MEDIUM;
        PlayerPrefs.SetInt("Difficulty", difficultySelected);

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

    public void TutorialMenuToggle()
    {
        if (tutorialMenu.activeSelf)
        {
            titleFolder.SetActive(false);

            currentTutorialSlideNum = 1;
            LoadSlide(currentTutorialSlideNum, -1);
            slideNumText.GetComponent<TextMeshProUGUI>().text = "1/5";
        }
        else
        {
            patternVid.GetComponent<VideoPlayer>().Stop();
            forwardVid.GetComponent<VideoPlayer>().Stop();
            rightVid.GetComponent<VideoPlayer>().Stop();
            leftVid.GetComponent<VideoPlayer>().Stop();
            patternVid.SetActive(false);
            forwardVid.SetActive(false);
            rightVid.SetActive(false);
            leftVid.SetActive(false);
            keepInMindImage.SetActive(false);

            titleFolder.SetActive(true);
        }
    }

    public void LoadSlide(int slideNum, int prevSlide)
    {
        if (prevSlide != -1)
        {
            switch (prevSlide)
            {
                case (1):
                    {
                        patternVid.GetComponent<VideoPlayer>().Stop();
                        patternVid.SetActive(false);
                        break;
                    }
                case (2):
                    {
                        forwardVid.GetComponent<VideoPlayer>().Stop();
                        forwardVid.SetActive(false); break;
                    }
                case (3):
                    {
                        rightVid.GetComponent<VideoPlayer>().Stop();
                        rightVid.SetActive(false);
                        break;
                    }
                case (4):
                    {
                        leftVid.GetComponent<VideoPlayer>().Stop();
                        leftVid.SetActive(false);
                        break;
                    }
                case (5):
                    {
                        keepInMindImage.SetActive(false);
                        break;
                    }
                default:
                    Debug.Log("Error with loading slides");
                    break;

            }
        }

        switch (slideNum)
        {
            case (1):
                {
                    patternVid.SetActive(true);
                    patternVid.GetComponent<VideoPlayer>().Play();
                    tutorialTextInstruction.GetComponent<TextMeshProUGUI>().text = "Memorize the Given Pattern. Ultimate Goal is to Re-Create Pattern.";
                    break;
                }
            case (2):
                {
                    forwardVid.SetActive(true);
                    forwardVid.GetComponent<VideoPlayer>().Play();
                    tutorialTextInstruction.GetComponent<TextMeshProUGUI>().text = "Swipe UP to Move Forward";
                    break;
                }
            case (3):
                {
                    rightVid.SetActive(true);
                    rightVid.GetComponent<VideoPlayer>().Play();
                    tutorialTextInstruction.GetComponent<TextMeshProUGUI>().text = "Swipe RIGHT to Turn Right";
                    break;
                }
            case (4):
                {
                    leftVid.SetActive(true);
                    leftVid.GetComponent<VideoPlayer>().Play();
                    tutorialTextInstruction.GetComponent<TextMeshProUGUI>().text = "Swipe LEFT to Turn Left";
                    break;
                }
            case (5):
                {
                    keepInMindImage.SetActive(true);
                    tutorialTextInstruction.GetComponent<TextMeshProUGUI>().text = "Keep in Mind How Many Lives Left and How Many Bounces Remain Before You Must Switch Platforms";
                    break;
                }
            default:
                Debug.Log("Error with loading slides");
                break;
        }
    }

    public void PreviousButtonClicked()
    {
        int prevSlideNum = currentTutorialSlideNum;
        currentTutorialSlideNum--;
        if (currentTutorialSlideNum < 1)
        {
            currentTutorialSlideNum = 5;
        }
        LoadSlide(currentTutorialSlideNum, prevSlideNum);
        slideNumText.GetComponent<TextMeshProUGUI>().text = currentTutorialSlideNum + "/5";
    }

    public void NextButtonClicked()
    {
        int prevSlideNum = currentTutorialSlideNum;
        currentTutorialSlideNum++;
        if (currentTutorialSlideNum > 5)
        {
            currentTutorialSlideNum = 1;
        }
        LoadSlide(currentTutorialSlideNum, prevSlideNum);
        slideNumText.GetComponent<TextMeshProUGUI>().text = currentTutorialSlideNum + "/5";
    }
}
