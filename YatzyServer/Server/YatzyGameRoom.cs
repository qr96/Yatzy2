using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class YatzyGameRoom : GameRoom
    {
        public int roomID;
        public string roomName;

        public YatzyGameRoom(int roomID, string name)
        {
            this.roomID = roomID;
            this.roomName = name;
        }
    }
}

