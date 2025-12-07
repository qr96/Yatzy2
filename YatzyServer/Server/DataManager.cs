
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToC_ResRoomInfo;

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

        object _userLoginedlock = new object();

        string sqlConnectionInfo = "Server=133.186.247.63;Port=3306;Database=yacht;Uid=client;Pwd=Nhn!@#123";

        public bool SqlQuery(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(sqlConnectionInfo))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    if (command.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                    else
                    {
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

        public bool SqlQueryTransaction(List<string> sqlList)
        {
            using (MySqlConnection connection = new MySqlConnection(sqlConnectionInfo))
            {
                connection.Open();

                MySqlTransaction transactoin = connection.BeginTransaction();
                MySqlCommand command = new MySqlCommand();

                command.Connection = connection;
                command.Transaction = transactoin;

                try
                {
                    foreach (var sql in sqlList)
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }

                    transactoin.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to transaction in db : " + ex.ToString());
                    transactoin.Rollback();
                    return false;
                }
            }
        }

        // 쿼리 성공 여부 반환
        public bool GetSqlData(string sql, Action<MySqlDataReader> readFunc)
        {
            using (MySqlConnection connection = new MySqlConnection(sqlConnectionInfo))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        readFunc(reader);
                    }
                    
                    reader.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to get data in db : " + ex.ToString());
                    return false;
                }
            }
        }

        // 0 : 성공, 1 : 닉네임 길이 문제, 2:db실패, 3: 이미 존재하는 유저
        public int AddNewUser(UserInfo userInfo)
        {
            if (userInfo.nickName.Length < 2 || userInfo.nickName.Length > 20) return 1;

            // user_id 이미 존재 여부 확인
            bool existUser = false;
            bool sqlResult = GetSqlData($"select exists (select user_id from user_info where user_id='{userInfo.nickName}') as is_exists;", (reader) =>
            {
                existUser = reader.GetBoolean(0);
            });

            if (!sqlResult) return 2;

            // user_id가 이미 존재
            if (existUser) return 3;

            // user_id를 닉네임으로 설정 (나중에 변경 필요)
            string sql = $"insert into user_info values('{userInfo.nickName}', '{userInfo.nickName}', {0}, {10000}, {0});";

            if (SqlQuery(sql))
            {
                Console.WriteLine($"Added user success. user_id={userInfo.nickName}");
                return 0;
            }
            
            return 2;
        }

        // 0: 성공, 1: 이미 로그인
        public int LoginUser(string userId)
        {
            lock (_userLoginedlock)
            {
                if (_userLogined.ContainsKey(userId))
                {
                    if (_userLogined[userId] == false)
                    {
                        _userLogined[userId] = true;
                        return 0;
                    }
                    return 1;
                }
                else
                {
                    _userLogined.Add(userId, true);
                    return 0;
                }
            }
        }

        public void Logout(string nickName)
        {
            lock (_userLoginedlock)
            {
                if (_userLogined.ContainsKey(nickName))
                {
                    _userLogined[nickName] = false;
                }
            }
        }

        public UserInfo GetUserInfo(string userId)
        {
            UserInfo info = new UserInfo();
            Console.WriteLine("Get User Info : " + userId);
            bool sqlResult = GetSqlData($"select nickname, nickname_seq, money, ruby from user_info where user_id='{userId}';",
                (reader) =>
                {
                    info.nickName = reader.GetString(0);
                    info.money = reader.GetInt64(2);
                    info.ruby = reader.GetInt64(3);
                });
            if (!sqlResult) return null;

            return info;
        }

        public DevilCastleInfo GetUserDevilCastleInfo(string userId)
        {
            DevilCastleInfo devilCastleInfo = new DevilCastleInfo();

            // 악마성 정보 존재 여부 확인
            int existDevilCastleData = 0;
            bool sqlResult = GetSqlData($"select exists (select user_id from devil_castle_info where user_id='{userId}');",
                (reader) =>
                {
                    existDevilCastleData = reader.GetInt32(0);
                });
            if (sqlResult == false)
            {
                Console.WriteLine("Failed to check exist devil castle data. user_id : " + userId);
                return null;
            }

            // 악마성 정보 존재하지 않으면 추가
            if (existDevilCastleData <= 0)
            {
                if (SqlQuery($"insert into devil_castle_info values('{userId}', false, 0, 0);") == false)
                {
                    Console.WriteLine("Failed to add devil_castle_info. user_id:" + userId);
                    return null;
                }
            }
            
            // 악마성 정보 가져옴
            bool sqlResult2 = GetSqlData($"select opened, now_level, max_level from devil_castle_info where user_id='{userId}';",
                (reader) =>
                {
                    devilCastleInfo.opened = reader.GetBoolean(0);
                    devilCastleInfo.level = reader.GetInt32(1);
                    devilCastleInfo.maxLevel = reader.GetInt32(2);
                });

            if (sqlResult2 == false)
            {
                Console.WriteLine("Failed to read devil castle data. user_id : " + userId);
                return null;
            }
            
            return devilCastleInfo;
        }

        public bool OpenDevilCastle(string userId)
        {
            var userInfo = GetUserInfo(userId);
            if(userInfo == null) return false;

            if (userInfo.money < 1000) return false;

            List<string> sqlList = new List<string>
            {
                $"update user_info set money=money-{1000} where user_id='{userId}';",
                $"update devil_castle_info set opened = {true} where user_id='{userId}';"
            };

            return SqlQueryTransaction(sqlList);
        }

        public bool GetDevilCastleReward(string userId)
        {
            var devilCastleInfo = GetUserDevilCastleInfo(userId);
            if (devilCastleInfo == null) return false;

            if (devilCastleInfo.level <= 0) return false;

            List<string> transaction = new List<string>
            {
                $"update user_info set money=money+{1000}*pow(2, (select now_level from devil_castle_info where user_id='{userId}')) where user_id='{userId}';",
                $"update devil_castle_info set opened=false, now_level=0 where user_id='{userId}';"
            };
            return SqlQueryTransaction(transaction);
        }

        public void DevilCastleLevelUp(string userId)
        {
            SqlQuery($"update devil_castle_info set now_level=now_level+1, max_level=(case when now_level>max_level then now_level else max_level end) where user_id='{userId}';");
        }

        public void DevilCastleLevelReset(string userId)
        {
            SqlQuery($"update devil_castle_info set opened=false, now_level=0 where user_id='{userId}';");
        }

        public List<Tuple<string, int>> GetDevilCastleRanking()
        {
            List<Tuple<string, int>> ranking = new List<Tuple<string, int>>(); // <닉네임, 최대 랭크>

            bool sqlResult = GetSqlData($"select user_info.nickname, max_level" +
                $"\r\nfrom devil_castle_info, user_info" +
                $"\r\nwhere devil_castle_info.user_id = user_info.user_id" +
                $"\r\norder by max_level desc" +
                $"\r\nlimit 100;",
                (reader) =>
                {
                    ranking.Add(new Tuple<string, int>(reader.GetString(0), reader.GetInt32(1)));
                });

            return ranking;
        }
    }
}
