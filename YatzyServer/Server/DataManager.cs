
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;
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
        public Dictionary<string, bool> _userLogined = new Dictionary<string, bool>();

        public bool SqlQuery(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3306;Database=yacht;Uid=root;Pwd=0000"))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    if (command.ExecuteNonQuery() == 1)
                    {
                        Console.WriteLine("success to insert user in db");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("failed to insert user in db");
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to sql in db : " + ex.ToString());
                    return false;
                }
            }
        }

        public List<string> GetSqlData(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3306;Database=yacht;Uid=root;Pwd=0000"))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    List<string> result = new List<string>();

                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0)) result.Add("");
                        else result.Add(reader.GetString(0));
                    }

                    reader.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to get data in db : " + ex.ToString());
                }
            }
            return null;
        }

        // 0 : 성공, 1 : 닉네임 길이 문제, 2:db실패, 3: 이미 존재하는 닉네임
        public int AddNewUser(UserInfo userInfo)
        {
            if (userInfo.nickName.Length < 2 || userInfo.nickName.Length > 20) return 1;

            var reader = GetSqlData($"select max(nickname_seq) from user_info where nickname='{userInfo.nickName}'");
            if (reader == null) return 2;
            int maxNicknameSeq = -1;

            if (!Int32.TryParse(reader[0], out maxNicknameSeq)) maxNicknameSeq = 0;
            else maxNicknameSeq++;

            // 로그인 붙이기 전 임시처리 (일단은 닉네임 한개만 만들도록)
            if (maxNicknameSeq > 0) return 3;

            string sql = $"insert into user_info(nickname, nickname_seq, money, ruby) values('{userInfo.nickName}', {maxNicknameSeq}, {10000}, {0})";

            if (SqlQuery(sql))
            {
                Console.WriteLine("Added user success");
                return 0;
            }
            
            return 2;
        }

        public int LoginUser(string nickName)
        {
            // 0: 성공, 1: 이미 로그인
            lock (_userLogined)
            {
                if (_userLogined.ContainsKey(nickName))
                {
                    if (_userLogined[nickName] == false)
                    {
                        _userLogined[nickName] = true;
                        return 0;
                    }
                    return 1;
                }
                else
                {
                    _userLogined.Add(nickName, true);
                    return 0;
                }
            }
        }

        public void Logout(string nickName)
        {
            lock (_userLogined)
            {
                if (_userLogined.ContainsKey(nickName))
                {
                    _userLogined[nickName] = false;
                }
            }
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
