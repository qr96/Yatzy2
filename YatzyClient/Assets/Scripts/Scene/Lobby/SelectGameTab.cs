using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectGameTab : MonoBehaviour
{
    public DevilCastleItem devilCastle;

    public void DoMoveSceneEvent(int eventId)
    {
        if (eventId == 0)
            devilCastle.DoMoveSceneEvent(eventId);
    }

    public void OnClickHide()
    {
        gameObject.SetActive(false);
    }

}
