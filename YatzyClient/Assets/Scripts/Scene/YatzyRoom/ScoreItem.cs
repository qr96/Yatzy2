using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Button button;
    public GameObject selected;
    public GameObject turn;

    bool isOn = false;
    Action onClick;
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

    public void SetListener(Action onClick)
    {
        this.onClick = onClick;
    }

    public void SetToggleEnable(bool enable)
    {
        button.interactable = enable;
    }

    public void SetToggleOn(bool isOn)
    {
        this.isOn = isOn;
        selected.SetActive(isOn);
    }

    public void SetTurnLight(bool isActive)
    {
        turn.SetActive(isActive);
    }

    public bool IsOn()
    {
        return isOn;
    }

    public void OnClick()
    {
        if (onClick != null) onClick();
    }
}
