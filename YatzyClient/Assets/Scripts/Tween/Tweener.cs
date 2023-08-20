using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tweener : MonoBehaviour
{
    public RectTransform target;
    public int loop;

    public Vector3 from;
    public Vector3 to;
    public Color fromColor;
    public Color toColor;
    public float duration;
    public float delay;
    public AnimationCurve curve;
    public TweenType tweenType;

    public enum TweenType
    {
        Size = 0,
        Color = 1,
    }

    TweenerCore<Vector2, Vector2, VectorOptions> tween;

    void OnEnable()
    {
        if (tweenType == TweenType.Size) DoSizeTween();
        else if (tweenType == TweenType.Color) DoColorTween();
    }

    private void OnDisable()
    {
        tween.Kill();
    }

    void DoSizeTween()
    {
        target.sizeDelta = from;
        tween = target.DOSizeDelta(to, duration)
            .SetDelay(delay)
            .SetEase(curve)
            .SetLoops(loop);
    }

    void DoColorTween()
    {
        var image = target.GetComponent<Image>();
        image.color = fromColor;
        image.DOColor(toColor, duration)
            .SetDelay(delay)
            .SetEase(curve)
            .SetLoops(loop);
    }
}
