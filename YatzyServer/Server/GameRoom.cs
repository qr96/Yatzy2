using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        List<Tuple<ClientSession, ArraySegment<byte>>> _pendingListUni = new List<Tuple<ClientSession, ArraySegment<byte>>>();

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

            foreach (var pending in _pendingListUni)
            {
                pending.Item1.Send(pending.Item2);
            }
            
            Console.WriteLine($"Flushed {_pendingList.Count}");
            _pendingList.Clear();
            _pendingListUni.Clear();
        }

        public void BroadCast(ClientSession session, IPacket packet)
        {
            ArraySegment<byte> segment = packet.Write();

            _pendingList.Add(segment);
        }

        public void UniCast(ClientSession session, IPacket packet)
        {
            ArraySegment<byte> segment = packet.Write();

            _pendingListUni.Add(new Tuple<ClientSession, ArraySegment<byte>>(session, segment));
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
        }
    }
}
