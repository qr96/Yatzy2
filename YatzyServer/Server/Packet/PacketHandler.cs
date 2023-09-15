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

        if (loginPacket == null) return;

        int errorCode = DataManager.Instance.AddNewUser(new UserInfo() { nickName = loginPacket.nickName, money = 10000, ruby = 0 });

        if (errorCode == 0 || errorCode == 3)
            errorCode = DataManager.Instance.LoginUser(loginPacket.nickName);
        else errorCode = 2;

        clientSession.SetInfo(loginPacket.nickName);

        GameRoom lobby = clientSession.Lobby;
        lobby.Push(() => lobby.UniCast(clientSession, new ToC_ResLogin() { errorCode = errorCode }));
    }

    public static void ToS_ReqMyInfoHandler(PacketSession session, IPacket packet)
    {
        Console.Write("ToS_ReqMyInfoHandler");

        ClientSession clientSession = session as ClientSession;
        ToS_ReqMyInfo myInfo = packet as ToS_ReqMyInfo;

        if (clientSession.Lobby == null)
            return;

        var userInfo = DataManager.Instance.GetUserInfo(clientSession.nickName);
        if (userInfo == null) return;

        GameRoom lobby = clientSession.Lobby;
        lobby.Push(() => lobby.UniCast(clientSession, new ToC_ResMyInfo() { nickName = userInfo.nickName, money = userInfo.money, ruby = userInfo.ruby }));
    }

    public static void ToS_ReqRoomListHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Lobby == null)
            return;

        ToC_ResRoomList roomListPacket = new ToC_ResRoomList();
        List<YatzyGameRoom> roomList = GameRoomManager.Instance.GetRoomList();

        foreach (var roomInfo in roomList)
            roomListPacket.roomInfos.Add(new ToC_ResRoomList.RoomInfo() { 
                roomId = roomInfo.roomID, roomName = roomInfo.roomName, playerCount = roomInfo.GetUserCount() 
            });

        GameRoom gameRoom = clientSession.Lobby;
        gameRoom.Push(() => gameRoom.UniCast(clientSession, roomListPacket));
    }

    public static void ToS_ReqMakeRoomHandler(PacketSession session, IPacket packet)
    {
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

        GameRoomManager.Instance.EnterRoom(clientSession, packet.roomId);
        GameRoom lobby = clientSession.Lobby;
        YatzyGameRoom room = clientSession.GameRoom;

        if (lobby == null) return;

        if (room != null && room.GetUserCount() > 0)
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

        if (room == null)
            return;

        room.Push(() => room.Leave(clientSession));
    }

    public static void ToS_ReqEnterSingleRoomHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Lobby == null) return;

        GameRoom lobby = clientSession.Lobby;
        clientSession.YatzySingleGame = new YatzySingleGame(clientSession, (result) =>
        {
            if (result == 0) DataManager.Instance.DevilCastleLevelUp(clientSession.nickName);
            else if (result == 1) DataManager.Instance.DevilCastleLevelReset(clientSession.nickName);
        });
        lobby.Push(() => lobby.UniCast(clientSession, new ToC_ResEnterSingleRoom()));
    }

    public static void ToS_ReqLeaveSingleRoomHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        YatzySingleGame singleGame = clientSession.YatzySingleGame;
        if (singleGame == null) return;

        if (singleGame.gameEnd == false)
            DataManager.Instance.DevilCastleLevelReset(clientSession.nickName);

        clientSession.YatzySingleGame = null;

        GameRoom lobby = clientSession.Lobby;
        if (clientSession.Lobby == null) return;

        lobby.Push(() => lobby.UniCast(clientSession, new ToC_ResLeaveSingleRoom()));
    }

    public static void ToS_ReqDevilCastleInfoHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession.Lobby == null) return;

        DevilCastleInfo info = DataManager.Instance.GetUserDevilCastleInfo(clientSession.nickName);
        if (info == null) return;

        GameRoom lobby = clientSession.Lobby;
        if (clientSession.Lobby == null) return;
        int reward = (int)Math.Pow(2, info.level) * 1000;
        if (info.level <= 0) reward = 0;
        lobby.Push(() => lobby.UniCast(clientSession, new ToC_ResDevilCastleInfo() { isOpened = info.opened, level = info.level, reward = reward }));
    }

    public static void ToS_ReqOpenDevilCastleHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession.Lobby == null) return;

        bool result = DataManager.Instance.OpenDevilCastle(clientSession.nickName);

        GameRoom lobby = clientSession.Lobby;
        if (clientSession.Lobby == null) return;

        lobby.Push(() => lobby.UniCast(clientSession, new ToC_ResOpenDevilCastle() { success = result }));
    }

    public static void ToS_ReqGetDevilCastleRewardHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession.Lobby == null) return;

        bool result = DataManager.Instance.GetDevilCastleReward(clientSession.nickName);

        GameRoom lobby = clientSession.Lobby;
        if (clientSession.Lobby == null) return;

        lobby.Push(() => lobby.UniCast(clientSession, new ToC_ResGetDevilCastleReward() { success = result }));
    }

    public static void ToS_ReqDevilCastleRankingHandler(PacketSession session, IPacket packet)
    {
        Console.WriteLine("ToS_ReqDevilCastleRankingHandler");

        ClientSession clientSession = session as ClientSession;
        if (clientSession.Lobby == null) return;

         GameRoom lobby = clientSession.Lobby;
        if (clientSession.Lobby == null) return;

        List<Tuple<string, int>> ranking = DataManager.Instance.GetDevilCastleRanking();
        ToC_RecDevilCastleRanking res = new ToC_RecDevilCastleRanking();
        foreach (var rank in ranking)
        {
            res.rankings.Add(new ToC_RecDevilCastleRanking.Ranking() { userName = rank.Item1, maxLevel = rank.Item2 });
        }

        lobby.Push(() => lobby.UniCast(clientSession, res));
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

    public static void ToS_LockDiceHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_LockDice lockDice = packet as ToS_LockDice;
        if (clientSession.GameRoom == null) 
            return;
        if (lockDice == null)
            return;

        ToC_LockDice toCLockDice = new ToC_LockDice();
        toCLockDice.diceIndex = lockDice.diceIndex;
        toCLockDice.isLocked = lockDice.isLocked;

        YatzyGameRoom gameRoom = clientSession.GameRoom;
        gameRoom.Push(() => gameRoom.BroadCast(toCLockDice));
    }

    public static void ToS_SelectScoreHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_SelectScore selectScore = packet as ToS_SelectScore;
        if (clientSession.GameRoom == null)
            return;
        if (selectScore == null)
            return;

        YatzyGameRoom gameRoom = clientSession.GameRoom;
        gameRoom.Push(() => gameRoom.SendSelectScore(clientSession, selectScore.jocboIndex));
    }


    // single games
    public static void ToS_ReqSingleRoomInfoHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_ReqSingleRoomInfo roomInfo = packet as ToS_ReqSingleRoomInfo;

        if (clientSession.YatzySingleGame == null) return;
        if (roomInfo == null) return;

        YatzySingleGame game = clientSession.YatzySingleGame;
        game.UniCast(new ToC_ResSingleRoomInfo() { userName = clientSession.nickName, mobName = "마몬" });
    }

    public static void ToS_SingleReadyToStartHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_SingleReadyToStart ready = packet as ToS_SingleReadyToStart;

        if (clientSession.YatzySingleGame == null) return;
        if (ready == null) return;

        YatzySingleGame game = clientSession.YatzySingleGame;
        game.Push(() => game.StartGame(clientSession));
    }

    public static void ToS_SingleRollDiceHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_SingleRollDice diceInfo = packet as ToS_SingleRollDice;

        if (clientSession.YatzySingleGame == null) return;
        if (diceInfo == null) return;

        List<int> fixDices = new List<int>();
        foreach (var fix in diceInfo.fixDices)
            fixDices.Add(fix.diceIndex);

        YatzySingleGame game = clientSession.YatzySingleGame;
        game.Push(() => game.RollDice(clientSession, fixDices));
    }

    public static void ToS_SingleWriteScoreHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        ToS_SingleWriteScore scoreInfo = packet as ToS_SingleWriteScore;

        if (clientSession.YatzySingleGame == null) return;
        if (scoreInfo == null) return;

        YatzySingleGame game = clientSession.YatzySingleGame;
        game.Push(() => game.WriteScore(clientSession, scoreInfo.jocboIndex));
    }
}
