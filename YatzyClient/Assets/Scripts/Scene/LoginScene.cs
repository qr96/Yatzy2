using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : MonoBehaviour
{
    public TextMeshProUGUI nameInput;

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

    public void OnClickLogin()
    {
        if (nameInput.text.Length <= 2)
        {
            ErrorManager.Instance.ShowPopup("�ȳ�", "�г����� 2���� �̻� �Է����ּ���.", null);
            return;
        }

        if (loginCo != null) StopCoroutine(loginCo);
        loginCo = StartCoroutine(WaitForConnect());
    }

    void OnRecvLogin(IPacket packet)
    {
        Debug.Log("RecvLogin() ");

        ToC_ResLogin res = packet as ToC_ResLogin;
        if (res.loginSuccess)
            SceneManager.Instance.MoveScene(1);
    }

    void SendLogin()
    {
        Debug.Log("SendLogin() ");

        ToS_ReqLogin req = new ToS_ReqLogin();
        req.nickName = nameInput.text;
        NetworkManager.Instance.Send(req.Write());
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
                ErrorManager.Instance.ShowPopup("�ȳ�", "�������� ���ῡ �����߽��ϴ�.");
                yield break;
            }
        }
    }
}
