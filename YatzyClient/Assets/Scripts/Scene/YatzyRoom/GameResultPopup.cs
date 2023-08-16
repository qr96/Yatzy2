using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResultPopup : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    public void ShowResult(bool draw, bool win)
    {
        if (draw) resultText.text = "���º�";
        else if (win) resultText.text = "�¸�";
        else resultText.text = "�й�";

        gameObject.SetActive(true);
    }

    public void OnClickRestart()
    {
        ToS_ReadyToStart req = new ToS_ReadyToStart();
        NetworkManager.Instance.Send(req.Write());

        gameObject.SetActive(false);
    }

    public void OnClickLeave()
    {
        ErrorManager.Instance.ShowLoadingIndicator();

        ToS_ReqLeaveRoom packet = new ToS_ReqLeaveRoom();
        NetworkManager.Instance.Send(packet.Write());
    }
}
