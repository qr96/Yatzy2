using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GameRoomManager
    {
        #region Singleton
        static GameRoomManager _instance = new GameRoomManager();
        public static GameRoomManager Instance { get { return _instance; } }
        #endregion

        int _roomId = 0;
        Dictionary<int, YatzyGameRoom> _rooms = new Dictionary<int, YatzyGameRoom>();
        object _lock = new object();

        public void Flush()
        {
            lock (_lock)
            {
                foreach (var room in _rooms.Values)
                {
                    room.Flush();
                }
            }
        }

        public YatzyGameRoom MakeRoom(ClientSession session, string name)
        {
            lock (_lock)
            {
                int roomId = _roomId;
                if (_rooms.ContainsKey(roomId + 1)) return null;
                _roomId++;

                YatzyGameRoom room = new YatzyGameRoom(roomId, name);
                room.roomID = roomId;
                _rooms.Add(roomId, room);
                room.Enter(session);

                Console.WriteLine($"MakeRoom : {roomId}");

                return room;
            }
        }

        public YatzyGameRoom Find(int id)
        {
            lock (_lock)
            {
                YatzyGameRoom room = null;
                _rooms.TryGetValue(id, out room);
                return room;
            }
        }

        public void RemoveRoom(int roomId)
        {
            lock (_lock)
            {
                _rooms.Remove(roomId);
            }
        }

        public void EnterRoom(ClientSession session, int roomId)
        {
            lock(_lock)
            {
                YatzyGameRoom room = null;
                _rooms.TryGetValue(roomId, out room);
                if (room != null)
                {
                    room.Enter(session);
                }
            }
        }

        public void LeaveRoom(ClientSession session)
        {
            lock (_lock)
            {
                if (session.GameRoom != null)
                {
                    YatzyGameRoom room = null;
                    _rooms.TryGetValue(session.GameRoom.roomID, out room);
                    if (room != null)
                        room.Leave(session);
                }
            }
        }

        public List<YatzyGameRoom> GetRoomList()
        {
            lock (_lock)
            {
                return _rooms.Values.ToList();
            }
        }
    }
}
