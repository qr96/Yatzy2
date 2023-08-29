using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevilCastleItem : MonoBehaviour
{
    public TopUserInfoUI topUI;
    public TextMeshProUGUI desc;
    public Button openBtn;
    public Button challegeBtn;
    public Button getRewardBtn;

    int level = 0;

    private void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResDevilCastleInfo, RecvDevilCastleInfo);
        PacketHandler.AddAction(PacketID.ToC_ResOpenDevilCastle, RecvOpenDevilCastle);
        PacketHandler.AddAction(PacketID.ToC_ResEnterSingleRoom, RecvEnterSingleGame);
        PacketHandler.AddAction(PacketID.ToC_ResGetDevilCastleReward, RecvGetDevilCastleReward);

        ReqDevilCastleInfo();
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_ResDevilCastleInfo);
        PacketHandler.RemoveAction(PacketID.ToC_ResOpenDevilCastle);
        PacketHandler.RemoveAction(PacketID.ToC_ResEnterSingleRoom);
        PacketHandler.RemoveAction(PacketID.ToC_ResGetDevilCastleReward);
    }

    // Packets
    void ReqDevilCastleInfo()
    {
        ToS_ReqDevilCastleInfo req = new ToS_ReqDevilCastleInfo();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvDevilCastleInfo(IPacket packet)
    {
        ToC_ResDevilCastleInfo res = packet as ToC_ResDevilCastleInfo;
        if (res == null) return;

        SetDesc(res.level, res.reward);
        SetButtons(res.isOpened);
        EnableGetRewardBtn(res.level > 0);
        ErrorManager.Instance.HideLoadingIndicator();
    }

    void ReqOpenDevilCastle()
    {
        ToS_ReqOpenDevilCastle req = new ToS_ReqOpenDevilCastle();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvOpenDevilCastle(IPacket packet)
    {
        ToC_ResOpenDevilCastle res = packet as ToC_ResOpenDevilCastle;
        if (res == null) return;

        if (res.success)
        {
            SetButtons(true);
        }
    }

    void SendEnterSingleGame()
    {
        ToS_ReqEnterSingleRoom packet = new ToS_ReqEnterSingleRoom();
        NetworkManager.Instance.Send(packet.Write());
    }

    void RecvEnterSingleGame(IPacket packet)
    {
        ToC_ResEnterSingleRoom res = packet as ToC_ResEnterSingleRoom;
        SceneManager.LoadScene(3);
    }

    void SendGetDevilCastleReward()
    {
        ToS_ReqGetDevilCastleReward packet = new ToS_ReqGetDevilCastleReward();
        NetworkManager.Instance.Send(packet.Write());
    }

    void RecvGetDevilCastleReward(IPacket packet)
    {
        ToC_ResGetDevilCastleReward res = packet as ToC_ResGetDevilCastleReward;
        if (res.success)
        {
            topUI.ReqUserInfo();
            SetDesc(0, 0);
            SetButtons(false);
        }
        else
        {
            ErrorManager.Instance.ShowPopup("안내", "보상 받기에 실패했습니다");
        }
    }


    // UIs
    void SetDesc(int level, long reward)
    {
        this.level = level;
        this.desc.text = $"현재 단계 : {level}단계\n현재 상금 : {reward}";
    }

    void SetButtons(bool isOpened)
    {
        openBtn.gameObject.SetActive(!isOpened);
        challegeBtn.gameObject.SetActive(isOpened);
        getRewardBtn.gameObject.SetActive(isOpened);
    }

    void EnableGetRewardBtn(bool enable)
    {
        getRewardBtn.enabled = enable;
    }



    // Events
    public void OnClickOpenBtn()
    {
        ErrorManager.Instance.ShowQuestionPopup("안내", "악마성 컨텐츠를 시작하면 1,000골드가 소모됩니다.\n시작하시겠습니까?", ()=>
        {
            ReqOpenDevilCastle();
        });
    }

    public void OnClickChallengeBtn()
    {
        string ment = "";
        if (level == 0) ment = "악마성의 첫 단계에 도전하시겠습니까?";
        else if (level > 0) ment = "악마성에 계속 도전하시겠습니까?\n(패배할 경우 악마성이 닫히고 보상이 초기화 됩니다.)";
        ErrorManager.Instance.ShowQuestionPopup("안내", ment, () =>
        {
            ErrorManager.Instance.ShowLoadingIndicator();
            SendEnterSingleGame();
        });
    }

    public void OnClickGetReward()
    {
        ErrorManager.Instance.ShowQuestionPopup("안내", "보상을 받고 악마성을 도전을 마치시겠습니까?\n(악마성이 닫히고 연승이 초기화 됩니다.)", () =>
        {
            SendGetDevilCastleReward();
        });
    }
}
