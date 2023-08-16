using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public List<ScoreItem> scoreList;

    int selectedIndex = -1;

    public int SelectedIndex()
    {
        return selectedIndex;
    }

    public void SetListener(Action onClick)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            var tmp = i;
            scoreList[i].SetListener(() =>
            {
                OnClickScore(tmp);
                onClick();
            });
        }
    }

    public void SetScore(int scoreIndex, int score)
    {
        scoreList[scoreIndex].SetScore(score);
    }

    public void SelectScore(int scoreIndex)
    {
        selectedIndex = scoreIndex;
        scoreList[scoreIndex].SetToggleOn(true);
    }

    public void UnSelectAll()
    {
        selectedIndex = -1;
        foreach (var score in scoreList)
            score.SetToggleOn(false);
    }

    public void DisableAll()
    {
        foreach (var score in scoreList)
            score.SetToggleEnable(false);
    }

    public void SetEnableScore(int index)
    {
        scoreList[index].SetToggleEnable(true);
    }

    public void SetPreviewScore(int index, int score)
    {
        if (scoreList[index].GetScore() < 0)
            scoreList[index].SetPrivewScore(score);
    }

    public void InitAllPreviewScore()
    {
        foreach (var score in scoreList)
        {
            if (score.GetScore() < 0) score.InitScore();
        }
    }

    public int GetScore(int index)
    {
        return scoreList[index].GetScore();
    }

    public int GetSubTotalScore()
    {
        int sum = 0;
        for (int i = 0; i < 6; i++)
        {
            if (scoreList[i].GetScore() > 0)
                sum += scoreList[i].GetScore();
        }
            
        return sum;
    }

    public int GetTotalScore()
    {
        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            if (scoreList[i].GetScore() > 0)
                sum += scoreList[i].GetScore();
        }
            
        return sum;
    }

    void OnClickScore(int index)
    {
        if (scoreList[index].IsOn())
        {
            selectedIndex = -1;
            scoreList[index].SetToggleOn(false);
        }
        else
        {
            for (int i = 0; i < scoreList.Count; i++)
                scoreList[i].SetToggleOn(false);
            scoreList[index].SetToggleOn(true);
            selectedIndex = index;
        }
    }
}


