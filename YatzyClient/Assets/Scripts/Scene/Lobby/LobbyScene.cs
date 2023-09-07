using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    public RoomListTab roomListTab;
    public SelectGameTab selectGameTab;
    public NavigationBar navigationBar;

    public GameObject mainTab;
    public GameObject profileTab;

    public RankingPopup rankingPopup;

    float degree;

    void Start()
    {
        navigationBar.SetHomeButtonListener(() =>
        {
            HideAllTab();
            mainTab.SetActive(true);
        });
        navigationBar.SetProfileButtonListener(() =>
        {
            HideAllTab();
            profileTab.SetActive(true);
        });

        ErrorManager.Instance.HideLoadingIndicator();
        DoMoveSceneEvent(SceneManager.Instance.GetEventId());
    }

    void Update()
    {
        degree += Time.deltaTime / 2;
        if (degree >= 360)
            degree = 0;

        RenderSettings.skybox.SetFloat("_Rotation", degree);
    }

    void DoMoveSceneEvent(int eventId)
    {
        if (eventId < 0) return;

        if (eventId == 0)
        {
            ShowDevilCastleItem();
        }
    }

    void ShowDevilCastleItem()
    {
        selectGameTab.gameObject.SetActive(true);
        selectGameTab.DoMoveSceneEvent(0);
    }

    void HideAllTab()
    {
        mainTab.SetActive(false);
        profileTab.SetActive(false);
    }

    public void OnClickNormalGame()
    {
        roomListTab.Show();
    }

    public void OnClickSingleGame()
    {
        selectGameTab.gameObject.SetActive(true);
    }

    public void OnClickRanking()
    {
        rankingPopup.Show();
    }
}
