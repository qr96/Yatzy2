using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager Instance;

    public GameObject loadingIndicator;
    public TextMeshProUGUI loadingIndicatorLabel;

    public GameObject errorPopup;
    public TextMeshProUGUI errorPopupTitle;
    public TextMeshProUGUI errorPopupDesc;
    public Button errorPopupOk;

    Coroutine loadingIndicatorCo;

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator LoadingIndicatorCo()
    {
        int i = 3;
        while(true)
        {
            if (i == 3) loadingIndicatorLabel.text = "Loading...";
            else if (i == 2) loadingIndicatorLabel.text = "Loading..";
            else if (i == 1) loadingIndicatorLabel.text = "Loading.";
            yield return new WaitForSeconds(0.1f);
            i--;
            if (i <= 0) i = 3;
        }
    }

    public void ShowLoadingIndicator()
    {
        loadingIndicator.SetActive(true);
        loadingIndicatorCo = StartCoroutine(LoadingIndicatorCo());
    }

    public void HideLoadingIndicator()
    {
        if (loadingIndicatorCo != null) StopCoroutine(loadingIndicatorCo);
        loadingIndicator.SetActive(false);
    }

    public void ShowPopup(string title, string desc, Action onClick)
    {
        errorPopupTitle.text = title;
        errorPopupDesc.text = desc;

        if (onClick != null)
            errorPopupOk.onClick.AddListener(() => onClick());
        errorPopupOk.onClick.AddListener(() => HidePopup());

        errorPopup.SetActive(true);
    }

    public void HidePopup()
    {
        errorPopup.SetActive(false);
    }
}
