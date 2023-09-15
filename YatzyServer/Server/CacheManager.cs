using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class CacheManager
    {
        #region Singleton
        static CacheManager _instance = new CacheManager();
        public static CacheManager Instance { get { return _instance; } }
        #endregion


    }
}
