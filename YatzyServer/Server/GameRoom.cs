using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _sessions)
            {
                s.Send(_pendingList);
            }
            
            //Console.WriteLine($"Flushed {_pendingList.Count}");
            _pendingList.Clear();
        }

        public void BroadCast(ClientSession session, IPacket packet)
        {
            ArraySegment<byte> segment = packet.Write();

            _pendingList.Add(segment);
        }

        public void UniCast(ClientSession session, IPacket packet)
        {
            ArraySegment<byte> segment = packet.Write();

            session.Send(segment);
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Lobby = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
        }
    }
}
