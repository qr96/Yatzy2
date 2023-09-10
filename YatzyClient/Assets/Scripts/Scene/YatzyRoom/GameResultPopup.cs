using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResultPopup : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    Action onClickRestart;
    Action onClickLeave;

    public void SetRestartBtnListener(Action onClick)
    {
        onClickRestart = onClick;
    }

    public void SetLeaveBtnListener(Action onClick)
    {
        onClickLeave = onClick;
    }

    public void ShowResult(GameResult result)
    {
        if (result == GameResult.Draw) resultText.text = "무승부";
        else if (result == GameResult.Win) resultText.text = "승리";
        else resultText.text = "패배";

        gameObject.SetActive(true);
    }

    public void OnClickRestart()
    {
        ToS_ReadyToStart req = new ToS_ReadyToStart();
        NetworkManager.Instance.Send(req.Write());

        if (onClickRestart != null) onClickRestart();

        gameObject.SetActive(false);
    }

    public void OnClickLeave()
    {
        ErrorManager.Instance.ShowLoadingIndicator();
        if (onClickLeave != null) onClickLeave();
    }
}
