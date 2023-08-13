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
		_makeFunc.Add((ushort)PacketID.ToS_ReqRoomList, MakePacket<ToS_ReqRoomList>);
		_handler.Add((ushort)PacketID.ToS_ReqRoomList, PacketHandler.ToS_ReqRoomListHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqMakeRoom, MakePacket<ToS_ReqMakeRoom>);
		_handler.Add((ushort)PacketID.ToS_ReqMakeRoom, PacketHandler.ToS_ReqMakeRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqEnterRoom, MakePacket<ToS_ReqEnterRoom>);
		_handler.Add((ushort)PacketID.ToS_ReqEnterRoom, PacketHandler.ToS_ReqEnterRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqLeaveRoom, MakePacket<ToS_ReqLeaveRoom>);
		_handler.Add((ushort)PacketID.ToS_ReqLeaveRoom, PacketHandler.ToS_ReqLeaveRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReqRoomInfo, MakePacket<ToS_ReqRoomInfo>);
		_handler.Add((ushort)PacketID.ToS_ReqRoomInfo, PacketHandler.ToS_ReqRoomInfoHandler);
		_makeFunc.Add((ushort)PacketID.ToS_ReadyToStart, MakePacket<ToS_ReadyToStart>);
		_handler.Add((ushort)PacketID.ToS_ReadyToStart, PacketHandler.ToS_ReadyToStartHandler);
		_makeFunc.Add((ushort)PacketID.ToS_RollDice, MakePacket<ToS_RollDice>);
		_handler.Add((ushort)PacketID.ToS_RollDice, PacketHandler.ToS_RollDiceHandler);
		_makeFunc.Add((ushort)PacketID.ToS_WriteScore, MakePacket<ToS_WriteScore>);
		_handler.Add((ushort)PacketID.ToS_WriteScore, PacketHandler.ToS_WriteScoreHandler);

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