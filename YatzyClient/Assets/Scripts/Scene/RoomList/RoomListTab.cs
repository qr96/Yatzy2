using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomListTab : MonoBehaviour
{
    public RoomItem roomItemPrefab;
    public GameObject content;
    public MakeRoomPopup makeRoomPopup;
    public GameObject noRoomNoti;

    private void Start()
    {
        ErrorManager.Instance.HideLoadingIndicator();

        PacketHandler.AddAction(PacketID.ToC_ResRoomList, OnRecvRoomList);
        PacketHandler.AddAction(PacketID.ToC_ResEnterRoom, OnRecvEnterRoom);

        SendReqRoomList();
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_ResRoomList);
        PacketHandler.RemoveAction(PacketID.ToC_ResEnterRoom);
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
        ErrorManager.Instance.HideLoadingIndicator();

        Debug.Log("OnRecvRoomList() ");
        ToC_ResRoomList roomList = packet as ToC_ResRoomList;

        if (roomList.roomInfos.Count > 0) noRoomNoti.SetActive(false);
        else noRoomNoti.SetActive(true);

        foreach (var info in roomList.roomInfos)
        {
            RoomItem prefab = Instantiate(roomItemPrefab, content.transform);
            int roomId = info.roomId;
            prefab.SetInfo(info.roomName);
            prefab.SetClickEvent(() =>
            {
                ToS_ReqEnterRoom req = new ToS_ReqEnterRoom();
                req.roomId = roomId;
                NetworkManager.Instance.Send(req.Write());
            });
            prefab.gameObject.SetActive(true);
        }
    }

    void OnRecvEnterRoom(IPacket packet)
    {
        Debug.Log("OnRecvEnterRoom() ");
        ToC_ResEnterRoom req = packet as ToC_ResEnterRoom;

        SceneManager.LoadScene(2);
    }

    public void OnClickMakeRoom()
    {
        makeRoomPopup.Show();
    }

    public void OnClikRefreshRoomList()
    {
        ErrorManager.Instance.ShowLoadingIndicator();
        SendReqRoomList();
    }
}
