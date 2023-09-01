using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCacheManager : MonoBehaviour
{
    public static DataCacheManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public long myMoney = 0;
    public long myRuby = 0;

    public GameResult lastDevilCastleResult = GameResult.None;
}
