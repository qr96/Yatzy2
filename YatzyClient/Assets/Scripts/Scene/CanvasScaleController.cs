using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScaleController : MonoBehaviour
{
    public CanvasScaler canvasScaler;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
#if UNITY_ANDROID
        canvasScaler.scaleFactor = 1.0f;
#elif UNITY_IOS
        canvasScaler.scaleFactor = 1.0f;
#else
        canvasScaler.scaleFactor = 0.5f;
#endif
    }
}
