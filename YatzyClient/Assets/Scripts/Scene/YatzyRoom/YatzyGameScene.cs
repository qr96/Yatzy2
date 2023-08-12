using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YatzyGameScene : MonoBehaviour
{
    public TextMeshProUGUI player0;
    public TextMeshProUGUI player1;

    public GameObject playerTurn0;
    public GameObject playerTurn1;

    public ToggleGroup scoreGroup;
    public List<ScoreItem> scoreListPlayer0;
    public List<ScoreItem> scoreListPlayer1;

    public List<DiceToggle> diceList;
    public Button rollDice;
    public Button recordScore;

    int myServerIndex = -1;

    void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResRoomInfo, RecvRoomInfo);
        PacketHandler.AddAction(PacketID.ToC_PlayerTurn, RecvPlayerTurn);
        PacketHandler.AddAction(PacketID.ToC_DiceResult, RecvDiceResult);
        PacketHandler.AddAction(PacketID.ToC_WriteScore, RecvWriteScore);

        foreach (var item in scoreListPlayer0)
            item.SetListener(() => CheckAllScoreButton());
        foreach (var item in scoreListPlayer1)
            item.SetListener(() => CheckAllScoreButton());

        EnableDiceButton(false);
        EnableRecordScoreButton(false);
        //DisableAllScoreButton();

        ReqRoomInfo();
    }

    // New User Enter

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

        ToC_ResRoomInfo roomInfo = packet as ToC_ResRoomInfo;
        if (roomInfo != null)
        {
            myServerIndex = roomInfo.myServerIndex;

            if (roomInfo.userInfos.Count > 0)
                player0.text = roomInfo.userInfos[0].userName;
            else if (roomInfo.userInfos.Count > 1)
                player0.text = roomInfo.userInfos[1].userName;
        }

        ReqReadyToStart();
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
            EnableAllUnlockDices(false);
            EnableDiceButton(myServerIndex == p.playerTurn);
        }
    }

    void RecvDiceResult(IPacket packet)
    {
        Debug.Log("RecvDiceResult");

        ToC_DiceResult diceResult = packet as ToC_DiceResult;
        if(diceResult != null)
        {
            List<int> dices = new List<int>();

            for (int i = 0; i < diceResult.diceResults.Count; i++)
            {
                dices.Add(diceResult.diceResults[i].dice);
                diceList[i].SetDice(diceResult.diceResults[i].dice);
            }

            if (diceResult.playerIndex == myServerIndex)
                EnableScoreButton(diceResult.playerIndex);
            else
                DisableAllScoreButton();
        }

        EnableAllUnlockDices(true);
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

    void EnableScoreButton(int index)
    {
        DisableAllScoreButton();
        if (index == 0)
        {
            foreach (var score in scoreListPlayer0)
                score.SetToggleEnable(true);
        }
        else if(index == 2)
        {
            foreach (var score in scoreListPlayer1)
                score.SetToggleEnable(true);
        }
            
    }

    void DisableAllScoreButton()
    {
        foreach (var score in scoreListPlayer0)
            score.SetToggleEnable(false);
        foreach (var score in scoreListPlayer1)
            score.SetToggleEnable(false);
    }

    void UnlockAllDiceLocks()
    {
        foreach (var diceLock in diceList)
            diceLock.UnSelectToggle();
    }

    void EnableAllUnlockDices(bool enable)
    {
        foreach (var diceLock in diceList)
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

    public void OnClickRollDice()
    {
        int unlockedCount = 0;

        ToS_RollDice packet = new ToS_RollDice();
        for (int i = 0; i < 5; i++)
        {
            if (diceList[i].IsLocked()) packet.fixDices.Add(new ToS_RollDice.FixDice() { diceIndex = i });
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
