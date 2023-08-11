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

    public List<TextMeshProUGUI> diceList;
    public Button rollDice;

    int myServerIndex = -1;

    void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResRoomInfo, RecvRoomInfo);
        PacketHandler.AddAction(PacketID.ToC_PlayerTurn, RecvPlayerTurn);
        PacketHandler.AddAction(PacketID.ToC_DiceResult, RecvDiceResult);
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
            EnableDiceButton(myServerIndex == p.playerTurn);
        }
    }

    void RecvDiceResult(IPacket packet)
    {
        Debug.Log("RecvDiceResult");

        ToC_DiceResult diceResult = packet as ToC_DiceResult;

        if(diceResult != null)
        {
            for (int i = 0; i < diceResult.diceResults.Count; i++)
            {
                diceList[i].text = diceResult.diceResults[i].dice.ToString();
            }
        }
    }


    // Game UI
    void EnableDiceButton(bool enable)
    {
        rollDice.interactable = enable;
    }

    void ShowPlayerTurn(int index)
    {
        playerTurn0.SetActive(false);
        playerTurn1.SetActive(false);

        if (index == 0) playerTurn0.SetActive(true);
        else if (index == 1) playerTurn1.SetActive(true);
    }

    void SetDiceResult(List<int> dices)
    {

    }


    // Events

    public void OnClickRollDice()
    {
        ToS_RollDice packet = new ToS_RollDice();

        NetworkManager.Instance.Send(packet.Write());
    }
}
