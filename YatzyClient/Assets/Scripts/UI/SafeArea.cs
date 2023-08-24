using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    public RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rect != null) ApplySafeAreaPosition(rect);
    }

    public static void ApplySafeAreaPosition(RectTransform rt)
    {
        Rect safeArea = Screen.safeArea;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x = rt.anchorMin.x;
        anchorMax.x = rt.anchorMax.x;

        anchorMin.y = 0f;
        anchorMax.y /= Screen.height;

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
    }
}
