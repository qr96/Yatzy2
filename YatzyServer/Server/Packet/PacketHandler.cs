using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void ToS_ReqLoginHandler(PacketSession session, IPacket packet)
    {
        Console.WriteLine("ToS_ReqLoginHandler");
        ClientSession clientSession = session as ClientSession;
        ToS_ReqLogin loginPacket = packet as ToS_ReqLogin;

        if (clientSession.Lobby == null)
            return;

        if (loginPacket != null)
            clientSession.SetInfo(loginPacket.nickName);

        GameRoom lobby = clientSession.Lobby;
        lobby.Push(() => lobby.UniCast(clientSession, new ToC_ResLogin() { loginSuccess = true }));
    }

    public static void ToS_ReqRoomListHandler(PacketSession session, IPacket packet)
    {
        Console.WriteLine("ToS_ReqRoomListHandler");
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Lobby == null)
            return;

        ToC_ResRoomList roomListPacket = new ToC_ResRoomList();
        List<YatzyGameRoom> roomList = GameRoomManager.Instance.GetRoomList();

        foreach (var roomInfo in roomList)
            roomListPacket.roomInfos.Add(new ToC_ResRoomList.RoomInfo() { roomId = roomInfo.roomID, roomName = roomInfo.roomName });

        GameRoom gameRoom = clientSession.Lobby;
        gameRoom.Push(() => gameRoom.UniCast(clientSession, roomListPacket));
    }

    public static void ToS_ReqMakeRoomHandler(PacketSession session, IPacket packet)
    {
        Console.WriteLine("ToS_ReqRoomListHandler");
        ClientSession clientSession = session as ClientSession;
        ToS_ReqMakeRoom newRoom = packet as ToS_ReqMakeRoom;

        if (clientSession.Lobby == null)
            return;

        YatzyGameRoom gameRoom = GameRoomManager.Instance.MakeRoom(clientSession, newRoom.roomName);
        GameRoom lobby = clientSession.Lobby;

        if (gameRoom != null)
            lobby.Push(() => clientSession.Lobby.UniCast(clientSession, new ToC_ResEnterRoom() { success = true }));
        else
            lobby.Push(() => clientSession.Lobby.UniCast(clientSession, new ToC_ResEnterRoom() { success = false }));
    }

    public static void ToS_ReqEnterRoomHandler(PacketSession session, IPacket ipacket)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_ReqEnterRoom packet = ipacket as ToS_ReqEnterRoom;

        if (clientSession.Lobby == null)
            return;

        YatzyGameRoom room = GameRoomManager.Instance.EnterRoom(clientSession, packet.roomId);
        GameRoom lobby = clientSession.Lobby;

        if (room != null)
            lobby.Push(() => clientSession.Lobby.UniCast(clientSession, new ToC_ResEnterRoom() { success = true }));
        else
            lobby.Push(() => clientSession.Lobby.UniCast(clientSession, new ToC_ResEnterRoom() { success = false }));
    }

    public static void ToS_ReqLeaveRoomHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Lobby == null)
            return;

        YatzyGameRoom room = clientSession.GameRoom;
        room.Push(() => room.Leave(clientSession));
    }


    // ROOM

    public static void ToS_ReqRoomInfoHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.GameRoom == null)
            return;

        YatzyGameRoom gameRoom = clientSession.GameRoom;
        gameRoom.Push(() => {
            var userList = gameRoom.GetUserInfos();
            var userInfoList = new List<ToC_ResRoomInfo.UserInfo>();
            foreach (var user in userList)
                userInfoList.Add(new ToC_ResRoomInfo.UserInfo() { userName = user });

            gameRoom.UniCast(clientSession, new ToC_ResRoomInfo()
            {
                myServerIndex = gameRoom.GetUserIndex(clientSession),
                roomName = gameRoom.roomName,
                userInfos = userInfoList
            });
        });
        gameRoom.Push(() =>
        {
            gameRoom.BroadCast(new ToC_PlayerEnterRoom()
            {
                playerNickName = clientSession.nickName,
                playerIndex = gameRoom.GetUserIndex(clientSession),
            });
        });
    }

    public static void ToS_ReadyToStartHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_ReadyToStart ready = packet as ToS_ReadyToStart;

        if (clientSession.GameRoom == null)
            return;

        YatzyGameRoom gameRoom = clientSession.GameRoom;

        gameRoom.Push(() => { gameRoom.ReadyUser(clientSession); });
    }

    public static void ToS_RollDiceHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_RollDice rollDice = packet as ToS_RollDice;

        if (clientSession.GameRoom == null)
            return;

        YatzyGameRoom gameRoom = clientSession.GameRoom;
        List<int> fixDices = new List<int>();
        foreach (var fix in rollDice.fixDices)
            fixDices.Add(fix.diceIndex);

        gameRoom.Push(() => { gameRoom.RollDice(clientSession, fixDices); });
    }

    public static void ToS_WriteScoreHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_WriteScore writeScore = packet as ToS_WriteScore;

        if (clientSession.GameRoom == null || writeScore == null)
            return;

        YatzyGameRoom gameRoom = clientSession.GameRoom;

        gameRoom.Push(() => { gameRoom.WriteScore(clientSession, writeScore.jocboIndex); });
    }
}
