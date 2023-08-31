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

    public GameResult lastDevilCastleResult = GameResult.None;
}
