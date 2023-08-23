using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceLabelDesc : MonoBehaviour
{
    List<string> titleList = new List<string>()
    {
        "Aces",
        "Deuces",
        "Threes",
        "Fours",
        "Fives",
        "Sixes",
        "Choice",
        "4 of a Kind",
        "Full House",
        "Small Straight",
        "Large Straigt",
        "Yacht",
    };

    List<string> descList = new List<string>()
    {
        "1이 나온 주사위 눈의 총합\n(최대 5점)",
        "2가 나온 주사위 눈의 총합\n(최대 10점)",
        "3이 나온 주사위 눈의 총합\n(최대 15점)",
        "4가 나온 주사위 눈의 총합\n(최대 20점)",
        "5가 나온 주사위 눈의 총합\n(최대 25점)",
        "6이 나온 주사위 눈의 총합\n(최대 30점)",
        "주사위 눈 5개의 총합\n(최대 30점)",
        "동일한 주사위 눈이 4개 이상일 때, 주사위 눈 5개의 총합\n(최대 30점)",
        "주사위를 3개, 2개로 묶었을 때 각각의 묶음 안에서 주사위 눈이 서로 동일할 때, 주사위 눈 5개의 총합\n(최대 30점)",
        "이어지는 주사위 눈이 4개 이상일 때\n(고정 15점)",
        "이어지는 주사위 눈이 5개일 때\n(고정 30점)",
        "야추~ 동일한 주사위 눈이 5개일 때\n(고정 50점)"
    };

    public void OnClickTip(int index)
    {
        ErrorManager.Instance.ShowPopup(titleList[index], descList[index]);
    }
}
