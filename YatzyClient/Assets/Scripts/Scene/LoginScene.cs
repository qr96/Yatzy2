using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : MonoBehaviour
{
    public TextMeshProUGUI nameInput;

    private void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResLogin, OnRecvLogin);
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_ResLogin);
    }

    public void OnClickLogin()
    {
        if (nameInput.text.Length <= 2)
        {
            ErrorManager.Instance.ShowPopup("안내", "닉네임은 2글자 이상 입력해주세요.", null);
            return;
        }

        StartCoroutine(WaitForConnect());
    }

    void OnRecvLogin(IPacket packet)
    {
        Debug.Log("RecvLogin() ");

        ToC_ResLogin res = packet as ToC_ResLogin;
        if (res.loginSuccess)
            SceneManager.LoadScene(1);
    }

    void SendLogin()
    {
        ToS_ReqLogin req = new ToS_ReqLogin();
        req.nickName = nameInput.text;
        NetworkManager.Instance.Send(req.Write());
    }

    IEnumerator WaitForConnect()
    {
        float waitTime = 0f;

        NetworkManager.Instance.ConnectToServer();
        ErrorManager.Instance.ShowLoadingIndicator();

        while (NetworkManager.Instance._connected == false)
        {
            yield return new WaitForSeconds(0.5f);
            waitTime += 0.5f;
            if (waitTime > 10f)
            {
                ErrorManager.Instance.HideLoadingIndicator();
                ErrorManager.Instance.ShowPopup("안내", "서버와의 연결에 실패했습니다.");
                yield break;
            }
        }
        SendLogin();
    }
}
