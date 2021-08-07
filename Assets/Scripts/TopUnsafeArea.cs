using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUnsafeArea : MonoBehaviour
{
    RectTransform rectTransform;
    Rect safeArea;
    Vector2 minAnchor;
    Vector2 maxAnchor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        safeArea = Screen.safeArea;
        minAnchor = new Vector2(0f, (safeArea.y+safeArea.height)/Screen.height);

        rectTransform.anchorMin = minAnchor;
    }
}
