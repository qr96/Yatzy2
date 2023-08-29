using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopUserInfoUI : MonoBehaviour
{
    public TextMeshProUGUI nickName;
    public TextMeshProUGUI money;
    public TextMeshProUGUI ruby;

    void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResMyInfo, RecvUserInfo);

        ReqUserInfo();
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToS_ReqMyInfo);
    }

    public void ReqUserInfo()
    {
        ToS_ReqMyInfo req = new ToS_ReqMyInfo();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvUserInfo(IPacket packet)
    {
        ToC_ResMyInfo res = packet as ToC_ResMyInfo;
        if (res == null) return;

        nickName.text = res.nickName;
        money.text = res.money.ToString();
        ruby.text = res.ruby.ToString();
    }
}
