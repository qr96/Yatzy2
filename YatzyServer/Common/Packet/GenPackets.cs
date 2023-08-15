using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{
	ToS_ReqLogin = 1,
	ToC_ResLogin = 2,
	ToS_ReqRoomList = 3,
	ToC_ResRoomList = 4,
	ToS_ReqMakeRoom = 5,
	ToC_ResMakeRoom = 6,
	ToS_ReqEnterRoom = 7,
	ToC_ResEnterRoom = 8,
	ToS_ReqLeaveRoom = 9,
	ToC_ResLeaveRoom = 10,
	ToS_ReqRoomInfo = 11,
	ToC_ResRoomInfo = 12,
	ToC_PlayerEnterRoom = 13,
	ToS_ReadyToStart = 14,
	ToC_PlayerTurn = 15,
	ToS_RollDice = 16,
	ToC_DiceResult = 17,
	ToS_WriteScore = 18,
	ToC_WriteScore = 19,
	
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
		public bool privateRoom;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort roomNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.roomName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roomNameLen);
			count += roomNameLen;
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

