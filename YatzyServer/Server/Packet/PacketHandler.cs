using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void ToS_ReqRoomListHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Lobby == null)
            return;

        Console.WriteLine("ToS_ReqRoomListHandler");

        ToC_ResRoomList roomListPacket = new ToC_ResRoomList();
        List<YatzyGameRoom> roomList = GameRoomManager.Instance.GetRoomList();

        foreach (var roomInfo in roomList)
            roomListPacket.roomInfos.Add(new ToC_ResRoomList.RoomInfo() { roomId = roomInfo.roomID, roomName = roomInfo.roomName });

        GameRoom gameRoom = clientSession.Lobby;
        gameRoom.Push(() => gameRoom.UniCast(clientSession, roomListPacket));
    }

    public static void ToS_ReqMakeRoomHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_ReqMakeRoom room = packet as ToS_ReqMakeRoom;

        if (clientSession.Lobby == null)
            return;

        YatzyGameRoom makedRoom = GameRoomManager.Instance.MakeRoom(room.roomName);
        makedRoom.Enter(clientSession);
    }

    public static void ToS_ReqEnterRoomHandler(PacketSession session, IPacket ipacket)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_ReqEnterRoom packet = ipacket as ToS_ReqEnterRoom;

        if (clientSession.Lobby == null)
            return;

        YatzyGameRoom room = GameRoomManager.Instance.EnterRoom(clientSession, packet.roomId);
        if (room != null)
            clientSession.Lobby.UniCast(clientSession, new ToC_ResEnterRoom() { success = true, roomId = room.roomID, roomName = room.roomName });
        else
            clientSession.Lobby.UniCast(clientSession, new ToC_ResEnterRoom() { success = false, roomId = -1, roomName = "-" });
    }

    public static void ToS_ReqLeaveRoomHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Lobby == null)
            return;
    }
}
