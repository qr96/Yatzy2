using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConversationPopup : MonoBehaviour
{
    public TextMeshProUGUI name;
    public TextMeshProUGUI content;

    List<string> conversation;
    int conversationIndex;
    Action onEnd; // 대화 종료 이벤트

    public void ShowConversation(string talkerName, List<string> conversation, Action onEnd)
    {
        this.name.text = talkerName;
        this.conversation = conversation;
        this.onEnd = onEnd;
        if (conversation.Count <= 0) return;

        content.text = conversation[0];
        conversationIndex = 0;
        gameObject.SetActive(true);
    }

    public void OnClickNext()
    {
        NextConversation();
    }

    void NextConversation()
    {
        conversationIndex++;
        if (conversationIndex >= conversation.Count)
        {
            if (onEnd != null) onEnd();
            gameObject.SetActive(false);
        }
        else
        {
            content.text = conversation[conversationIndex];
        }
    }
}
