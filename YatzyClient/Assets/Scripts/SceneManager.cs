using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    int moveSceneEvent;

    private void Awake()
    {
        Instance = this;
    }

    public void MoveScene(int scene, int moveSceneEvent = -1)
    {
        this.moveSceneEvent = moveSceneEvent;

        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public int GetEventId()
    {
        return moveSceneEvent;
    }
}
