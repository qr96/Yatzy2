using Server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YatzySingleGame : MonoBehaviour
{
    public TextMeshProUGUI nowTurnInfo;
    public TextMeshProUGUI playerNickName0;
    public TextMeshProUGUI playerNickName1;
    public GameObject turnLight0;
    public GameObject turnLight1;

    public GameObject gameSheet;

    public ScoreBoard scoreBoard0;
    public ScoreBoard scoreBoard1;
    public TextMeshProUGUI subTotalScore0;
    public TextMeshProUGUI subTotalScore1;
    public TextMeshProUGUI bonusScore0;
    public TextMeshProUGUI bonusScore1;
    public TextMeshProUGUI totalScore0;
    public TextMeshProUGUI totalScore1;

    public List<DiceToggle> diceToggleList;
    public Button rollDice;
    public Button recordScore;
    public TextMeshProUGUI leftDiceCount;

    public DiceViewer diceViewer;

    public GameResultPopup gameResult;

    List<int> _dices = new List<int>();
    int myServerIndex = 0;
    int nowTurn = 0;

    void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResSingleRoomInfo, RecvSingleRoomInfo);
        PacketHandler.AddAction(PacketID.ToC_SingleStartGame, RecvStartGame);
        PacketHandler.AddAction(PacketID.ToC_SingleDiceResult, RecvDiceResult);
        PacketHandler.AddAction(PacketID.ToC_SingleMobPlayResult, RecvNpcPlayResult);
        PacketHandler.AddAction(PacketID.ToC_ResLeaveSingleRoom, RecvLeaveSingleRoom);

        scoreBoard0.SetListener(() => CheckAllScoreButton());
        scoreBoard1.SetListener(() => CheckAllScoreButton());

        for (int i = 0; i < 5; i++)
        {
            int tmp = i;
            diceToggleList[i].SetClickEvent(() =>
            {
                if (diceToggleList[tmp].IsLocked()) diceViewer.LockDice(tmp);
                else diceViewer.UnLockDice(tmp);
            });
        }

        gameResult.SetRestartBtnListener(() => InitGame());
        gameResult.SetLeaveBtnListener(() =>
        {
            ErrorManager.Instance.ShowLoadingIndicator();
            ReqLeaveRoom();
        });

        EnableDiceButton(false);
        EnableRecordScoreButton(false);
        DisableAllScoreButton();

        playerNickName0.text = "-";
        playerNickName1.text = "-";
        SetNowTurn(0);

        ReqSingleRoomInfo();
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_ResSingleRoomInfo);
        PacketHandler.RemoveAction(PacketID.ToC_SingleStartGame);
        PacketHandler.RemoveAction(PacketID.ToC_SingleDiceResult);
        PacketHandler.RemoveAction(PacketID.ToC_SingleMobPlayResult);
        PacketHandler.RemoveAction(PacketID.ToC_ResLeaveSingleRoom);
    }

    // Packets
    void ReqSingleRoomInfo()
    {
        ToS_ReqSingleRoomInfo req = new ToS_ReqSingleRoomInfo();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvSingleRoomInfo(IPacket packet)
    {
        ToC_ResSingleRoomInfo res = packet as ToC_ResSingleRoomInfo;
        SetUserNicknames(res.userName, res.mobName);

        ReqReadyToStart();

        ErrorManager.Instance.HideLoadingIndicator();
    }

    void ReqReadyToStart()
    {
        ToS_SingleReadyToStart req = new ToS_SingleReadyToStart();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvStartGame(IPacket packet)
    {
        ToC_SingleStartGame res = packet as ToC_SingleStartGame;

        ShowPlayerTurnLight(0);
        DisableAllScoreButton();
        UnlockAllDiceLocks();
        InitAllDiceNumbers();
        InitAllPreviewScores();
        EnableAllUnlockDices(false);
        EnableDiceButton(true);
        leftDiceCount.text = "Roll(3)";
        SetNowTurn(++nowTurn);
    }

    void ReqRollDice(List<int> fixDices)
    {
        ToS_SingleRollDice req = new ToS_SingleRollDice();
        foreach (var dice in fixDices)
            req.fixDices.Add(new ToS_SingleRollDice.FixDice() { diceIndex = dice });

        NetworkManager.Instance.Send(req.Write());
    }

    void RecvDiceResult(IPacket packet)
    {
        Debug.Log("RecvDiceResult");
        ToC_SingleDiceResult diceResult = packet as ToC_SingleDiceResult;

        if (diceResult != null)
        {
            // 데이터 가공
            _dices.Clear();
            for (int i = 0; i < diceResult.diceResults.Count; i++)
                _dices.Add(diceResult.diceResults[i].dice);

            // 다이스 버튼 설정
            RollDice(_dices, diceResult.leftDice, 0);
        }
    }

    void ReqWriteScore(int scoreIndex)
    {
        ToS_SingleWriteScore req = new ToS_SingleWriteScore();
        req.jocboIndex = scoreIndex;
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvNpcPlayResult(IPacket packet)
    {
        ToC_SingleMobPlayResult res = packet as ToC_SingleMobPlayResult;

        if (res == null) return;

        ShowPlayerTurnLight(1);
        DisableAllScoreButton();
        UnlockAllDiceLocks();
        InitAllDiceNumbers();
        InitAllPreviewScores();
        EnableAllUnlockDices(false);
        EnableDiceButton(false);
        leftDiceCount.text = "Roll(3)";
        SetNowTurn(++nowTurn);

        List<List<int>> diceResultList = new List<List<int>>();
        List<List<int>> diceLockList = new List<List<int>>();

        foreach (var diceResultPacket in res.diceResultLists)
        {
            List<int> diceResult = new List<int>();
            foreach (var dice in diceResultPacket.diceResults)
                diceResult.Add(dice.diceNum);
            diceResultList.Add(diceResult);
        }
        foreach (var diceLocksPacket in res.diceLockLists)
        {
            List<int> diceLocks = new List<int>();
            foreach (var diceIndex in diceLocksPacket.diceLocks)
                diceLocks.Add(diceIndex.diceIndex);
            diceLockList.Add(diceLocks);
        }

        StartCoroutine(NpcPlayCo(diceResultList, diceLockList, res.jocboIndex, res.jocboScore));
    }

    void ReqLeaveRoom()
    {
        ToS_ReqLeaveSingleRoom req = new ToS_ReqLeaveSingleRoom();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvLeaveSingleRoom(IPacket packet)
    {
        ToC_ResLeaveSingleRoom res = packet as ToC_ResLeaveSingleRoom;
        ErrorManager.Instance.ShowLoadingIndicator();
        SceneManager.Instance.MoveScene(1, 0);
    }


    // Scene Setting
    void InitGame()
    {
        subTotalScore0.text = "0/63";
        subTotalScore1.text = "0/63";
        totalScore0.text = "0";
        totalScore1.text = "0";
        scoreBoard0.InitScoreBoard();
        scoreBoard1.InitScoreBoard();
        DisableAllScoreButton();
        UnlockAllDiceLocks();
        InitAllDiceNumbers();
        EnableDiceButton(false);
        EnableRecordScoreButton(false);
        InitPlayerTurnLight();
    }

    void SetUserNicknames(string user0, string user1)
    {
        playerNickName0.text = user0;
        playerNickName1.text = user1;
    }

    void EnableDiceButton(bool enable)
    {
        rollDice.interactable = enable;
    }

    void EnableRecordScoreButton(bool enable)
    {
        recordScore.interactable = enable;
    }

    void DisableAllScoreButton()
    {
        scoreBoard0.DisableAll();
        scoreBoard1.DisableAll();
    }

    public void UpdateScoreBoard(int player)
    {
        int subTotal = 0;
        int total = 0;

        if (player == 0)
        {
            subTotal = scoreBoard0.GetSubTotalScore();
            total = scoreBoard0.GetTotalScore();

            subTotalScore0.text = $"{subTotal}/63";
            if (subTotal >= 63)
            {
                total += 35;
                bonusScore0.text = "+35";
            }
            totalScore0.text = total.ToString();
        }
        else if (player == 1)
        {
            subTotal = scoreBoard1.GetSubTotalScore();
            total = scoreBoard1.GetTotalScore();

            subTotalScore1.text = $"{subTotal}/63";
            if (subTotal >= 63)
            {
                total += 35;
                bonusScore1.text = "+35";
            }
            totalScore1.text = total.ToString();
        }
    }

    void UnlockAllDiceLocks()
    {
        foreach (var diceLock in diceToggleList)
            diceLock.ToggleOn(false);

        diceViewer.UnLockAllDice();
    }

    void InitAllDiceNumbers()
    {
        foreach (var dice in diceToggleList)
        {
            dice.SetDice(1);
        }
    }

    void InitAllPreviewScores()
    {
        scoreBoard0.InitAllPreviewScore();
        scoreBoard1.InitAllPreviewScore();
    }

    void EnableAllUnlockDices(bool enable)
    {
        foreach (var diceLock in diceToggleList)
            diceLock.EnableToggle(enable);
    }

    void ShowPlayerTurnLight(int index)
    {
        turnLight0.SetActive(index == 0);
        turnLight1.SetActive(index == 1);
        scoreBoard0.SetTurnLight(index == 0);
        scoreBoard1.SetTurnLight(index == 1);
    }

    void InitPlayerTurnLight()
    {
        turnLight0.SetActive(false);
        turnLight1.SetActive(false);
        scoreBoard0.SetTurnLight(false);
        scoreBoard1.SetTurnLight(false);
    }

    void CheckAllScoreButton()
    {
        int selected = -1;

        selected = scoreBoard0.SelectedIndex();

        EnableRecordScoreButton(selected >= 0);
        if (selected == -1) return;

        AllScoreToggleOff();
        scoreBoard0.SelectScore(selected);
    }

    void AllScoreToggleOff()
    {
        scoreBoard0.UnSelectAll();
        scoreBoard1.UnSelectAll();
    }

    void SetNowTurn(int turn)
    {
        nowTurn = turn;
        nowTurnInfo.text = $"{nowTurn}/24";
    }

    void RollDice(List<int> diceList, int leftDice, int playerIndex)
    {
        leftDiceCount.text = $"Roll({leftDice})";
        for (int i = 0; i < diceList.Count; i++)
            diceViewer.SetDice(i, diceList[i]);
        diceViewer.PlayRollDice(() =>
        {
            EnableDiceButton(myServerIndex == playerIndex && leftDice > 0);

            for (int i = 0; i < diceList.Count; i++)
                diceToggleList[i].SetDice(diceList[i]);

            // 스코어 버튼 설정
            DisableAllScoreButton();

            for (int i = 0; i < 12; i++)
            {
                if (playerIndex == 0)
                {
                    scoreBoard0.SetPreviewScore(i, YatzyUtil.GetScore(diceList, i));
                    scoreBoard0.SetEnableScore(i);
                }
                else if (playerIndex == 1)
                {
                    scoreBoard1.SetPreviewScore(i, YatzyUtil.GetScore(diceList, i));
                    scoreBoard1.SetEnableScore(i);
                }
            }

            EnableAllUnlockDices(true);
        });
    }

    IEnumerator NpcPlayCo(List<List<int>> diceResultList, List<List<int>> diceLockList, int selectIndex, int selectScore)
    {
        for (int i = 0; i < diceResultList.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            RollDice(diceResultList[i], 2 - i, 1);
            yield return new WaitForSeconds(1f);

            if (diceLockList.Count > i)
            {
                string locks = "";
                for (int j = 0; j < 5; j++)
                {
                    if (diceLockList[i].Contains(j))
                    {
                        locks += $", {j}";
                        diceToggleList[j].ToggleOn(true);
                        diceViewer.LockDice(j);
                    }
                    else
                    {
                        diceToggleList[j].ToggleOn(false);
                        diceViewer.UnLockDice(j);
                    }
                    yield return new WaitForSeconds(0.3f);
                }
                Debug.Log(locks);
            }
        }

        AllScoreToggleOff();
        scoreBoard1.SetScore(selectIndex, selectScore);
        UpdateScoreBoard(1);

        if (nowTurn >= 24)
        {
            if (scoreBoard0.GetTotalScore() > scoreBoard1.GetTotalScore())
            {
                DataCacheManager.Instance.lastDevilCastleResult = GameResult.Win;
                gameResult.ShowResult(GameResult.Win);
            }
            else if (scoreBoard0.GetTotalScore() == scoreBoard1.GetTotalScore())
            {
                DataCacheManager.Instance.lastDevilCastleResult = GameResult.Draw;
                gameResult.ShowResult(GameResult.Draw);
            }
            else
            {
                DataCacheManager.Instance.lastDevilCastleResult = GameResult.Defeat;
                gameResult.ShowResult(GameResult.Defeat);
            }
            
            yield break;
        }

        // My Turn
        ShowPlayerTurnLight(0);
        DisableAllScoreButton();
        UnlockAllDiceLocks();
        InitAllDiceNumbers();
        InitAllPreviewScores();
        EnableAllUnlockDices(false);
        EnableDiceButton(true);
        leftDiceCount.text = "Roll(3)";
        SetNowTurn(++nowTurn);
    }



    // Events

    public void OnClickLeaveRoom()
    {
        ErrorManager.Instance.ShowQuestionPopup("안내", "정말로 기권하고 나가시겠습니까? 게임이 패배처리 됩니다.", () =>
        {
            ErrorManager.Instance.ShowLoadingIndicator();
            ReqLeaveRoom();
        });
    }

    public void OnClickRollDice()
    {
        int unlockedCount = 0;
        List<int> fixDices = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            if (diceToggleList[i].IsLocked()) fixDices.Add(i);
            else unlockedCount++;
        }

        if (unlockedCount <= 0) return;

        EnableDiceButton(false);
        ReqRollDice(fixDices);
    }

    public void OnClickRecordScore()
    {
        int selected = -1;

        selected = scoreBoard0.SelectedIndex();

        if (selected < 0) return;

        AllScoreToggleOff();
        EnableDiceButton(false);
        EnableRecordScoreButton(false);
        DisableAllScoreButton();

        scoreBoard0.SetScore(selected, YatzyUtil.GetScore(_dices, selected));
        UpdateScoreBoard(0);

        ReqWriteScore(selected);
    }
}
