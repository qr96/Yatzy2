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
        if (result == GameResult.Draw) resultText.text = "���º�";
        else if (result == GameResult.Win) resultText.text = "�¸�";
        else resultText.text = "�й�";

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
