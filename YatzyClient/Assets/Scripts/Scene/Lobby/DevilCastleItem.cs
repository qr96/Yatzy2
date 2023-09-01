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
                ErrorManager.Instance.Conversation("����", new List<string> {
                    "����? �ڳ� ���⺸�� ���� ������~",
                    "������ �� ��մ� ��� �����ֱ� ����ϰڳ�."});
            else if (DataCacheManager.Instance.lastDevilCastleResult == GameResult.Win)
                ErrorManager.Instance.Conversation("����", new List<string> {
                    "��~! �ڳ� �� �ϴ±���!",
                    "������ �̾�� �ǰ�, ������ �ް� �׸��ֵ� �ȴٳ�",
                    "�ڳ��� �Ѱ谡 �ñ����� ������?"});
            else
                ErrorManager.Instance.Conversation("����", new List<string> {
                    "��մ� ��⿴��!",
                    "�ƽ��Ե� ���ºδ� �������� ġ���� �ʳ�",
                    "���, ���� �ʱ�ȭ���� �ʾ�."});
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
            ErrorManager.Instance.ShowPopup("�ȳ�", "���� �ޱ⿡ �����߽��ϴ�");
        }
    }


    // UIs
    void SetDesc(int level, long reward)
    {
        this.level = level;
        this.desc.text = $"���� �ܰ� : {level}�ܰ�\n���� ��� : {reward}";
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
            ErrorManager.Instance.Conversation("����", new List<string>() {
                "�ȳ��ϽŰ�~\n�Ǹ����� �°� ȯ���ϳ�~",
                "���� �����̶�� �ϳ�",
                "�Ǹ����� ó���ΰ� ������ �����ϰ� ���� ���ְڳ�",
                "1000��带 ���� ���� ������ �� �� �־�",
                "���� �̱�� ������ ����� �ι�� �����ְڳ�",
                "����, �̰Ż��̶�� �ڳ״� ��̸� ������ ���Ұ� ������",
                "�ڳװ� ���� �̱涧���� ������ �ι�� �÷��ְڳ�",
                "���� �� �� �̱�ٸ� 2000���, �� �� �̱�ٸ� 4000���, �� �� �̱�� 8000����ϼ�",
                "�� ���� �¸��Ѵٸ� ���� 1024000 �����",
                "������ ����ϰ�. �� ���̶� �й��Ѵٸ� �ڳ׿��� �� ���� ����",
                "����� ���ڳ�."},
                () =>
                {
                    PlayerPrefs.SetInt("FirstDevilCastle", 1);
                    ErrorManager.Instance.ShowQuestionPopup("�ȳ�", "�Ǹ��� �������� �����ϸ� 1,000��尡 �Ҹ�˴ϴ�.\n�����Ͻðڽ��ϱ�?", () =>
                    {
                        ReqOpenDevilCastle();
                    });
                });
        }
        else
        {
            ErrorManager.Instance.ShowQuestionPopup("�ȳ�", "�Ǹ��� �������� �����ϸ� 1,000��尡 �Ҹ�˴ϴ�.\n�����Ͻðڽ��ϱ�?", () =>
            {
                ReqOpenDevilCastle();
            });
        }
    }

    public void OnClickChallengeBtn()
    {
        string ment = "";
        if (level == 0) ment = "�Ǹ����� ù �ܰ迡 �����Ͻðڽ��ϱ�?";
        else if (level > 0) ment = "�Ǹ����� ��� �����Ͻðڽ��ϱ�?\n(�й��� ��� �Ǹ����� ������ ������ �ʱ�ȭ �˴ϴ�.)";
        ErrorManager.Instance.ShowQuestionPopup("�ȳ�", ment, () =>
        {
            ErrorManager.Instance.ShowLoadingIndicator();
            SendEnterSingleGame();
        });
    }

    public void OnClickGetReward()
    {
        ErrorManager.Instance.ShowQuestionPopup("�ȳ�", "������ �ް� �Ǹ����� ������ ��ġ�ðڽ��ϱ�?\n(�Ǹ����� ������ ������ �ʱ�ȭ �˴ϴ�.)", () =>
        {
            SendGetDevilCastleReward();
        });
    }
}
