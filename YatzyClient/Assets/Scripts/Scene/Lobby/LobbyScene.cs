using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    public RoomListTab roomListTab;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI rubyText;

    float degree;

    void Start()
    {
        ErrorManager.Instance.HideLoadingIndicator();

        // Request user Info
        // name, money, ruby
        moneyText.text = "0";
        rubyText.text = "0";
    }

    private void Update()
    {
        degree += Time.deltaTime / 2;
        if (degree >= 360)
            degree = 0;

        RenderSettings.skybox.SetFloat("_Rotation", degree);
    }

    public void OnClickNormalGame()
    {
        roomListTab.Show();
    }
}
