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
        if (roomNameInput.text.Length <= 2)
        {
            ErrorManager.Instance.ShowPopup("안내", "방이름은 2글자 이상 입력해주세요", null);
            return;
        }

        Debug.Log("ToS_ReqMakeRoom()");
        ToS_ReqMakeRoom req = new ToS_ReqMakeRoom();
        req.roomName = roomNameInput.text;
        NetworkManager.Instance.Send(req.Write());

        Hide();
        ErrorManager.Instance.ShowLoadingIndicator();
    }
}

