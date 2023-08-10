using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListTab : MonoBehaviour
{
    public RoomItem roomItemPrefab;
    public GameObject content;

    private void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResRoomList, OnRecvRoomList);
        SendReqRoomList();
    }

    void SendReqRoomList()
    {
        Debug.Log("SendReqRoomList()");
        ToS_ReqRoomList req = new ToS_ReqRoomList();
        req.authToken = "1234";
        NetworkManager.Instance.Send(req.Write());
    }

    void OnRecvRoomList(IPacket packet)
    {
        Debug.Log("OnRecvRoomList() ");
        ToC_ResRoomList roomInfoPacket = packet as ToC_ResRoomList;

        foreach (var info in roomInfoPacket.roomInfos)
        {
            RoomItem prefab = Instantiate(roomItemPrefab, content.transform);
            prefab.SetInfo(info.roomName);
            prefab.gameObject.SetActive(true);
        }
    }

    public void OnClickMakeRoom()
    {
        
    }
}
