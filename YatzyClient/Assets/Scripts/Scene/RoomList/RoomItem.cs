using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI roomName;

    Action enterRoom;

    public void SetInfo(string name)
    {
        this.roomName.text = name;
    }

    public void SetClickEvent(Action onClick)
    {
        this.enterRoom = onClick;
    }
}
