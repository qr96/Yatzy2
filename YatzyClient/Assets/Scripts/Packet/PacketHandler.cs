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

    public static void RemoveAction(PacketID protocol)
    {
        if (actionDic.ContainsKey(protocol) && actionDic[protocol] != null)
            actionDic.Remove(protocol);
    }

    public static void ToC_ResLoginHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResRoomListHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_ResMyInfoHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResMyInfoHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_ResRoomListHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResRoomListHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_ResMakeRoomHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResMakeRoomHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_ResEnterRoomHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResEnterRoomHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_ResLeaveRoomHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResLeaveRoomHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_ResEnterSingleRoomHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResEnterSingleRoomHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_ResLeaveSingleRoomHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResEnterSingleRoomHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }


    // ROOM

    public static void ToC_ResRoomInfoHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResRoomInfoHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_PlayerEnterRoomHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_PlayerEnterRoomHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_PlayerTurnHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResRoomInfoHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_DiceResultHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_DiceResultHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_WriteScoreHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ScoreUpdateHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_EndGameHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_EndGameHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_LockDiceHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_LockDiceHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_SelectScoreHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_SelectScoreHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }



    // Single
    public static void ToC_ResSingleRoomInfoHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_ResSingleRoomInfoHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_SingleStartGameHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_SingleStartGameHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_SingleDiceResultHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_SingleDiceResultHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }

    public static void ToC_SingleMobPlayResultHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Recv ToC_SingleMobPlayResultHandler");

        if (actionDic[(PacketID)packet.Protocol] != null)
            actionDic[(PacketID)packet.Protocol].Invoke(packet);
    }
}
