using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RankingPopup : MonoBehaviour
{
    public GameObject dim;
    public GameObject itemPrefab;
    public GameObject content;

    List<GameObject> itemPool = new List<GameObject>();

    void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_RecDevilCastleRanking, RecvDevilCastleRanking);

        ReqDevilCastleRanking();
    }

    void OnDestroy()
    {
        PacketHandler.RemoveAction(PacketID.ToC_RecDevilCastleRanking);
    }

    void ReqDevilCastleRanking()
    {
        Debug.Log("ToS_ReqDevilCastleRanking");
        ToS_ReqDevilCastleRanking req = new ToS_ReqDevilCastleRanking();
        NetworkManager.Instance.Send(req.Write());
    }

    void RecvDevilCastleRanking(IPacket packet)
    {
        ToC_RecDevilCastleRanking res = packet as ToC_RecDevilCastleRanking;
        HideAllList();
        SetItems(res.rankings);
    }


    // UI
    void HideAllList()
    {
        foreach (var item in itemPool)
        {
            item.SetActive(false);
        }
    }

    void SetItems(List<ToC_RecDevilCastleRanking.Ranking> ranking)
    {
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, ranking.Count * 100);

        for (int i = 0; i < ranking.Count; i++)
        {
            if (i >= itemPool.Count)
            {
                var prefab = Instantiate(itemPrefab, content.transform);
                itemPool.Add(prefab);
            }
            itemPool[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = $"{i}.";
            itemPool[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = ranking[i].userName;
            itemPool[i].GetComponentsInChildren<TextMeshProUGUI>()[2].text = $"최대 {ranking[i].maxLevel}연승";
            itemPool[i].SetActive(true);
        }
    }

    // Event
    public void Show()
    {
        dim.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        dim.SetActive(false);
        gameObject.SetActive(false);
    }
}
