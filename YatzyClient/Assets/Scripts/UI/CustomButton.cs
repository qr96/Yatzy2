using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
            gameObject.transform.localScale = new Vector2(0.9f, 0.9f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.transform.localScale = Vector2.one;
    }
}
