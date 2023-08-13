using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ToS readyGame

// ToC player n turn 

// ToS roll dice
// ToC dice result
// (possible ToS scored )

// ToS roll dice (fix dices)
// ToC dice result
// (possible ToS scored )

// ToS roll dice (fix dices)
// ToC dice result
// (possible ToS scored )

// ToC gameResult

// turn 12 * player

namespace Server
{
    class PlayerGameInfo
    {
        public int index = -1;
        public bool ready = false;
        public List<int> scoreBoard = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    public class YatzyGameRoom
    {
        public int roomID;
        public string roomName;

        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        List<Tuple<ClientSession, ArraySegment<byte>>> _pendingListUni = new List<Tuple<ClientSession, ArraySegment<byte>>>();
        Dictionary<int, PlayerGameInfo> _playerGameInfoDic = new Dictionary<int, PlayerGameInfo>();

        Random random = new Random();

        int gameTurn;
        int _diceCount;
        int[] _dices = new int[5];

        public YatzyGameRoom(int roomID, string name)
        {
            this.roomID = roomID;
            this.roomName = name;
        }

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

            //Console.WriteLine($"Flushed {_pendingList.Count}");
            _pendingList.Clear();
            _pendingListUni.Clear();
        }

        public void BroadCast(IPacket packet)
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
            _playerGameInfoDic.Add(session.SessionId, new PlayerGameInfo() { index = _sessions.Count - 1 });
            session.GameRoom = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
            _playerGameInfoDic.Remove(session.SessionId);

            if (_sessions.Count == 0) GameRoomManager.Instance.RemoveRoom(this.roomID);
        }

        public List<string> GetUserInfos()
        {
            List<string> userInfos = new List<string>();
            foreach (var session in _sessions)
                userInfos.Add(session.nickName);

            return userInfos;
        }

        public int GetUserIndex(ClientSession session)
        {
            PlayerGameInfo info = null;
            _playerGameInfoDic.TryGetValue(session.SessionId, out info);

            return info != null ? info.index : -1;
        }

        public void ReadyUser(ClientSession session)
        {
            PlayerGameInfo info = null;
            _playerGameInfoDic.TryGetValue(session.SessionId, out info);

            if (info == null) return;
            info.ready = true;

            int readyCount = 0;
            foreach (var value in _playerGameInfoDic.Values)
                if (info.ready == true) readyCount++;

            if (readyCount >= 2)
                StartGame();
        }

        public void RollDice(ClientSession session, List<int> fixDices)
        {
            ToC_DiceResult diceResult = new ToC_DiceResult();
            PlayerGameInfo info = null;
            _playerGameInfoDic.TryGetValue(session.SessionId, out info);

            if (isPlayerTurn(info.index) == false)
                return;

            if (_diceCount <= 0 || fixDices.Count >= 5)
                return;

            _diceCount--;

            diceResult.playerIndex = info.index;
            diceResult.leftDice = _diceCount;

            for (int i = 0; i < 5; i++)
            {
                if (!fixDices.Contains(i)) _dices[i] = random.Next(1, 7);
                diceResult.diceResults.Add(new ToC_DiceResult.DiceResult() { dice = _dices[i] });
            }

            BroadCast(diceResult);
        }

        public void WriteScore(ClientSession session, int jocbo)
        {
            PlayerGameInfo info = null;
            _playerGameInfoDic.TryGetValue(session.SessionId, out info);

            if (isPlayerTurn(info.index) == false)
                return;

            if (_diceCount > 2)
                return;

            if (info == null || info.scoreBoard[jocbo] > 0)
                return;
             
            info.scoreBoard[jocbo] = YatzyUtil.GetScore(_dices, jocbo);

            ToC_WriteScore writeScore = new ToC_WriteScore();
            writeScore.playerIndex = info.index;
            writeScore.jocboIndex = jocbo;
            writeScore.jocboScore = info.scoreBoard[jocbo];

            BroadCast(writeScore);
            Push(() => ChangeTurn());
        }

        void StartGame()
        {
            ToC_PlayerTurn packet = new ToC_PlayerTurn();
            packet.playerTurn = gameTurn % _sessions.Count;

            _diceCount = 3;

            BroadCast(packet);
        }

        bool isPlayerTurn(int index)
        {
            return index == gameTurn % _sessions.Count;
        }

        void ChangeTurn()
        {
            gameTurn++;
            _diceCount = 3;

            ToC_PlayerTurn playerTurn = new ToC_PlayerTurn();
            playerTurn.playerTurn = gameTurn % _sessions.Count;

            Push(() => BroadCast(playerTurn));
        }
    }
}

