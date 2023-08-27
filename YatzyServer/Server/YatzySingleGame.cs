using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToC_SingleMobPlayResult;

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

        public void StartGame(ClientSession session)
        {
            _diceCount = 3;
            gameTurn = 0;

            UniCast(new ToC_SingleStartGame());
        }

        public void RollDice(ClientSession session, List<int> fixDices)
        {
            ToC_SingleDiceResult diceResult = new ToC_SingleDiceResult();

            if (_diceCount <= 0 || fixDices.Count >= 5)
                return;

            if (_diceCount >= 3 && fixDices.Count > 0)
                return;

            _diceCount--;

            diceResult.leftDice = _diceCount;

            for (int i = 0; i < 5; i++)
            {
                if (!fixDices.Contains(i)) _dices[i] = random.Next(1, 7);
                diceResult.diceResults.Add(new ToC_SingleDiceResult.DiceResult() { dice = _dices[i] });
            }

            UniCast(diceResult);
        }

        public void WriteScore(ClientSession session, int jocbo)
        {
            PlayerGameInfo info = _playerGameInfoDic[0];

            if (_diceCount > 2)
                return;

            if (info == null || info.scoreBoard[jocbo] >= 0)
                return;

            info.scoreBoard[jocbo] = YatzyUtil.GetScore(_dices, jocbo);

            Push(() => NpcPlay());
        }

        void NpcPlay()
        {
            gameTurn += 2;
            _diceCount = 3;

            ToC_SingleMobPlayResult playResult = new ToC_SingleMobPlayResult();
            
            // NPC 플레이
            int writeScore = -1;
            bool[] lockDices = new bool[5] { false, false, false, false, false };

            for (int i = 2; i >= 0; i--)
            {
                DiceResultList diceResult = new DiceResultList();
                DiceLockList diceLockList = new DiceLockList();

                // 주사위 던짐
                for (int j = 0; j < 5; j++)
                {
                    if (!lockDices[j])
                        _dices[j] = random.Next(1, 7);

                    diceResult.diceResults.Add(new DiceResultList.DiceResult() { diceNum = _dices[j] });
                }
                playResult.diceResultLists.Add(diceResult);

                writeScore = YatzyUtil.GetBotWantWriteScore(_dices, _playerGameInfoDic[1].scoreBoard, i);
                if (writeScore >= 0) break;

                lockDices = YatzyUtil.GetBotWantLockDice(_dices, _playerGameInfoDic[1].scoreBoard, i);
                for (int j = 0; j < 5; j++)
                {
                    if (lockDices[j])
                        diceLockList.diceLocks.Add(new DiceLockList.DiceLock() { diceIndex = j });
                }
                playResult.diceLockLists.Add(diceLockList);
            }

            _playerGameInfoDic[1].scoreBoard[writeScore] = YatzyUtil.GetScore(_dices, writeScore);
            
            playResult.jocboIndex = writeScore;
            playResult.jocboScore = _playerGameInfoDic[1].scoreBoard[writeScore];

            Push(() => UniCast(playResult));

            if (gameTurn >= 24)
                Push(() => EndGame());
        }

        void EndGame()
        {
            int winner = -1;
            bool drawGame = false;

            if (_playerGameInfoDic[0].GetScoreSum() > _playerGameInfoDic[1].GetScoreSum())
                winner = 0;
            else if (_playerGameInfoDic[0].GetScoreSum() == _playerGameInfoDic[1].GetScoreSum())
                drawGame = true;
            else
                winner = 1;
        }
    }
}
