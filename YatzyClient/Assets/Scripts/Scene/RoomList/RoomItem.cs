using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI userCount;

    Action onClick;

    public void SetInfo(string name, int userCount)
    {
        roomName.text = name;
        this.userCount.text = $"{userCount}/2";
    }

    public void SetClickEvent(Action onClick)
    {
        this.onClick = onClick;
    }

    public void OnClick()
    {
        if (onClick != null) onClick();
    }
}
