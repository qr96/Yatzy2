using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour
{
    public GameObject selectMark;
    public TextMeshProUGUI score;
    public Button button;

    Action onClick;

    public void SetScore(string score)
    {
        this.score.text = score;
    }

    public void AddListener(Action onClick)
    {
        this.onClick = onClick;
    }

    public void SetButtonEnable(bool enable)
    {
        button.interactable = enable;
    }

    public void Select()
    {
        selectMark.SetActive(true);
    }

    public void UnSelect()
    {
        selectMark.SetActive(false);
    }

    public void OnClick()
    {
        if (onClick != null) onClick();
    }
}
