using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Lobby = new GameRoom();

        static void FlushRoom()
        {
            GameRoomManager.Instance.Flush();
            Lobby.Push(() => Lobby.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static IPEndPoint GetIPEndPoint(BuildType buildType)
        {
            if (buildType == BuildType.REAL)
            {
                string host = Dns.GetHostName();
                IPHostEntry ipHost = Dns.GetHostEntry(host);
                IPAddress ipAddr = ipHost.AddressList[0];
                return new IPEndPoint(ipAddr, 7777);
            }
            else
            {
                return new IPEndPoint(IPAddress.Any, 7777);
            }
        }

        static void Main(string[] args)
        {
            IPEndPoint endPoint = GetIPEndPoint(BuildType.ALPHA);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            //FlushRoom();
            JobTimer.Instance.Push(FlushRoom);

            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    }

    enum BuildType
    {
        ALPHA = 0,
        REAL = 1,

    }
}
