using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{
	ToS_ReqLogin = 1,
	ToC_ResLogin = 2,
	ToS_ReqMyInfo = 3,
	ToC_ResMyInfo = 4,
	ToS_ReqRoomList = 5,
	ToC_ResRoomList = 6,
	ToS_ReqMakeRoom = 7,
	ToC_ResMakeRoom = 8,
	ToS_ReqEnterRoom = 9,
	ToC_ResEnterRoom = 10,
	ToS_ReqLeaveRoom = 11,
	ToC_ResLeaveRoom = 12,
	ToS_ReqEnterSingleRoom = 13,
	ToC_ResEnterSingleRoom = 14,
	ToS_ReqLeaveSingleRoom = 15,
	ToC_ResLeaveSingleRoom = 16,
	ToS_ReqDevilCastleInfo = 17,
	ToC_ResDevilCastleInfo = 18,
	ToS_ReqOpenDevilCastle = 19,
	ToC_ResOpenDevilCastle = 20,
	ToS_ReqGetDevilCastleReward = 21,
	ToC_ResGetDevilCastleReward = 22,
	ToS_ReqDevilCastleRanking = 23,
	ToC_RecDevilCastleRanking = 24,
	ToS_ReqRoomInfo = 25,
	ToC_ResRoomInfo = 26,
	ToC_PlayerEnterRoom = 27,
	ToS_ReadyToStart = 28,
	ToC_PlayerTurn = 29,
	ToS_RollDice = 30,
	ToC_DiceResult = 31,
	ToS_WriteScore = 32,
	ToC_WriteScore = 33,
	ToC_EndGame = 34,
	ToS_LockDice = 35,
	ToC_LockDice = 36,
	ToS_SelectScore = 37,
	ToC_SelectScore = 38,
	ToS_ReqSingleRoomInfo = 39,
	ToC_ResSingleRoomInfo = 40,
	ToS_SingleReadyToStart = 41,
	ToC_SingleStartGame = 42,
	ToS_SingleRollDice = 43,
	ToC_SingleDiceResult = 44,
	ToS_SingleWriteScore = 45,
	ToC_SingleMobPlayResult = 46,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


class ToS_ReqLogin : IPacket
{
    public string nickName;

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqLogin; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort nickNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.nickName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nickNameLen);
		count += nickNameLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqLogin);
        count += sizeof(ushort);
        ushort nickNameLen = (ushort)Encoding.Unicode.GetBytes(this.nickName, 0, this.nickName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(nickNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += nickNameLen;
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResLogin : IPacket
{
    public bool loginSuccess;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResLogin; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.loginSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResLogin);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.loginSuccess), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqMyInfo : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqMyInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqMyInfo);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResMyInfo : IPacket
{
    public string nickName;
	public long money;
	public long ruby;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResMyInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort nickNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.nickName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nickNameLen);
		count += nickNameLen;
		this.money = BitConverter.ToInt64(segment.Array, segment.Offset + count);
		count += sizeof(long);
		this.ruby = BitConverter.ToInt64(segment.Array, segment.Offset + count);
		count += sizeof(long);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResMyInfo);
        count += sizeof(ushort);
        ushort nickNameLen = (ushort)Encoding.Unicode.GetBytes(this.nickName, 0, this.nickName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(nickNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += nickNameLen;
		Array.Copy(BitConverter.GetBytes(this.money), 0, segment.Array, segment.Offset + count, sizeof(long));
		count += sizeof(long);
		Array.Copy(BitConverter.GetBytes(this.ruby), 0, segment.Array, segment.Offset + count, sizeof(long));
		count += sizeof(long);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqRoomList : IPacket
{
    public string authToken;

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqRoomList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort authTokenLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.authToken = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, authTokenLen);
		count += authTokenLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqRoomList);
        count += sizeof(ushort);
        ushort authTokenLen = (ushort)Encoding.Unicode.GetBytes(this.authToken, 0, this.authToken.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(authTokenLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += authTokenLen;
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResRoomList : IPacket
{
    public class RoomInfo
	{
		public int roomId;
		public string roomName;
		public int playerCount;
		public bool privateRoom;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort roomNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.roomName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomNameLen);
			count += roomNameLen;
			this.playerCount = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.privateRoom = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			ushort roomNameLen = (ushort)Encoding.Unicode.GetBytes(this.roomName, 0, this.roomName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(roomNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += roomNameLen;
			Array.Copy(BitConverter.GetBytes(this.playerCount), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.privateRoom), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			return success;
		}	
	}
	public List<RoomInfo> roomInfos = new List<RoomInfo>();

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResRoomList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.roomInfos.Clear();
		ushort roomInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < roomInfoLen; i++)
		{
			RoomInfo roomInfo = new RoomInfo();
			roomInfo.Read(segment, ref count);
			roomInfos.Add(roomInfo);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResRoomList);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.roomInfos.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (RoomInfo roomInfo in this.roomInfos)
			roomInfo.Write(segment, ref count);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqMakeRoom : IPacket
{
    public string roomName;

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqMakeRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort roomNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomNameLen);
		count += roomNameLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqMakeRoom);
        count += sizeof(ushort);
        ushort roomNameLen = (ushort)Encoding.Unicode.GetBytes(this.roomName, 0, this.roomName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomNameLen;
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResMakeRoom : IPacket
{
    public string roomName;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResMakeRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort roomNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomNameLen);
		count += roomNameLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResMakeRoom);
        count += sizeof(ushort);
        ushort roomNameLen = (ushort)Encoding.Unicode.GetBytes(this.roomName, 0, this.roomName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomNameLen;
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqEnterRoom : IPacket
{
    public int roomId;

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqEnterRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqEnterRoom);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResEnterRoom : IPacket
{
    public bool success;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResEnterRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.success = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResEnterRoom);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.success), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqLeaveRoom : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqLeaveRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqLeaveRoom);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResLeaveRoom : IPacket
{
    public int leavePlayerIndex;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResLeaveRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.leavePlayerIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResLeaveRoom);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.leavePlayerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqEnterSingleRoom : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqEnterSingleRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqEnterSingleRoom);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResEnterSingleRoom : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResEnterSingleRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResEnterSingleRoom);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqLeaveSingleRoom : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqLeaveSingleRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqLeaveSingleRoom);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResLeaveSingleRoom : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResLeaveSingleRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResLeaveSingleRoom);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqDevilCastleInfo : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqDevilCastleInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqDevilCastleInfo);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResDevilCastleInfo : IPacket
{
    public bool isOpened;
	public int level;
	public int reward;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResDevilCastleInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.isOpened = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.level = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.reward = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResDevilCastleInfo);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.isOpened), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.level), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.reward), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqOpenDevilCastle : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqOpenDevilCastle; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqOpenDevilCastle);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResOpenDevilCastle : IPacket
{
    public bool success;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResOpenDevilCastle; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.success = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResOpenDevilCastle);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.success), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqGetDevilCastleReward : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqGetDevilCastleReward; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqGetDevilCastleReward);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResGetDevilCastleReward : IPacket
{
    public bool success;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResGetDevilCastleReward; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.success = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResGetDevilCastleReward);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.success), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqDevilCastleRanking : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqDevilCastleRanking; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqDevilCastleRanking);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_RecDevilCastleRanking : IPacket
{
    public class Ranking
	{
		public string userName;
		public int maxLevel;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort userNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.userName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, userNameLen);
			count += userNameLen;
			this.maxLevel = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort userNameLen = (ushort)Encoding.Unicode.GetBytes(this.userName, 0, this.userName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(userNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += userNameLen;
			Array.Copy(BitConverter.GetBytes(this.maxLevel), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<Ranking> rankings = new List<Ranking>();

    public ushort Protocol { get { return (ushort)PacketID.ToC_RecDevilCastleRanking; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.rankings.Clear();
		ushort rankingLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < rankingLen; i++)
		{
			Ranking ranking = new Ranking();
			ranking.Read(segment, ref count);
			rankings.Add(ranking);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_RecDevilCastleRanking);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.rankings.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Ranking ranking in this.rankings)
			ranking.Write(segment, ref count);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqRoomInfo : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqRoomInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqRoomInfo);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResRoomInfo : IPacket
{
    public string roomName;
	public int myServerIndex;
	public class UserInfo
	{
		public string userName;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort userNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.userName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, userNameLen);
			count += userNameLen;
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort userNameLen = (ushort)Encoding.Unicode.GetBytes(this.userName, 0, this.userName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(userNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += userNameLen;
			return success;
		}	
	}
	public List<UserInfo> userInfos = new List<UserInfo>();

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResRoomInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort roomNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roomName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomNameLen);
		count += roomNameLen;
		this.myServerIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.userInfos.Clear();
		ushort userInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < userInfoLen; i++)
		{
			UserInfo userInfo = new UserInfo();
			userInfo.Read(segment, ref count);
			userInfos.Add(userInfo);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResRoomInfo);
        count += sizeof(ushort);
        ushort roomNameLen = (ushort)Encoding.Unicode.GetBytes(this.roomName, 0, this.roomName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roomNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roomNameLen;
		Array.Copy(BitConverter.GetBytes(this.myServerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes((ushort)this.userInfos.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (UserInfo userInfo in this.userInfos)
			userInfo.Write(segment, ref count);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_PlayerEnterRoom : IPacket
{
    public string playerNickName;
	public int playerIndex;

    public ushort Protocol { get { return (ushort)PacketID.ToC_PlayerEnterRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort playerNickNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerNickName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerNickNameLen);
		count += playerNickNameLen;
		this.playerIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_PlayerEnterRoom);
        count += sizeof(ushort);
        ushort playerNickNameLen = (ushort)Encoding.Unicode.GetBytes(this.playerNickName, 0, this.playerNickName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerNickNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerNickNameLen;
		Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReadyToStart : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReadyToStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReadyToStart);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_PlayerTurn : IPacket
{
    public int playerTurn;

    public ushort Protocol { get { return (ushort)PacketID.ToC_PlayerTurn; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerTurn = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_PlayerTurn);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerTurn), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_RollDice : IPacket
{
    public class FixDice
	{
		public int diceIndex;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.diceIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.diceIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<FixDice> fixDices = new List<FixDice>();

    public ushort Protocol { get { return (ushort)PacketID.ToS_RollDice; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.fixDices.Clear();
		ushort fixDiceLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < fixDiceLen; i++)
		{
			FixDice fixDice = new FixDice();
			fixDice.Read(segment, ref count);
			fixDices.Add(fixDice);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_RollDice);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.fixDices.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (FixDice fixDice in this.fixDices)
			fixDice.Write(segment, ref count);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_DiceResult : IPacket
{
    public int playerIndex;
	public int leftDice;
	public class DiceResult
	{
		public int dice;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.dice = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.dice), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<DiceResult> diceResults = new List<DiceResult>();

    public ushort Protocol { get { return (ushort)PacketID.ToC_DiceResult; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.leftDice = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.diceResults.Clear();
		ushort diceResultLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < diceResultLen; i++)
		{
			DiceResult diceResult = new DiceResult();
			diceResult.Read(segment, ref count);
			diceResults.Add(diceResult);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_DiceResult);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.leftDice), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes((ushort)this.diceResults.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (DiceResult diceResult in this.diceResults)
			diceResult.Write(segment, ref count);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_WriteScore : IPacket
{
    public int jocboIndex;

    public ushort Protocol { get { return (ushort)PacketID.ToS_WriteScore; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.jocboIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_WriteScore);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.jocboIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_WriteScore : IPacket
{
    public int playerIndex;
	public int jocboIndex;
	public int jocboScore;

    public ushort Protocol { get { return (ushort)PacketID.ToC_WriteScore; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.jocboIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.jocboScore = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_WriteScore);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.jocboIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.jocboScore), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_EndGame : IPacket
{
    public int winner;
	public bool drawGame;

    public ushort Protocol { get { return (ushort)PacketID.ToC_EndGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.winner = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.drawGame = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_EndGame);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.winner), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.drawGame), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_LockDice : IPacket
{
    public int diceIndex;
	public bool isLocked;

    public ushort Protocol { get { return (ushort)PacketID.ToS_LockDice; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.diceIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.isLocked = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_LockDice);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.diceIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.isLocked), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_LockDice : IPacket
{
    public int diceIndex;
	public bool isLocked;

    public ushort Protocol { get { return (ushort)PacketID.ToC_LockDice; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.diceIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.isLocked = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_LockDice);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.diceIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.isLocked), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_SelectScore : IPacket
{
    public int jocboIndex;

    public ushort Protocol { get { return (ushort)PacketID.ToS_SelectScore; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.jocboIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_SelectScore);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.jocboIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_SelectScore : IPacket
{
    public int playerIndex;
	public int jocboIndex;

    public ushort Protocol { get { return (ushort)PacketID.ToC_SelectScore; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.jocboIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_SelectScore);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.jocboIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_ReqSingleRoomInfo : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_ReqSingleRoomInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_ReqSingleRoomInfo);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_ResSingleRoomInfo : IPacket
{
    public string userName;
	public string mobName;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResSingleRoomInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort userNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.userName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, userNameLen);
		count += userNameLen;
		ushort mobNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.mobName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, mobNameLen);
		count += mobNameLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResSingleRoomInfo);
        count += sizeof(ushort);
        ushort userNameLen = (ushort)Encoding.Unicode.GetBytes(this.userName, 0, this.userName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(userNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += userNameLen;
		ushort mobNameLen = (ushort)Encoding.Unicode.GetBytes(this.mobName, 0, this.mobName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(mobNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += mobNameLen;
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_SingleReadyToStart : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToS_SingleReadyToStart; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_SingleReadyToStart);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_SingleStartGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.ToC_SingleStartGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_SingleStartGame);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_SingleRollDice : IPacket
{
    public class FixDice
	{
		public int diceIndex;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.diceIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.diceIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<FixDice> fixDices = new List<FixDice>();

    public ushort Protocol { get { return (ushort)PacketID.ToS_SingleRollDice; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.fixDices.Clear();
		ushort fixDiceLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < fixDiceLen; i++)
		{
			FixDice fixDice = new FixDice();
			fixDice.Read(segment, ref count);
			fixDices.Add(fixDice);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_SingleRollDice);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.fixDices.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (FixDice fixDice in this.fixDices)
			fixDice.Write(segment, ref count);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_SingleDiceResult : IPacket
{
    public int leftDice;
	public class DiceResult
	{
		public int dice;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.dice = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.dice), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<DiceResult> diceResults = new List<DiceResult>();

    public ushort Protocol { get { return (ushort)PacketID.ToC_SingleDiceResult; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.leftDice = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.diceResults.Clear();
		ushort diceResultLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < diceResultLen; i++)
		{
			DiceResult diceResult = new DiceResult();
			diceResult.Read(segment, ref count);
			diceResults.Add(diceResult);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_SingleDiceResult);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.leftDice), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes((ushort)this.diceResults.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (DiceResult diceResult in this.diceResults)
			diceResult.Write(segment, ref count);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToS_SingleWriteScore : IPacket
{
    public int jocboIndex;

    public ushort Protocol { get { return (ushort)PacketID.ToS_SingleWriteScore; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.jocboIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToS_SingleWriteScore);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.jocboIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

class ToC_SingleMobPlayResult : IPacket
{
    public class DiceResultList
	{
		public class DiceResult
		{
			public int diceNum;
		
			public void Read(ArraySegment<byte> segment, ref ushort count)
			{
				this.diceNum = BitConverter.ToInt32(segment.Array, segment.Offset + count);
				count += sizeof(int);
			}
		
			public bool Write(ArraySegment<byte> segment, ref ushort count)
			{
				bool success = true;
				Array.Copy(BitConverter.GetBytes(this.diceNum), 0, segment.Array, segment.Offset + count, sizeof(int));
				count += sizeof(int);
				return success;
			}	
		}
		public List<DiceResult> diceResults = new List<DiceResult>();
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.diceResults.Clear();
			ushort diceResultLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < diceResultLen; i++)
			{
				DiceResult diceResult = new DiceResult();
				diceResult.Read(segment, ref count);
				diceResults.Add(diceResult);
			}
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes((ushort)this.diceResults.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach (DiceResult diceResult in this.diceResults)
				diceResult.Write(segment, ref count);
			return success;
		}	
	}
	public List<DiceResultList> diceResultLists = new List<DiceResultList>();
	public class DiceLockList
	{
		public class DiceLock
		{
			public int diceIndex;
		
			public void Read(ArraySegment<byte> segment, ref ushort count)
			{
				this.diceIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
				count += sizeof(int);
			}
		
			public bool Write(ArraySegment<byte> segment, ref ushort count)
			{
				bool success = true;
				Array.Copy(BitConverter.GetBytes(this.diceIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
				count += sizeof(int);
				return success;
			}	
		}
		public List<DiceLock> diceLocks = new List<DiceLock>();
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.diceLocks.Clear();
			ushort diceLockLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			for (int i = 0; i < diceLockLen; i++)
			{
				DiceLock diceLock = new DiceLock();
				diceLock.Read(segment, ref count);
				diceLocks.Add(diceLock);
			}
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes((ushort)this.diceLocks.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			foreach (DiceLock diceLock in this.diceLocks)
				diceLock.Write(segment, ref count);
			return success;
		}	
	}
	public List<DiceLockList> diceLockLists = new List<DiceLockList>();
	public int jocboIndex;
	public int jocboScore;

    public ushort Protocol { get { return (ushort)PacketID.ToC_SingleMobPlayResult; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.diceResultLists.Clear();
		ushort diceResultListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < diceResultListLen; i++)
		{
			DiceResultList diceResultList = new DiceResultList();
			diceResultList.Read(segment, ref count);
			diceResultLists.Add(diceResultList);
		}
		this.diceLockLists.Clear();
		ushort diceLockListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < diceLockListLen; i++)
		{
			DiceLockList diceLockList = new DiceLockList();
			diceLockList.Read(segment, ref count);
			diceLockLists.Add(diceLockList);
		}
		this.jocboIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.jocboScore = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_SingleMobPlayResult);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.diceResultLists.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (DiceResultList diceResultList in this.diceResultLists)
			diceResultList.Write(segment, ref count);
		Array.Copy(BitConverter.GetBytes((ushort)this.diceLockLists.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (DiceLockList diceLockList in this.diceLockLists)
			diceLockList.Write(segment, ref count);
		Array.Copy(BitConverter.GetBytes(this.jocboIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.jocboScore), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

