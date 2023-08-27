using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectGameTab : MonoBehaviour
{
    private void Start()
    {
        PacketHandler.AddAction(PacketID.ToC_ResEnterSingleRoom, RecvEnterSingleGame);
    }

    void SendEnterSingleGame()
    {
        ToS_ReqEnterSingleRoom packet = new ToS_ReqEnterSingleRoom();
        NetworkManager.Instance.Send(packet.Write());
    }

    void RecvEnterSingleGame(IPacket packet)
    {
        ToC_ResEnterSingleRoom res = packet as ToC_ResEnterSingleRoom;
        SceneManager.LoadScene(3);
    }

    public void OnClickDevilCastle()
    {
        ErrorManager.Instance.ShowLoadingIndicator();
        SendEnterSingleGame();
    }
}
