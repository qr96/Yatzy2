using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Toggle toggle;

    bool isOn = false;
    Action onChange;
    int score = -1;

    public void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
        scoreText.color = new Color(0f, 0f, 0f, 1f);
    }

    public void SetPrivewScore(int score)
    {
        scoreText.text = score.ToString();
        scoreText.color = new Color(0f, 0f, 0f, 0.5f);
    }

    public void InitScore()
    {
        score = -1;
        scoreText.text = "";
    }

    public int GetScore()
    {
        return score;
    }

    public void SetListener(Action onChange)
    {
        this.onChange = onChange;
    }

    public void SetToggleEnable(bool enable)
    {
        toggle.interactable = enable;
    }

    public void SetToggleOff()
    {
        toggle.SetIsOnWithoutNotify(false);
    }

    public bool IsOn()
    {
        return isOn;
    }

    public void OnToggle()
    {
        isOn = toggle.isOn;
        if (onChange != null) onChange();
    }
}
