using UnityEngine;

/*
 * Class that handles the arrows that show up to give player information about the registered swipe and direction the ball will go after bouncing on platform.
 */
public class ArrowHandler : MonoBehaviour
{
    private GameObject directionArrowsUI;

    private GameObject upArrowObject;
    private GameObject rightArrowObject;
    private GameObject leftArrowObject;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * Initializes Arrows. Initially none are visible.
     */
    public void SetUpArrows()
    {
        directionArrowsUI = GameObject.Find("DirectionArrowsUI");
        upArrowObject = GameObject.Find("UpArrowImage");
        rightArrowObject = GameObject.Find("RightArrowImage");
        leftArrowObject = GameObject.Find("LeftArrowImage");

        HideUpArrow();
        HideRightArrow();
        HideLeftArrow();
    }

    /**
     * Up Arrow Visible.
    */
    public void ShowUpArrow()
    {
        upArrowObject.SetActive(true);
    }

    /**
     * Right Arrow Visible.
    */
    public void ShowRightArrow()
    {
        rightArrowObject.SetActive(true);
    }

    /**
     * Left Arrow Visible.
    */
    public void ShowLeftArrow()
    {
        leftArrowObject.SetActive(true);
    }

    /**
     * Up Arrow Invisible.
     */
    public void HideUpArrow()
    {
        upArrowObject.SetActive(false);
    }

    /**
     * Right Arrow Invisible.
     */
    public void HideRightArrow()
    {
        rightArrowObject.SetActive(false);
    }

    /**
     * Left Arrow Invisible.
     */
    public void HideLeftArrow()
    {
        leftArrowObject.SetActive(false);
    }
}
