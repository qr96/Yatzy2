using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class PacketHandler
{
    public static Dictionary<PacketID, Action<IPacket>> actionDic = new Dictionary<PacketID, Action<IPacket>>();

    public static void AddAction(PacketID protocol, Action<IPacket> action)
    {
        actionDic.Add(protocol, action);
    }

    public static void RemoveAction(PacketID protocol, Action action)
    {
        if (actionDic[protocol] != null)
            actionDic.Remove(protocol);
    }

    public static void ToC_ResRoomListHandler(PacketSession session, IPacket packet)
    {
        //ToC_ResRoomList resRoomList = packet as ToC_ResRoomList;
        //ServerSession serverSession = session as ServerSession;
        Debug.Log("recv");
        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_ResMakeRoomHandler(PacketSession session, IPacket packet)
    {

    }

    public static void ToC_ResEnterRoomHandler(PacketSession session, IPacket packet)
    {

    }

    public static void ToC_ResLeaveRoomHandler(PacketSession session, IPacket packet)
    {

    }
}
