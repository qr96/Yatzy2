using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YatzyGameScene : MonoBehaviour
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

    public Animation yachtEffect;

    int myServerIndex = -1;
    int nowTurn = 0;
    
    void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResRoomInfo, RecvRoomInfo);
        PacketHandler.AddAction(PacketID.ToC_ResLeaveRoom, RecvLeaveRoom);
        PacketHandler.AddAction(PacketID.ToC_PlayerEnterRoom, RecvPlayerEnterRoom);
        PacketHandler.AddAction(PacketID.ToC_PlayerTurn, RecvPlayerTurn);
        PacketHandler.AddAction(PacketID.ToC_DiceResult, RecvDiceResult);
        PacketHandler.AddAction(PacketID.ToC_WriteScore, RecvWriteScore);
        PacketHandler.AddAction(PacketID.ToC_EndGame, RecvGameEnd);
        PacketHandler.AddAction(PacketID.ToC_LockDice, RecvLockDice);
        PacketHandler.AddAction(PacketID.ToC_SelectScore, RecvSelectScore);

        scoreBoard0.SetListener(() => CheckAllScoreButton());
        scoreBoard1.SetListener(() => CheckAllScoreButton());

        for (int i = 0; i < 5; i++)
        {
            int tmp = i;
            diceToggleList[i].SetClickEvent(() =>
            {
                if (diceToggleList[tmp].IsLocked()) diceViewer.LockDice(tmp);
                else diceViewer.UnLockDice(tmp);

                ToS_LockDice lockDicePacket = new ToS_LockDice();
                lockDicePacket.diceIndex = tmp;
                lockDicePacket.isLocked = diceToggleList[tmp].IsLocked();
                NetworkManager.Instance.Send(lockDicePacket.Write());
            });
        }

        gameResult.SetRestartBtnListener(() => InitGame());
        gameResult.SetLeaveBtnListener(() => OnClickLeaveRoom());

        EnableDiceButton(false);
        EnableRecordScoreButton(false);
        DisableAllScoreButton();

        playerNickName0.text = "-";
        playerNickName1.text = "-";
        SetNowTurn(0);

        ReqRoomInfo();
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_ResRoomInfo);
        PacketHandler.RemoveAction(PacketID.ToC_ResLeaveRoom);
        PacketHandler.RemoveAction(PacketID.ToC_PlayerEnterRoom);
        PacketHandler.RemoveAction(PacketID.ToC_PlayerTurn);
        PacketHandler.RemoveAction(PacketID.ToC_DiceResult);
        PacketHandler.RemoveAction(PacketID.ToC_WriteScore);
        PacketHandler.RemoveAction(PacketID.ToC_EndGame);
        PacketHandler.RemoveAction(PacketID.ToC_LockDice);
        PacketHandler.RemoveAction(PacketID.ToC_SelectScore);
    }

    // Packets
    void ReqRoomInfo()
    {
        ToS_ReqRoomInfo req = new ToS_ReqRoomInfo();
        NetworkManager.Instance.Send(req.Write());
    }

    void ReqReadyToStart()
    {
        ToS_ReadyToStart req = new ToS_ReadyToStart();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvRoomInfo(IPacket packet)
    {
        Debug.Log("RecvRoomInfo");

        ErrorManager.Instance.HideLoadingIndicator();

        ToC_ResRoomInfo roomInfo = packet as ToC_ResRoomInfo;
        if (roomInfo != null)
        {
            myServerIndex = roomInfo.myServerIndex;

            if (roomInfo.userInfos.Count > 0)
                playerNickName0.text = roomInfo.userInfos[0].userName;
            else if (roomInfo.userInfos.Count > 1)
                playerNickName1.text = roomInfo.userInfos[1].userName;
        }

        SetNowTurn(0);

        ReqReadyToStart();
    }

    void RecvLeaveRoom(IPacket packet)
    {
        Debug.Log("RecvLeaveRoom");

        ToC_ResLeaveRoom leaveRoom = packet as ToC_ResLeaveRoom;

        if(leaveRoom != null)
        {
            if (leaveRoom.leavePlayerIndex == myServerIndex)
            {
                ErrorManager.Instance.HideLoadingIndicator();
                SceneManager.Instance.MoveScene(1);
            }
            else
            {
                if (leaveRoom.leavePlayerIndex == 0) playerNickName0.text = "-";
                else if (leaveRoom.leavePlayerIndex == 1) playerNickName1.text = "-";
            }
        }
    }

    void RecvPlayerEnterRoom(IPacket packet)
    {
        ToC_PlayerEnterRoom playerInfo = packet as ToC_PlayerEnterRoom;
        if (playerInfo != null)
        {
            if (playerInfo.playerIndex == 0)
                playerNickName0.text = playerInfo.playerNickName;
            else if (playerInfo.playerIndex == 1)
                playerNickName1.text = playerInfo.playerNickName;
        }
    }

    void RecvPlayerTurn(IPacket packet)
    {
        Debug.Log("RecvPlayerTurn");

        ToC_PlayerTurn p = packet as ToC_PlayerTurn;

        if (p != null)
        {
            ShowPlayerTurnLight(p.playerTurn);
            DisableAllScoreButton();
            UnlockAllDiceLocks();
            InitAllDiceNumbers();
            InitAllPreviewScores();
            EnableAllUnlockDices(false);
            EnableDiceButton(myServerIndex == p.playerTurn);
            leftDiceCount.text = "Roll(3)";
            SetNowTurn(++nowTurn);
        }
    }

    void RecvDiceResult(IPacket packet)
    {
        Debug.Log("RecvDiceResult");

        ToC_DiceResult diceResult = packet as ToC_DiceResult;
        if(diceResult != null)
        {
            // 데이터 가공
            List<int> dices = new List<int>();
            for (int i = 0; i < diceResult.diceResults.Count; i++)
                dices.Add(diceResult.diceResults[i].dice);

            // 다이스 버튼 설정
            leftDiceCount.text = $"Roll({diceResult.leftDice})";
            for (int i = 0; i < diceResult.diceResults.Count; i++)
                diceViewer.SetDice(i, diceResult.diceResults[i].dice);
            diceViewer.PlayRollDice(()=>
            {
                EnableDiceButton(diceResult.playerIndex == myServerIndex && diceResult.leftDice > 0);

                for (int i = 0; i < diceResult.diceResults.Count; i++)
                    diceToggleList[i].SetDice(diceResult.diceResults[i].dice);

                // 스코어 버튼 설정
                DisableAllScoreButton();

                for (int i = 0; i < 12; i++)
                {
                    if (diceResult.playerIndex == 0)
                    {
                        scoreBoard0.SetPreviewScore(i, YatzyUtil.GetScore(dices, i));

                        if (diceResult.playerIndex == myServerIndex && scoreBoard0.GetScore(i) < 0)
                            scoreBoard0.SetEnableScore(i);
                    }
                    else if (diceResult.playerIndex == 1)
                    {
                        scoreBoard1.SetPreviewScore(i, YatzyUtil.GetScore(dices, i));

                        if (diceResult.playerIndex == myServerIndex && scoreBoard1.GetScore(i) < 0)
                            scoreBoard1.SetEnableScore(i);
                    }
                }

                if (diceResult.playerIndex == myServerIndex)
                    EnableAllUnlockDices(true);

                // yacht 이펙트
                bool yacht = true;
                for (int i = 0; i < dices.Count - 1; i++)
                {
                    if (dices[i] != dices[i + 1])
                    {
                        yacht = false;
                        break;
                    }
                }
                if (yacht)
                {
                    Debug.Log("Yacht!!");
                    yachtEffect.Play();
                }
            });
        }
    }

    void RecvWriteScore(IPacket packet)
    {
        Debug.Log("RecvWriteScore");

        ToC_WriteScore writeScore = packet as ToC_WriteScore;

        if (writeScore == null) return;

        AllScoreToggleOff();

        if (writeScore.playerIndex == 0)
            scoreBoard0.SetScore(writeScore.jocboIndex, writeScore.jocboScore);
        else if (writeScore.playerIndex == 1)
            scoreBoard1.SetScore(writeScore.jocboIndex, writeScore.jocboScore);

        UpdateScoreBoard(writeScore.playerIndex);
    }

    void RecvGameEnd(IPacket packet)
    {
        Debug.Log("RecvGameEnd");

        ToC_EndGame endGame = packet as ToC_EndGame;
        if (endGame == null) return;

        if (endGame.drawGame)
            gameResult.ShowResult(GameResult.Draw);
        else if (endGame.winner == myServerIndex)
            gameResult.ShowResult(GameResult.Win);
        else
            gameResult.ShowResult(GameResult.Defeat);
    }

    void RecvLockDice(IPacket packet)
    {
        Debug.Log("RecvLockDice");

        ToC_LockDice lockDice = packet as ToC_LockDice;
        if (lockDice == null) return;

        diceToggleList[lockDice.diceIndex].ToggleOn(lockDice.isLocked);

        if (lockDice.isLocked) diceViewer.LockDice(lockDice.diceIndex);
        else diceViewer.UnLockDice(lockDice.diceIndex);
    }

    void RecvSelectScore(IPacket packet)
    {
        Debug.Log("RecvSelectScore");

        ToC_SelectScore toC_SelectScore = packet as ToC_SelectScore;
        if (toC_SelectScore == null) return;
        if (toC_SelectScore.playerIndex == myServerIndex) return;

        AllScoreToggleOff();

        if (toC_SelectScore.playerIndex == 0)
            scoreBoard0.SelectScore(toC_SelectScore.jocboIndex);
        else if (toC_SelectScore.playerIndex == 1)
            scoreBoard1.SelectScore(toC_SelectScore.jocboIndex);
    }

    // Game UI
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
            if (subTotal >= 63) bonusScore0.text = "+35";

            totalScore0.text = total.ToString();
        }
        else if (player == 1)
        {
            subTotal = scoreBoard1.GetSubTotalScore();
            total = scoreBoard1.GetTotalScore();

            subTotalScore1.text = $"{subTotal}/63";
            if (subTotal >= 63) bonusScore1.text = "+35";

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
        int score = -1;

        if (myServerIndex == 0)
        {
            selected = scoreBoard0.SelectedIndex();
            score = scoreBoard0.GetScore(selected);
        }
        else if (myServerIndex == 1)
        {
            selected = scoreBoard1.SelectedIndex();
            score = scoreBoard1.GetScore(selected);
        }

        EnableRecordScoreButton(selected >= 0 && score < 0);
        if (selected == -1) return;

        ToS_SelectScore toS_SelectScore = new ToS_SelectScore();
        toS_SelectScore.jocboIndex = selected;
        NetworkManager.Instance.Send(toS_SelectScore.Write());
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

    // Events

    public void OnClickLeaveRoom()
    {
        ErrorManager.Instance.ShowLoadingIndicator();

        ToS_ReqLeaveRoom packet = new ToS_ReqLeaveRoom();
        NetworkManager.Instance.Send(packet.Write());
    }

    public void OnClickRollDice()
    {
        int unlockedCount = 0;

        ToS_RollDice packet = new ToS_RollDice();
        for (int i = 0; i < 5; i++)
        {
            if (diceToggleList[i].IsLocked()) packet.fixDices.Add(new ToS_RollDice.FixDice() { diceIndex = i });
            else unlockedCount++;
        }

        if (unlockedCount <= 0) return;

        EnableDiceButton(false);
        NetworkManager.Instance.Send(packet.Write());
    }

    public void OnClickRecordScore()
    {
        int selected = -1;

        if (myServerIndex == 0)
            selected = scoreBoard0.SelectedIndex();
        else if (myServerIndex == 1)
            selected = scoreBoard1.SelectedIndex();

        if (selected < 0) return;

        AllScoreToggleOff();
        EnableDiceButton(false);
        EnableRecordScoreButton(false);
        DisableAllScoreButton();
        
        ToS_WriteScore packet = new ToS_WriteScore();
        packet.jocboIndex = selected;
        NetworkManager.Instance.Send(packet.Write());
    }
}
