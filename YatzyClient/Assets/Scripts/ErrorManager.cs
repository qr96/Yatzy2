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

    // 질문 팝업
    public GameObject questionPopup;
    public TextMeshProUGUI questionPopupTitle;
    public TextMeshProUGUI questionPopupDesc;
    public Button questionPopupOk;

    // 대화 팝업
    public ConversationPopup conversationPopup;

    Coroutine loadingIndicatorCo;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
#if UNITY_IOS || UNITY_ANDROID
        Application.targetFrameRate = 60;
#else
        Application.targetFrameRate = 120;
#endif
    }

    IEnumerator LoadingIndicatorCo()
    {
        float waitTime = 0f;
        int i = 3;
        while(true)
        {
            if (i == 3) loadingIndicatorLabel.text = "Loading...";
            else if (i == 2) loadingIndicatorLabel.text = "Loading..";
            else if (i == 1) loadingIndicatorLabel.text = "Loading.";
            yield return new WaitForSeconds(0.1f);
            i--;
            if (i <= 0) i = 3;

            waitTime += 0.1f;
            if (waitTime >= 15f)
            {
                Debug.Log("Error");
                ShowPopup("안내", "에러가 발생했습니다.\n다시 시도해주세요", () =>
                {
                    HidePopup();
                    HideLoadingIndicator();
                });
                yield break;
            }
                
        }
    }

    public void ShowLoadingIndicator()
    {
        loadingIndicator.SetActive(true);
        if (loadingIndicatorCo != null) StopCoroutine(loadingIndicatorCo);
        loadingIndicatorCo = StartCoroutine(LoadingIndicatorCo());
    }

    public void HideLoadingIndicator()
    {
        if (loadingIndicatorCo != null) StopCoroutine(loadingIndicatorCo);
        loadingIndicator.SetActive(false);
    }

    public void ShowPopup(string title, string desc, Action onClick = null)
    {
        errorPopupTitle.text = title;
        errorPopupDesc.text = desc;

        errorPopupOk.onClick.RemoveAllListeners();
        if (onClick != null)
            errorPopupOk.onClick.AddListener(() => onClick());
        errorPopupOk.onClick.AddListener(() => HidePopup());

        errorPopup.SetActive(true);
    }

    public void ShowQuestionPopup(string title, string desc, Action onClickOk)
    {
        questionPopupTitle.text = title;
        questionPopupDesc.text = desc;

        questionPopupOk.onClick.RemoveAllListeners();
        if (onClickOk != null)
            questionPopupOk.onClick.AddListener(() => onClickOk());
        questionPopupOk.onClick.AddListener(() => HideQuestionPopup());

        questionPopup.SetActive(true);
    }

    public void HidePopup()
    {
        errorPopup.SetActive(false);
    }

    public void HideQuestionPopup()
    {
        questionPopup.SetActive(false);
    }

    public void Conversation(string talkerName, List<string> conversation, Action onEnd)
    {
        conversationPopup.ShowConversation(talkerName, conversation, onEnd);
    }
}
