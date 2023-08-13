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
		_makeFunc.Add((ushort)PacketID.ToC_ResLogin, MakePacket<ToC_ResLogin>);
		_handler.Add((ushort)PacketID.ToC_ResLogin, PacketHandler.ToC_ResLoginHandler);
		_makeFunc.Add((ushort)PacketID.ToC_ResRoomList, MakePacket<ToC_ResRoomList>);
		_handler.Add((ushort)PacketID.ToC_ResRoomList, PacketHandler.ToC_ResRoomListHandler);
		_makeFunc.Add((ushort)PacketID.ToC_ResMakeRoom, MakePacket<ToC_ResMakeRoom>);
		_handler.Add((ushort)PacketID.ToC_ResMakeRoom, PacketHandler.ToC_ResMakeRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToC_ResEnterRoom, MakePacket<ToC_ResEnterRoom>);
		_handler.Add((ushort)PacketID.ToC_ResEnterRoom, PacketHandler.ToC_ResEnterRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToC_ResLeaveRoom, MakePacket<ToC_ResLeaveRoom>);
		_handler.Add((ushort)PacketID.ToC_ResLeaveRoom, PacketHandler.ToC_ResLeaveRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToC_ResRoomInfo, MakePacket<ToC_ResRoomInfo>);
		_handler.Add((ushort)PacketID.ToC_ResRoomInfo, PacketHandler.ToC_ResRoomInfoHandler);
		_makeFunc.Add((ushort)PacketID.ToC_PlayerEnterRoom, MakePacket<ToC_PlayerEnterRoom>);
		_handler.Add((ushort)PacketID.ToC_PlayerEnterRoom, PacketHandler.ToC_PlayerEnterRoomHandler);
		_makeFunc.Add((ushort)PacketID.ToC_PlayerTurn, MakePacket<ToC_PlayerTurn>);
		_handler.Add((ushort)PacketID.ToC_PlayerTurn, PacketHandler.ToC_PlayerTurnHandler);
		_makeFunc.Add((ushort)PacketID.ToC_DiceResult, MakePacket<ToC_DiceResult>);
		_handler.Add((ushort)PacketID.ToC_DiceResult, PacketHandler.ToC_DiceResultHandler);
		_makeFunc.Add((ushort)PacketID.ToC_WriteScore, MakePacket<ToC_WriteScore>);
		_handler.Add((ushort)PacketID.ToC_WriteScore, PacketHandler.ToC_WriteScoreHandler);

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