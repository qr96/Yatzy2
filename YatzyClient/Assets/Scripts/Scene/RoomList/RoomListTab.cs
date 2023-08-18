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

    List<RoomItem> roomItemPool = new List<RoomItem>();

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

        for (int i = 0; i < roomList.roomInfos.Count; i++)
        {
            RoomItem roomItemTmp;

            if (i < roomItemPool.Count)
            {
                roomItemTmp = roomItemPool[i];
            }
            else
            {
                roomItemTmp = Instantiate(roomItemPrefab, content.transform);
                roomItemPool.Add(roomItemTmp);
            }
                

            int roomId = roomList.roomInfos[i].roomId;
            roomItemTmp.SetInfo(roomList.roomInfos[i].roomName, roomList.roomInfos[i].playerCount);
            roomItemTmp.SetClickEvent(() =>
            {
                ToS_ReqEnterRoom req = new ToS_ReqEnterRoom();
                req.roomId = roomId;
                NetworkManager.Instance.Send(req.Write());
                ErrorManager.Instance.ShowLoadingIndicator();
            });
            roomItemTmp.gameObject.SetActive(true);
        }

        for (int i = roomList.roomInfos.Count; i < roomItemPool.Count; i++)
        {
            roomItemPool[i].gameObject.SetActive(false);
        }
    }

    void OnRecvEnterRoom(IPacket packet)
    {
        Debug.Log("OnRecvEnterRoom() ");
        ToC_ResEnterRoom req = packet as ToC_ResEnterRoom;
        if (req == null) return;

        if (req.success)
            SceneManager.LoadScene(2);
        else
        {
            ErrorManager.Instance.HideLoadingIndicator();
            ErrorManager.Instance.ShowPopup("안내", "방입장에 실패했습니다");
        }
            
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
