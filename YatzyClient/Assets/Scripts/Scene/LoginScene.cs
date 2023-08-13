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

    public void OnClickLogin()
    {
        if (nameInput.text.Length <= 2)
        {
            ErrorManager.Instance.ShowPopup("�ȳ�", "�г����� 2���� �̻� �Է����ּ���.", null);
            return;
        }

        ToS_ReqLogin req = new ToS_ReqLogin();
        req.nickName = nameInput.text;

        NetworkManager.Instance.ConnectToServer(() => NetworkManager.Instance.Send(req.Write()));
        
        ErrorManager.Instance.ShowLoadingIndicator();
    }

    void OnRecvLogin(IPacket packet)
    {
        Debug.Log("RecvLogin() ");

        ToC_ResLogin res = packet as ToC_ResLogin;
        if (res.loginSuccess)
            SceneManager.LoadScene(1);
    }
}
