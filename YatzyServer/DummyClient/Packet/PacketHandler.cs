using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void ToC_ResRoomListHandler(PacketSession session, IPacket packet)
    {
        ToC_ResRoomList resRoomList = packet as ToC_ResRoomList;
        ServerSession serverSession = session as ServerSession;

        foreach (var roomInfo in resRoomList.roomInfos)
        {
            Console.WriteLine($"roomID : {roomInfo.roomId} {roomInfo.roomName}");
        }
    }
}
 