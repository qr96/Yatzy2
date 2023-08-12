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

    bool isOn;

    public void SetDice(int diceNum)
    {
        this.diceNum.text = diceNum.ToString();
    }

    public void UnSelectToggle()
    {
        isOn = false;
        selected.SetActive(false);
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
    }
}
