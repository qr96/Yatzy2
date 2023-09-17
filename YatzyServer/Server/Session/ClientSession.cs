using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    public class ClientSession : PacketSession
    {
        public int SessionId {  get; set; }
        public GameRoom Lobby { get; set; }
        public YatzyGameRoom GameRoom { get; set; }
        public YatzySingleGame YatzySingleGame { get; set; }

        public string userId = "None";

        public void SetInfo(string userId)
        {
            this.userId = userId;
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Program.Lobby.Push(() => Program.Lobby.Enter(this));
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Lobby != null)
            {
                GameRoom room = Program.Lobby;
                Lobby.Push(() => room.Leave(this));
                Lobby = null;
            }
            
            if(GameRoom != null)
            {
                YatzyGameRoom room = GameRoomManager.Instance.Find(GameRoom.roomID);
                GameRoom.Push(() => room.Leave(this));
                GameRoom = null;
            }

            if(YatzySingleGame != null)
            {
                if (YatzySingleGame.gameEnd == false)
                    DataManager.Instance.DevilCastleLevelReset(userId);
                YatzySingleGame = null;
            }

            DataManager.Instance.Logout(userId);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred Bytes : {numOfBytes}");
        }
    }

}
