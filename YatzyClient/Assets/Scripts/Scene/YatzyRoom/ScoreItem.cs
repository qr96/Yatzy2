using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour
{
    public TextMeshProUGUI score;
    public Toggle toggle;

    bool isOn = false;
    Action onChange;

    public void SetScore(string score)
    {
        this.score.text = score;
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
