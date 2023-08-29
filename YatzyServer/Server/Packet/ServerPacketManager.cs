using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
		
	public void Register()
	{
		_makeFunc.Add((ushort)PacketID.ToS_ReqLogin, MakePacket<ToS_ReqLogin>);
		_handler.Add((ushort)PacketID.ToS_ReqLogin, PacketHandler.ToS_ReqLoginHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqMyInfo, MakePacket<ToS_ReqMyInfo>);
		_handler.Add((ushort)PacketID.ToS_ReqMyInfo, PacketHandler.ToS_ReqMyInfoHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqRoomList, MakePacket<ToS_ReqRoomList>);
		_handler.Add((ushort)PacketID.ToS_ReqRoomList, PacketHandler.ToS_ReqRoomListHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqMakeRoom, MakePacket<ToS_ReqMakeRoom>);
		_handler.Add((ushort)PacketID.ToS_ReqMakeRoom, PacketHandler.ToS_ReqMakeRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqEnterRoom, MakePacket<ToS_ReqEnterRoom>);
		_handler.Add((ushort)PacketID.ToS_ReqEnterRoom, PacketHandler.ToS_ReqEnterRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqLeaveRoom, MakePacket<ToS_ReqLeaveRoom>);
		_handler.Add((ushort)PacketID.ToS_ReqLeaveRoom, PacketHandler.ToS_ReqLeaveRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqEnterSingleRoom, MakePacket<ToS_ReqEnterSingleRoom>);
		_handler.Add((ushort)PacketID.ToS_ReqEnterSingleRoom, PacketHandler.ToS_ReqEnterSingleRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqLeaveSingleRoom, MakePacket<ToS_ReqLeaveSingleRoom>);
		_handler.Add((ushort)PacketID.ToS_ReqLeaveSingleRoom, PacketHandler.ToS_ReqLeaveSingleRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqDevilCastleInfo, MakePacket<ToS_ReqDevilCastleInfo>);
		_handler.Add((ushort)PacketID.ToS_ReqDevilCastleInfo, PacketHandler.ToS_ReqDevilCastleInfoHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqOpenDevilCastle, MakePacket<ToS_ReqOpenDevilCastle>);
		_handler.Add((ushort)PacketID.ToS_ReqOpenDevilCastle, PacketHandler.ToS_ReqOpenDevilCastleHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqGetDevilCastleReward, MakePacket<ToS_ReqGetDevilCastleReward>);
		_handler.Add((ushort)PacketID.ToS_ReqGetDevilCastleReward, PacketHandler.ToS_ReqGetDevilCastleRewardHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqRoomInfo, MakePacket<ToS_ReqRoomInfo>);
		_handler.Add((ushort)PacketID.ToS_ReqRoomInfo, PacketHandler.ToS_ReqRoomInfoHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReadyToStart, MakePacket<ToS_ReadyToStart>);
		_handler.Add((ushort)PacketID.ToS_ReadyToStart, PacketHandler.ToS_ReadyToStartHandler);
		_makeFunc.Add((ushort)PacketID.ToS_RollDice, MakePacket<ToS_RollDice>);
		_handler.Add((ushort)PacketID.ToS_RollDice, PacketHandler.ToS_RollDiceHandler);
		_makeFunc.Add((ushort)PacketID.ToS_WriteScore, MakePacket<ToS_WriteScore>);
		_handler.Add((ushort)PacketID.ToS_WriteScore, PacketHandler.ToS_WriteScoreHandler);
		_makeFunc.Add((ushort)PacketID.ToS_LockDice, MakePacket<ToS_LockDice>);
		_handler.Add((ushort)PacketID.ToS_LockDice, PacketHandler.ToS_LockDiceHandler);
		_makeFunc.Add((ushort)PacketID.ToS_SelectScore, MakePacket<ToS_SelectScore>);
		_handler.Add((ushort)PacketID.ToS_SelectScore, PacketHandler.ToS_SelectScoreHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqSingleRoomInfo, MakePacket<ToS_ReqSingleRoomInfo>);
		_handler.Add((ushort)PacketID.ToS_ReqSingleRoomInfo, PacketHandler.ToS_ReqSingleRoomInfoHandler);
		_makeFunc.Add((ushort)PacketID.ToS_SingleReadyToStart, MakePacket<ToS_SingleReadyToStart>);
		_handler.Add((ushort)PacketID.ToS_SingleReadyToStart, PacketHandler.ToS_SingleReadyToStartHandler);
		_makeFunc.Add((ushort)PacketID.ToS_SingleRollDice, MakePacket<ToS_SingleRollDice>);
		_handler.Add((ushort)PacketID.ToS_SingleRollDice, PacketHandler.ToS_SingleRollDiceHandler);
		_makeFunc.Add((ushort)PacketID.ToS_SingleWriteScore, MakePacket<ToS_SingleWriteScore>);
		_handler.Add((ushort)PacketID.ToS_SingleWriteScore, PacketHandler.ToS_SingleWriteScoreHandler);

	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
		if (_makeFunc.TryGetValue(id, out func))
		{
			IPacket packet = func.Invoke(session, buffer);
			if (onRecvCallback != null)
				onRecvCallback.Invoke(session, packet);
			else
				HandlePacket(session, packet);
		}
	}

	T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
		pkt.Read(buffer);
		return pkt;
	}

	public void HandlePacket(PacketSession session, IPacket packet)
	{
		Action<PacketSession, IPacket> action = null;
		if (_handler.TryGetValue(packet.Protocol, out action))
			action.Invoke(session, packet);
	}
}