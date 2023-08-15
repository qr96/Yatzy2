using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResultPopup : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    public void ShowResult(bool win)
    {
        if (win) resultText.text = "½Â¸®";
        else resultText.text = "ÆÐ¹è";
    }

    public void OnClickRestart()
    {

    }

    public void OnClickLeave()
    {

    }
}
