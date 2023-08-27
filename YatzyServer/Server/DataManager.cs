
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToC_ResRoomInfo;

namespace Server
{
    public class DataManager
    {
        #region Singleton
        static GameRoomManager _instance = new GameRoomManager();
        public static GameRoomManager Instance { get { return _instance; } }
        #endregion

        public Dictionary<string, UserInfo> _userInfoDic = new Dictionary<string, UserInfo>();

        public bool AddUser(UserInfo userInfo)
        {
            lock (_userInfoDic)
            {
                if (_userInfoDic.ContainsKey(userInfo.nickName)) return false;

                return _userInfoDic.TryAdd(userInfo.nickName, userInfo);
            }
        }

        public bool ExistNickName(string nickName)
        {
            return _userInfoDic.ContainsKey(nickName);
        }

        public UserInfo GetUserInfo(string nickName)
        {
            UserInfo info;
            _userInfoDic.TryGetValue(nickName, out info);

            if (info == null) return null;

            UserInfo deepCopy = new UserInfo() {
                nickName = info.nickName,
                money = info.money,
                ruby = info.ruby,
                devilCastleLevel = info.devilCastleLevel };
            return deepCopy;
        }

        public void DevilCastleLevelUp(string nickName)
        {
            lock (_userInfoDic)
            {
                UserInfo info;
                _userInfoDic.TryGetValue(nickName, out info);

                if (info == null) return;

                info.devilCastleLevel++;
            }
        }

        public void DevilCastleLevelReset(string nickName)
        {
            lock (_userInfoDic)
            {
                UserInfo info;
                _userInfoDic.TryGetValue(nickName, out info);

                if (info == null) return;

                info.devilCastleLevel = 0;
            }
        }
    }
}
