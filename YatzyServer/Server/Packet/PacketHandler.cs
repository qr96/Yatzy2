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

        if (clientSession.Room == null)
            return;

        ToC_ResRoomList roomList = new ToC_ResRoomList();
        roomList.roomInfos.Add(new ToC_ResRoomList.RoomInfo() { roomId = 0, roomName = "한판조져~" });
        roomList.roomInfos.Add(new ToC_ResRoomList.RoomInfo() { roomId = 1, roomName = "으자아~" });
        roomList.roomInfos.Add(new ToC_ResRoomList.RoomInfo() { roomId = 2, roomName = "가즈앗~" });

        GameRoom room = clientSession.Room;
        room.Push(() => room.UniCast(clientSession, roomList));
    }
}
