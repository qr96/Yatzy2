using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceToggle : MonoBehaviour
{
    public Button toggle;
    public TextMeshProUGUI diceNum;
    public GameObject selected;
    public DiceViewer diceViewer;

    Action onClick;
    bool isOn;

    public void SetClickEvent(Action onClick)
    {
        this.onClick = onClick;
    }

    public void SetDice(int diceNum)
    {
        this.diceNum.text = diceNum.ToString();
    }

    public void ToggleOn(bool isOn)
    {
        this.isOn = isOn;
        selected.SetActive(isOn);
    }

    public void EnableToggle(bool enable)
    {
        toggle.enabled = enable;
    }

    public bool IsLocked()
    {
        return isOn;
    }

    public void OnClickButton()
    {
        isOn = !isOn;
        selected.SetActive(isOn);
        if (onClick != null) onClick();
    }
}
