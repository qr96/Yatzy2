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
        "1�� ���� �ֻ��� ���� ����\n(�ִ� 5��)",
        "2�� ���� �ֻ��� ���� ����\n(�ִ� 10��)",
        "3�� ���� �ֻ��� ���� ����\n(�ִ� 15��)",
        "4�� ���� �ֻ��� ���� ����\n(�ִ� 20��)",
        "5�� ���� �ֻ��� ���� ����\n(�ִ� 25��)",
        "6�� ���� �ֻ��� ���� ����\n(�ִ� 30��)",
        "�ֻ��� �� 5���� ����\n(�ִ� 30��)",
        "������ �ֻ��� ���� 4�� �̻��� ��, �ֻ��� �� 5���� ����\n(�ִ� 30��)",
        "�ֻ����� 3��, 2���� ������ �� ������ ���� �ȿ��� �ֻ��� ���� ���� ������ ��, �ֻ��� �� 5���� ����\n(�ִ� 30��)",
        "�̾����� �ֻ��� ���� 4�� �̻��� ��\n(���� 15��)",
        "�̾����� �ֻ��� ���� 5���� ��\n(���� 30��)",
        "����~ ������ �ֻ��� ���� 5���� ��\n(���� 50��)"
    };

    public void OnClickTip(int index)
    {
        ErrorManager.Instance.ShowPopup(titleList[index], descList[index]);
    }
}
