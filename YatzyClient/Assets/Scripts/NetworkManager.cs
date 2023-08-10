using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    ServerSession _session = new ServerSession();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ConnectToServer();

        //StartCoroutine(SendCo());
    }

    private void Update()
    {
        IPacket packet = PacketQueue.Instance.Pop();
        if (packet != null)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

    public void ConnectToServer()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return _session; }, 1);
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    IEnumerator SendCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            ToS_ReqRoomList req = new ToS_ReqRoomList();
            req.authToken = "1234";
            ArraySegment<byte> seg = req.Write();
            _session.Send(seg);

            /*
            C_Chat chat = new C_Chat();
            chat.chat = "Hello Unity";
            ArraySegment<byte> seg = chat.Write();

            _session.Send(seg);
            */
        }
    }
}
