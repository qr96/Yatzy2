using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class DiceViewer : MonoBehaviour
{
    public List<GameObject> diceList;
    public Animator animator;
    public GameObject dices;

    public void SetDice(int diceIndex, int num)
    {
        switch(num)
        {
            case 1:
                diceList[diceIndex].transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                break;
            case 2:
                diceList[diceIndex].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 3:
                diceList[diceIndex].transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
                break;
            case 4:
                diceList[diceIndex].transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case 5:
                diceList[diceIndex].transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                break;
            case 6:
                diceList[diceIndex].transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                break;
        }
    }

    public void PlayRollDice()
    {
        dices.SetActive(true);
        animator.Rebind();
        animator.Play("RollDice");
    }
}
