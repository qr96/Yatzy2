using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevilCastleItem : MonoBehaviour
{
    public TextMeshProUGUI desc;
    public Button openBtn;
    public Button challegeBtn;
    public Button getRewardBtn;

    private void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResDevilCastleInfo, RecvDevilCastleInfo);
        PacketHandler.AddAction(PacketID.ToC_ResOpenDevilCastle, RecvOpenDevilCastle);
        PacketHandler.AddAction(PacketID.ToC_ResEnterSingleRoom, RecvEnterSingleGame);

        ReqDevilCastleInfo();
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_ResDevilCastleInfo);
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

        SetDesc($"현재 단계 : {res.level}단계\n현재 상금 : {res.reward}");
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


    // UIs
    void SetDesc(string desc)
    {
        this.desc.text = desc;
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
        ErrorManager.Instance.ShowLoadingIndicator();
        SendEnterSingleGame();
    }
}
