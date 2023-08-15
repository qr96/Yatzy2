using Server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ToC_DiceResult;

public class YatzyGameScene : MonoBehaviour
{
    public TextMeshProUGUI playerNickName0;
    public TextMeshProUGUI playerNickName1;

    public GameObject playerTurn0;
    public GameObject playerTurn1;

    public ToggleGroup scoreGroup;
    public List<ScoreItem> scoreListPlayer0;
    public List<ScoreItem> scoreListPlayer1;
    public TextMeshProUGUI subTotalScore0;
    public TextMeshProUGUI subTotalScore1;
    public TextMeshProUGUI totalScore0;
    public TextMeshProUGUI totalScore1;

    public List<DiceToggle> diceToggleList;
    public Button rollDice;
    public Button recordScore;
    public TextMeshProUGUI leftDiceCount;

    public DiceViewer diceViewer;

    int myServerIndex = -1;

    void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResRoomInfo, RecvRoomInfo);
        PacketHandler.AddAction(PacketID.ToC_ResLeaveRoom, RecvLeaveRoom);
        PacketHandler.AddAction(PacketID.ToC_PlayerEnterRoom, RecvPlayerEnterRoom);
        PacketHandler.AddAction(PacketID.ToC_PlayerTurn, RecvPlayerTurn);
        PacketHandler.AddAction(PacketID.ToC_DiceResult, RecvDiceResult);
        PacketHandler.AddAction(PacketID.ToC_WriteScore, RecvWriteScore);

        foreach (var item in scoreListPlayer0)
            item.SetListener(() => CheckAllScoreButton());
        foreach (var item in scoreListPlayer1)
            item.SetListener(() => CheckAllScoreButton());

        EnableDiceButton(false);
        EnableRecordScoreButton(false);
        DisableAllScoreButton();

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
                SceneManager.LoadScene(1);
            }
            else
            {

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
            ShowPlayerTurn(p.playerTurn);
            DisableAllScoreButton();
            UnlockAllDiceLocks();
            InitAllDiceNumbers();
            InitAllPreviewScores();
            EnableAllUnlockDices(false);
            EnableDiceButton(myServerIndex == p.playerTurn);
            leftDiceCount.text = "남은 횟수 : 3";
        }
    }

    void RecvDiceResult(IPacket packet)
    {
        Debug.Log("RecvDiceResult");

        ToC_DiceResult diceResult = packet as ToC_DiceResult;
        if(diceResult != null)
        {
            List<int> dices = new List<int>();

            // 다이스 버튼 설정
            leftDiceCount.text = $"남은 횟수 : {diceResult.leftDice}";
            EnableDiceButton(diceResult.playerIndex == myServerIndex && diceResult.leftDice > 0);

            for (int i = 0; i < diceResult.diceResults.Count; i++)
            {
                dices.Add(diceResult.diceResults[i].dice);
                diceToggleList[i].SetDice(diceResult.diceResults[i].dice);
                diceViewer.SetDice(i, diceResult.diceResults[i].dice);
            }

            diceViewer.PlayRollDice();

            // 스코어 버튼 설정
            DisableAllScoreButton();

            for (int i = 0; i < 12; i++)
            {
                if (diceResult.playerIndex == 0)
                {
                    if (scoreListPlayer0[i].GetScore() == -1)
                        scoreListPlayer0[i].SetPrivewScore(YatzyUtil.GetScore(dices, i));

                    if (diceResult.playerIndex == myServerIndex && scoreListPlayer0[i].GetScore() < 0)
                        scoreListPlayer0[i].SetToggleEnable(true);
                }
                else if (diceResult.playerIndex == 1)
                {
                    if (scoreListPlayer1[i].GetScore() == -1)
                        scoreListPlayer1[i].SetPrivewScore(YatzyUtil.GetScore(dices, i));

                    if (diceResult.playerIndex == myServerIndex && scoreListPlayer1[i].GetScore() < 0)
                        scoreListPlayer1[i].SetToggleEnable(true);
                }
            }

            if (diceResult.playerIndex == myServerIndex)
                EnableAllUnlockDices(true);
        }
    }

    void RecvWriteScore(IPacket packet)
    {
        Debug.Log("RecvWriteScore");

        ToC_WriteScore writeScore = packet as ToC_WriteScore;

        if (writeScore == null) return;

        if (writeScore.playerIndex == 0)
            scoreListPlayer0[writeScore.jocboIndex].SetScore(writeScore.jocboScore);
        else if (writeScore.playerIndex == 1)
            scoreListPlayer1[writeScore.jocboIndex].SetScore(writeScore.jocboScore);

        UpdateScoreBoard(writeScore.playerIndex);
    }


    // Game UI
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
        foreach (var score in scoreListPlayer0)
            score.SetToggleEnable(false);
        foreach (var score in scoreListPlayer1)
            score.SetToggleEnable(false);
    }

    public void UpdateScoreBoard(int player)
    {
        int subTotal = 0;
        int total = 0;

        if (player == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                if (scoreListPlayer0[i].GetScore() > 0) 
                    subTotal += scoreListPlayer0[i].GetScore();
            }
                
            for (int i = 0; i < 12; i++)
            {
                if (scoreListPlayer0[i].GetScore() > 0)
                    total += scoreListPlayer0[i].GetScore();
            }
                
            subTotalScore0.text = $"{subTotal} / 63";
            totalScore0.text = total.ToString();
        }
        else if (player == 1)
        {
            for (int i = 0; i < 6; i++)
            {
                if (scoreListPlayer1[i].GetScore() > 0)
                    subTotal += scoreListPlayer1[i].GetScore();
            }
            for (int i = 0; i < 12; i++)
            {
                if (scoreListPlayer1[i].GetScore() > 0)
                    total += scoreListPlayer1[i].GetScore();
            }
                
            subTotalScore1.text = $"{subTotal} / 63";
            totalScore1.text = total.ToString();
        }
    }

    void UnlockAllDiceLocks()
    {
        foreach (var diceLock in diceToggleList)
            diceLock.UnSelectToggle();
    }

    void InitAllDiceNumbers()
    {
        foreach (var dice in diceToggleList)
        {
            dice.SetDice(6);
        }
    }

    void InitAllPreviewScores()
    {
        foreach (var score in scoreListPlayer0)
            if (score.GetScore() < 0) score.InitScore();
        foreach (var score in scoreListPlayer1)
            if (score.GetScore() < 0) score.InitScore();
    }

    void EnableAllUnlockDices(bool enable)
    {
        foreach (var diceLock in diceToggleList)
            diceLock.EnableToggle(enable);
    }

    void ShowPlayerTurn(int index)
    {
        playerTurn0.SetActive(false);
        playerTurn1.SetActive(false);

        if (index == 0) playerTurn0.SetActive(true);
        else if (index == 1) playerTurn1.SetActive(true);
    }

    void CheckAllScoreButton()
    {
        bool hasOn = false;
        foreach (var item in scoreListPlayer0)
            if (item.IsOn()) hasOn = true;
        foreach (var item in scoreListPlayer1)
            if (item.IsOn()) hasOn = true;

        EnableRecordScoreButton(hasOn);
    }

    // Events

    public void OnClickLeaveRoom()
    {
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

        if (unlockedCount == 0) return;

        NetworkManager.Instance.Send(packet.Write());
    }

    public void OnClickRecordScore()
    {
        int selected = -1;

        if (myServerIndex == 0)
        {
            for (int i = 0; i < scoreListPlayer0.Count; i++)
            {
                if (scoreListPlayer0[i].IsOn()) selected = i;
            }
        }
        else if(myServerIndex == 1)
        {
            for (int i = 0; i < scoreListPlayer1.Count; i++)
            {
                if (scoreListPlayer1[i].IsOn()) selected = i;
            }
        }

        scoreGroup.SetAllTogglesOff();
        EnableDiceButton(false);
        EnableRecordScoreButton(false);
        DisableAllScoreButton();
        
        ToS_WriteScore packet = new ToS_WriteScore();
        packet.jocboIndex = selected;
        NetworkManager.Instance.Send(packet.Write());
    }
}
