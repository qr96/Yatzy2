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

    public void DoMoveSceneEvent(int eventId)
    {
        if (eventId == 0)
        {
            if (DataCacheManager.Instance.lastDevilCastleResult == GameResult.Defeat)
                ErrorManager.Instance.Conversation("마몬", new List<string> {
                    "오잉? 자네 보기보다 별거 없구만~",
                    "다음엔 더 재밌는 모습 보여주길 기대하겠네."});
            else if (DataCacheManager.Instance.lastDevilCastleResult == GameResult.Win)
                ErrorManager.Instance.Conversation("마몬", new List<string> {
                    "오~! 자네 꽤 하는구만!",
                    "도전을 이어가도 되고, 보상을 받고 그만둬도 된다네",
                    "자네의 한계가 궁금하지 않은가?"});
            else
                ErrorManager.Instance.Conversation("마몬", new List<string> {
                    "재밌는 경기였네!",
                    "아쉽게도 무승부는 연승으로 치지도 않네",
                    "대신, 보상도 초기화되지 않아."});
        }
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
        SceneManager.Instance.MoveScene(3);
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
        if (PlayerPrefs.GetInt("FirstDevilCastle", 0) == 0)
        {
            ErrorManager.Instance.Conversation("마몬", new List<string>() {
                "안녕하신가~\n악마성에 온걸 환영하네~",
                "나는 마몬이라고 하네",
                "악마성은 처음인거 같으니 간단하게 설명 해주겠네",
                "1000골드를 내면 내게 도전을 할 수 있어",
                "내게 이기면 지불한 골드의 두배로 돌려주겠네",
                "물론, 이거뿐이라면 자네는 흥미를 느끼지 못할것 같구만",
                "자네가 내게 이길때마다 보상을 두배로 늘려주겠네",
                "내게 한 번 이긴다면 2000골드, 두 번 이긴다면 4000골드, 세 번 이기면 8000골드일세",
                "열 번만 승리한다면 무려 1024000 골드지",
                "하지만 명심하게. 한 번이라도 패배한다면 자네에게 줄 골드는 없어",
                "행운을 빌겠네."},
                () =>
                {
                    PlayerPrefs.SetInt("FirstDevilCastle", 1);
                    ErrorManager.Instance.ShowQuestionPopup("안내", "악마성 컨텐츠를 시작하면 1,000골드가 소모됩니다.\n시작하시겠습니까?", () =>
                    {
                        ReqOpenDevilCastle();
                    });
                });
        }
        else
        {
            ErrorManager.Instance.ShowQuestionPopup("안내", "악마성 컨텐츠를 시작하면 1,000골드가 소모됩니다.\n시작하시겠습니까?", () =>
            {
                ReqOpenDevilCastle();
            });
        }
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
