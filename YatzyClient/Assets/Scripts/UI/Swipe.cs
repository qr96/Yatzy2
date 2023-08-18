using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    public Color[] colors;
    public Scrollbar scrollbar;//, imageContent;
    float[] pos;

    private float dragStart;
    private float dragEnd;
    private float dragDis;
    private bool moving;

    public float sensitivity;

    float distance;

    public GameObject LeftBtn;
    public GameObject RightBtn;

    public List<Image> navigations;

    public int nowPage; // 현재 페이지

    public delegate void Func();
    public Func OnMoveEnd; // 페이지 움직임 종료되었을때 호출

    private void Start()
    {
        InitSwipe();
    }

    void Update()
    {
        if (pos.Length < 1) return;

        if (moving == true)
        {
            // 페이지 움직임 종료
            if (scrollbar.value < pos[nowPage] + (distance / 2) && scrollbar.value > pos[nowPage] - (distance / 2))
            {
                moving = false;
                SetNavigation(nowPage);
                if (OnMoveEnd != null) OnMoveEnd();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            dragStart = scrollbar.value;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragEnd = scrollbar.value;
            dragDis = dragEnd - dragStart;

            if (Mathf.Abs(dragDis) > sensitivity)
            {
                moving = true;
                if (dragDis > 0)
                {
                    if (this.nowPage < pos.Length - 1)
                    {
                        nowPage++;
                    }
                }
                else if (dragDis < 0)
                {
                    if (this.nowPage > 0)
                    {
                        nowPage--;
                    }
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            //scroll_pos = scrollbar.value;
        }
        else
        {
            if (nowPage < pos.Length)
                scrollbar.value = Mathf.Lerp(scrollbar.value, pos[nowPage], 0.1f);
        }
    }

    public void InitSwipe()
    {
        pos = new float[transform.childCount];
        distance = 1f / (pos.Length - 1f);
        nowPage = 0;

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        sensitivity = 0.1f;

        SetNavigation(0);
    }

    // 네비게이션 버튼 처리
    public void SetNavigation(int page)
    {
        if (navigations != null && navigations.Count > 0)
        {
            for (int i = 0; i < navigations.Count; i++)
            {
                if (i == page)
                    navigations[i].color = Color.white;
                else
                    navigations[i].color = new Color(180 / 255f, 180 / 255f, 180 / 255f, 180 / 255f);
            }
        }
    }

    public void WhichBtnClicked(int nowPage)
    {
        this.nowPage = nowPage;
        moving = true;
    }
}
