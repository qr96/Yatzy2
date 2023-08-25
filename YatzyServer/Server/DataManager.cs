
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DataManager
    {
        #region Singleton
        static GameRoomManager _instance = new GameRoomManager();
        public static GameRoomManager Instance { get { return _instance; } }
        #endregion

        List<UserInfo> _userInfoList = new List<UserInfo>();

    }
}
