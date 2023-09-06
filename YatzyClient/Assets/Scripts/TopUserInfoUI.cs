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

        Debug.Log(this.gameObject.name);
        ReqUserInfo();
    }

    private void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_ResMyInfo);
        Debug.Log("Destroyed");
    }

    public void ReqUserInfo()
    {
        Debug.Log("ReqUserInfo");
        ToS_ReqMyInfo req = new ToS_ReqMyInfo();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvUserInfo(IPacket packet)
    {
        Debug.Log("RecvUserInfo");
        ToC_ResMyInfo res = packet as ToC_ResMyInfo;
        if (res == null) return;

        DataCacheManager.Instance.myMoney = res.money;
        DataCacheManager.Instance.myRuby = res.ruby;
        //nickName.text = res.nickName;
        money.text = res.money.ToString();
        ruby.text = res.ruby.ToString();
    }
}
