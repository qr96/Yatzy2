using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
    public class YatzyGameRoom
    {
        public int roomID;
        public string roomName;

        ClientSession[] _sessions = new ClientSession[2];
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        Dictionary<int, PlayerGameInfo> _playerGameInfoDic = new Dictionary<int, PlayerGameInfo>();

        int _playerCount = 0;

        Random random = new Random((int)DateTime.Now.Ticks);

        int gameTurn;
        int _diceCount;
        int[] _dices = new int[5];
        bool _gameStarted = false;

        static int MAX_PLAYER = 2;

        public YatzyGameRoom(int roomID, string name)
        {
            this.roomID = roomID;
            this.roomName = name;
        }

        public int GetUserCount()
        {
            return _playerCount;
        }

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _sessions)
            {
                if (s != null)
                    s.Send(_pendingList);
            }

            //Console.WriteLine($"Flushed {_pendingList.Count}");
            _pendingList.Clear();
        }

        public void BroadCast(IPacket packet)
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
            if (_playerCount >= MAX_PLAYER) return;
            if (_sessions.Contains(session) || _playerGameInfoDic.ContainsKey(session.SessionId)) return;
            _sessions[_playerCount] = session;
            _playerGameInfoDic.Add(session.SessionId, new PlayerGameInfo() { index = _playerCount });
            session.GameRoom = this;
            _playerCount++;
        }

        public void Leave(ClientSession session)
        {
            Console.WriteLine("LeaveRoom");
            PlayerGameInfo info = null;
            _playerGameInfoDic.TryGetValue(session.SessionId, out info);
            if (info == null) return;

            ToC_ResLeaveRoom leaveRoom = new ToC_ResLeaveRoom();
            leaveRoom.leavePlayerIndex = info.index;

            _sessions[info.index] = null;
            _playerGameInfoDic.Remove(session.SessionId);
            _playerCount--;

            session.GameRoom = null;
            session.Send(leaveRoom.Write());

            if (_playerCount <= 0) GameRoomManager.Instance.RemoveRoom(this.roomID);
            else
            {
                BroadCast(leaveRoom);
                if (_gameStarted) Push(() => RetireWinEnd());
            }
        }

        public List<string> GetUserInfos()
        {
            List<string> userInfos = new List<string>();
            foreach (var session in _sessions)
            {
                if (session != null)
                    userInfos.Add(session.userId);
            }
                

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

            if (IsPlayerTurn(info.index) == false)
                return;

            if (_diceCount <= 0 || fixDices.Count >= 5)
                return;

            if (_diceCount >= 3 && fixDices.Count > 0)
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

            if (IsPlayerTurn(info.index) == false)
                return;

            if (_diceCount > 2)
                return;

            if (info == null || info.scoreBoard[jocbo] >= 0)
                return;
             
            info.scoreBoard[jocbo] = YatzyUtil.GetScore(_dices, jocbo);

            ToC_WriteScore writeScore = new ToC_WriteScore();
            writeScore.playerIndex = info.index;
            writeScore.jocboIndex = jocbo;
            writeScore.jocboScore = info.scoreBoard[jocbo];

            BroadCast(writeScore);
            if (gameTurn >= 12 * 2 - 1) Push(() => EndGame());
            else Push(() => ChangeTurn());
        }

        public void SendSelectScore(ClientSession session, int scoreIndex)
        {
            PlayerGameInfo info = null;
            _playerGameInfoDic.TryGetValue(session.SessionId, out info);

            if (info == null) return;

            ToC_SelectScore scoreSelect = new ToC_SelectScore();
            scoreSelect.playerIndex = info.index;
            scoreSelect.jocboIndex = scoreIndex;

            BroadCast(scoreSelect);
        }

        void StartGame()
        {
            ToC_PlayerTurn packet = new ToC_PlayerTurn();
            packet.playerTurn = gameTurn % _playerCount;

            _diceCount = 3;
            _gameStarted = true;

            BroadCast(packet);
        }

        void InitGame()
        {
            foreach (var session in _sessions)
            {
                if (session != null)
                    _playerGameInfoDic[session.SessionId].RestartGame();
            }
            
            gameTurn = 0;
            _diceCount = 0;
            _gameStarted = false;
        }

        bool IsPlayerTurn(int index)
        {
            return index == gameTurn % _playerCount;
        }

        void ChangeTurn()
        {
            gameTurn++;
            _diceCount = 3;

            ToC_PlayerTurn playerTurn = new ToC_PlayerTurn();
            playerTurn.playerTurn = gameTurn % _playerCount;

            Push(() => BroadCast(playerTurn));
        }

        void EndGame()
        {
            int max = 0;
            int winner = -1;
            bool drawGame = false;

            foreach (var info in _playerGameInfoDic)
            {
                var score = info.Value.GetScoreSum();
                if (score > max)
                {
                    max = score;
                    winner = info.Value.index;
                }
                else if (score == max)
                {
                    drawGame = true;
                    winner = -1;
                    break;
                }
            }
            
            ToC_EndGame endGame = new ToC_EndGame();
            endGame.winner = winner;
            endGame.drawGame = drawGame;

            _gameStarted = false;

            BroadCast(endGame);
            Push(() => InitGame());
        }

        void RetireWinEnd()
        {
            PlayerGameInfo info = null;
            if (_sessions[0] != null)
                _playerGameInfoDic.TryGetValue(_sessions[0].SessionId, out info);
            else if (_sessions[1] != null)
                _playerGameInfoDic.TryGetValue(_sessions[1].SessionId, out info);
            else
                return;

            if (info == null) return;

            ToC_EndGame endGame = new ToC_EndGame();
            endGame.winner = info.index;
            endGame.drawGame = false;

            _gameStarted = false;

            BroadCast(endGame);
            Push(() => InitGame());
        }
    }
}

