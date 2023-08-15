using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResultPopup : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    public void ShowResult(bool win)
    {
        if (win) resultText.text = "�¸�";
        else resultText.text = "�й�";
    }

    public void OnClickRestart()
    {

    }

    public void OnClickLeave()
    {

    }
}
