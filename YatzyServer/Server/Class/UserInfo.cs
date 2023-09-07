using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class UserInfo
    {
        public string nickName;
        public long money;
        public long ruby;

        public DevilCastleInfo devilCastleInfo = new DevilCastleInfo();
    }

    public class DevilCastleInfo
    {
        public bool opened;
        public int level;
        public int maxLevel;
    }
}
