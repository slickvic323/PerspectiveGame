using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowHandler : MonoBehaviour
{
    GameObject directionArrowsUI;

    GameObject upArrowObject;
    GameObject rightArrowObject;
    GameObject leftArrowObject;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

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

    public void ShowUpArrow()
    {
        upArrowObject.SetActive(true);
    }

    public void ShowRightArrow()
    {
        rightArrowObject.SetActive(true);
    }

    public void ShowLeftArrow()
    {
        leftArrowObject.SetActive(true);
    }

    public void HideUpArrow()
    {
        upArrowObject.SetActive(false);
    }

    public void HideRightArrow()
    {
        rightArrowObject.SetActive(false);
    }

    public void HideLeftArrow()
    {
        leftArrowObject.SetActive(false);
    }
}
