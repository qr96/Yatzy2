using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class YatzySingleGame
    {
        public int roomID;
        public string roomName;

        ClientSession _session;
        JobQueue _jobQueue = new JobQueue();
        PlayerGameInfo[] _playerGameInfoDic = new PlayerGameInfo[2];

        Random random = new Random((int)DateTime.Now.Ticks);

        int _playerCount = 2;

        int gameTurn;
        int _diceCount;
        int[] _dices = new int[5];

        public YatzySingleGame(ClientSession session)
        {
            this._session = session;
            _playerGameInfoDic[0] = new PlayerGameInfo();
            _playerGameInfoDic[1] = new PlayerGameInfo();
        }

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void UniCast(IPacket packet)
        {
            ArraySegment<byte> segment = packet.Write();

            _session.Send(segment);
        }

        public void RollDice(ClientSession session, List<int> fixDices)
        {
            ToC_DiceResult diceResult = new ToC_DiceResult();
            PlayerGameInfo info = _playerGameInfoDic[0];

            if (IsPlayerTurn(info.index) == false)
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

            UniCast(diceResult);
        }

        public void WriteScore(ClientSession session, int jocbo)
        {
            PlayerGameInfo info = _playerGameInfoDic[0];

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

            UniCast(writeScore);
            if (gameTurn >= 12 * 2 - 1) Push(() => EndGame());
            else Push(() => ChangeTurn());
        }

        public void SendSelectScore(ClientSession session, int scoreIndex)
        {
            PlayerGameInfo info = null;
            //_playerGameInfoDic.TryGetValue(session.SessionId, out info);

            if (info == null) return;

            ToC_SelectScore scoreSelect = new ToC_SelectScore();
            scoreSelect.playerIndex = info.index;
            scoreSelect.jocboIndex = scoreIndex;

            UniCast(scoreSelect);
        }

        void StartGame()
        {
            ToC_PlayerTurn packet = new ToC_PlayerTurn();
            packet.playerTurn = gameTurn % _playerCount;

            _diceCount = 3;

            UniCast(packet);
        }

        void InitGame()
        {
            foreach (var info in _playerGameInfoDic)
                info.RestartGame();

            gameTurn = 0;
            _diceCount = 0;
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

            Push(() => UniCast(playerTurn));
        }

        void EndGame()
        {
            int max = 0;
            int winner = -1;
            bool drawGame = false;

            foreach (var info in _playerGameInfoDic)
            {
                var score = info.GetScoreSum();
                if (score > max)
                {
                    max = score;
                    winner = info.index;
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

            UniCast(endGame);
            Push(() => InitGame());
        }
    }
}
