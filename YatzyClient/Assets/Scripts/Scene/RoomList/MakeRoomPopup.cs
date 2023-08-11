using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MakeRoomPopup : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    public GameObject dim;

    public void Show()
    {
        gameObject.SetActive(true);
        dim.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        dim.SetActive(false);
    }

    public void OnClickMakeRoom()
    {
        Debug.Log("ToS_ReqMakeRoom()");
        ToS_ReqMakeRoom req = new ToS_ReqMakeRoom();
        req.roomName = roomNameInput.text;
        NetworkManager.Instance.Send(req.Write());

        Hide();
    }
}

