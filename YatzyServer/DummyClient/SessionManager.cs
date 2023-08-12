using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DummyClient
{
    class SessionManager
    {

        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();

        public void SendForEach()
        {
            lock (_lock)
            {
                int i = 0;
                foreach (ServerSession session in _sessions)
                {
                    ToS_ReqLogin req = new ToS_ReqLogin();
                    req.nickName = "Dummy " + i++;

                    session.Send(req.Write());
                }

                int i2 = 0;
                foreach (ServerSession session in _sessions)
                {
                    ToS_ReqMakeRoom req = new ToS_ReqMakeRoom();
                    req.roomName = "Room " + i2++;

                    session.Send(req.Write());
                }
            }
        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }
    }
}
