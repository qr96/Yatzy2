using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    public bool _isDev = false;
    public bool _connected = false;

    ServerSession _session = new ServerSession();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
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
        IPAddress ipAddr;
        if (_isDev)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            ipAddr = ipHost.AddressList[0];
        }
        else
        {
            ipAddr = IPAddress.Parse("180.210.82.243");
        }
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
        /* for aws
        string host = Dns.GetHostName();
        //string host = "ec2-3-34-253-239.ap-northeast-2.compute.amazonaws.com";
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        */

        Connector connector = new Connector();
        _connected = false;
        connector.Connect(endPoint, () => { return _session; }, 1);
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    public void OnDisconnected()
    {
        _connected = false;
        ErrorManager.Instance.ShowLoadingIndicator();
        ErrorManager.Instance.ShowPopup("안내", "서버와 연결이 끊어졌습니다", () =>
        {
            SceneManager.Instance.MoveScene(0);
        });
    }
}
