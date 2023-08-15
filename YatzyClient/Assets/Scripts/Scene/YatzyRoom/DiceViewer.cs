using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class DiceViewer : MonoBehaviour
{
    public List<GameObject> diceList;
    public List<GameObject> lockedDiceList;
    public Animator animator;
    public GameObject dices;

    Action onEndRoll;
    Coroutine rollWaitCo;

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

    public void LockDice(int diceIndex)
    {
        diceList[diceIndex].SetActive(false);
        lockedDiceList[diceIndex].SetActive(true);
    }

    public void UnLockDice(int diceIndex)
    {
        diceList[diceIndex].SetActive(true);
        lockedDiceList[diceIndex].SetActive(false);
    }

    public void UnLockAllDice()
    {
        foreach (var dice in diceList)
            dice.SetActive(true);

        foreach (var dice in lockedDiceList)
            dice.SetActive(false);
    }

    public void PlayRollDice(Action onAniEnd)
    {
        this.onEndRoll = onAniEnd;
        dices.SetActive(true);
        animator.Rebind();
        animator.Play("RollDice");

        if (rollWaitCo != null) StopCoroutine(rollWaitCo);
        rollWaitCo = StartCoroutine(WaitForRollDice());
    }

    IEnumerator WaitForRollDice()
    {
        yield return new WaitForSeconds(0.4f);
        if (onEndRoll != null) onEndRoll();
    }
}

