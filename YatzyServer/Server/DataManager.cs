
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
        static DataManager _instance = new DataManager();
        public static DataManager Instance { get { return _instance; } }
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

            return info;
        }

        public DevilCastleInfo GetUserDevilCastleInfo(string nickName)
        {
            UserInfo info;
            _userInfoDic.TryGetValue(nickName, out info);

            return info.devilCastleInfo;
        }

        public bool OpenDevilCastle(string nickName)
        {
            lock(_userInfoDic)
            {
                UserInfo info;
                _userInfoDic.TryGetValue(nickName, out info);

                if (info == null) return false;

                if (info.money < 1000) return false;

                info.money -= 1000;
                info.devilCastleInfo.opened = true;

                return true;
            }
        }

        public bool GetDevilCastleReward(string nickName)
        {
            lock (_userInfoDic)
            {
                UserInfo info;
                _userInfoDic.TryGetValue(nickName, out info);

                if (info == null) return false;

                if (info.devilCastleInfo.level <= 0) return false;

                info.money += 1000 * (long)Math.Pow(2, info.devilCastleInfo.level);
                info.devilCastleInfo.level = 0;
                info.devilCastleInfo.opened = false;
                return true;
            }
        }

        public void DevilCastleLevelUp(string nickName)
        {
            lock (_userInfoDic)
            {
                UserInfo info;
                _userInfoDic.TryGetValue(nickName, out info);

                if (info == null) return;

                info.devilCastleInfo.level++;
                if (info.devilCastleInfo.maxLevel < info.devilCastleInfo.level)
                    info.devilCastleInfo.maxLevel = info.devilCastleInfo.level;
            }
        }

        public void DevilCastleLevelReset(string nickName)
        {
            lock (_userInfoDic)
            {
                UserInfo info;
                _userInfoDic.TryGetValue(nickName, out info);

                if (info == null) return;

                info.devilCastleInfo.level = 0;
                info.devilCastleInfo.opened = false;
            }
        }

        public List<Tuple<string, int>> GetDevilCastleRanking()
        {
            List<Tuple<string, int>> ranking = new List<Tuple<string, int>>();

            lock (_userInfoDic)
            {
                foreach (var userInfo in _userInfoDic.Values)
                {
                    ranking.Add(new Tuple<string, int>(userInfo.nickName, userInfo.devilCastleInfo.maxLevel));
                }
            }

            ranking.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            return ranking;
        }
    }
}
