using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{
	ToS_ReqRoomList = 1,
	ToC_ResRoomList = 2,
	ToS_ReqMakeRoom = 3,
	ToC_ResMakeRoom = 4,
	ToS_ReqEnterRoom = 5,
	ToC_ResEnterRoom = 6,
	ToS_ReqLeaveRoom = 7,
	ToC_ResLeaveRoom = 8,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
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
	public int roomId;
	public string roomName;

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResEnterRoom; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.success = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResEnterRoom);
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.success), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
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
    

    public ushort Protocol { get { return (ushort)PacketID.ToC_ResLeaveRoom; } }

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.ToC_ResLeaveRoom);
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false) 
            return null;
        return SendBufferHelper.Close(count);
    }
}

