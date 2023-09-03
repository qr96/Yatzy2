using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBar : MonoBehaviour
{
    public RectTransform selected;
    public Button shopButton;
    public Button profileButton;
    public Button homeButton;
    public Button communityButton;
    public Button settingButton;

    private void Start()
    {
        shopButton.onClick.AddListener(() =>
        {
            MoveSelected(-280f);
        });
        profileButton.onClick.AddListener(() =>
        {
            MoveSelected(-140f);
        });
        homeButton.onClick.AddListener(() =>
        {
            MoveSelected(0f);
        });
        communityButton.onClick.AddListener(() =>
        {
            MoveSelected(140f);
        });
        settingButton.onClick.AddListener(() =>
        {
            MoveSelected(280f);
        });
    }

    void MoveSelected(float posX)
    {
        selected.DOAnchorPos(new Vector2(posX, -15f), 0.2f);
    }

    public void SetShopButtonListener(Action onClick)
    {
        if (onClick != null)
            shopButton.onClick.AddListener(() => onClick());
    }

    public void SetProfileButtonListener(Action onClick)
    {
        if (onClick != null)
            profileButton.onClick.AddListener(() => onClick());
    }

    public void SetHomeButtonListener(Action onClick)
    {
        if (onClick != null)
            homeButton.onClick.AddListener(() => onClick());
    }
}
