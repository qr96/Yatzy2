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
        ToS_ReqLogin req = new ToS_ReqLogin();
        req.nickName = nameInput.text;

        NetworkManager.Instance.Send(req.Write());
    }

    void OnRecvLogin(IPacket packet)
    {
        Debug.Log("RecvLogin() ");
        ToC_ResLogin res = packet as ToC_ResLogin;
        if (res.loginSuccess)
            SceneManager.LoadScene(1);
    }
}
