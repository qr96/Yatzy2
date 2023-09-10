using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : MonoBehaviour
{
    public TextMeshProUGUI nameInput;
    public GameObject makeAccountPopup;

    Coroutine loginCo;

    private void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResLogin, OnRecvLogin);

        ErrorManager.Instance.HideLoadingIndicator();
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_ResLogin);
    }

    void SendLogin()
    {
        Debug.Log("SendLogin() ");

        ToS_ReqLogin req = new ToS_ReqLogin();
        req.nickName = nameInput.text;
        NetworkManager.Instance.Send(req.Write());
    }

    void OnRecvLogin(IPacket packet)
    {
        Debug.Log("RecvLogin() ");

        ToC_ResLogin res = packet as ToC_ResLogin;
        if (res.errorCode == 0)
        {
            SceneManager.Instance.MoveScene(1);
        }
        if (res.errorCode == 1)
        {
            ErrorManager.Instance.HideLoadingIndicator();
            ErrorManager.Instance.ShowPopup("안내", "이미 로그인 중인 계정입니다.");
        }
        else if (res.errorCode == 2)
        {
            ErrorManager.Instance.HideLoadingIndicator();
            ErrorManager.Instance.ShowPopup("안내", "닉네임이 존재하지 않습니다.");
        }
    }

    IEnumerator WaitForConnect()
    {
        float waitTime = 0f;

        if (NetworkManager.Instance._connected == false)
            NetworkManager.Instance.ConnectToServer();

        ErrorManager.Instance.ShowLoadingIndicator();

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (NetworkManager.Instance._connected)
            {
                SendLogin();
                yield break;
            }
            
            waitTime += 0.5f;
            if (waitTime > 10f)
            {
                ErrorManager.Instance.HideLoadingIndicator();
                ErrorManager.Instance.ShowPopup("안내", "서버와의 연결에 실패했습니다.");
                yield break;
            }
        }
    }

    // Events
    public void OnClickLogin()
    {
        if (nameInput.text.Length <= 2)
        {
            ErrorManager.Instance.ShowPopup("안내", "닉네임은 2글자 이상 입력해주세요.", null);
            return;
        }

        if (loginCo != null) StopCoroutine(loginCo);
        loginCo = StartCoroutine(WaitForConnect());
    }
}
