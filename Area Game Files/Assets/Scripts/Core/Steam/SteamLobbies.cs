using System.Linq;
using System.Collections.Generic;
using Steamworks;

public class SteamLobby
{
    public ulong ID;
    public SteamPlayer host;
    public SteamPlayer[] playerList;
    public int playerCount;
    public int maxPlayerCount;

    public bool isHost;
    public bool inQueue = false;

    internal SteamLobby(ulong id)
    {
        ID = id;

        if (id != 0)
        {
            Update();
        }
    }

    internal SteamLobby(CSteamID id) : this((ulong)id) { }

    public void Update()
    {
        List<SteamPlayer> players = new List<SteamPlayer>();
        playerCount = SteamMatchmaking.GetNumLobbyMembers((CSteamID)ID);
        maxPlayerCount = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)ID);
        host = SteamPlayer.FromID(SteamMatchmaking.GetLobbyOwner((CSteamID)ID));
        isHost = host == Game.Instance.Steam.Player;

        for (int i = 0; i < playerCount; i++)
        {
            CSteamID playerID = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)ID, i);
            players.Add(SteamPlayer.FromID(playerID));
        }

        playerList = players.ToArray();
    }

    public void DataUpdate()
    {

    }

    public void HandlePacket(byte[] data)
    {

    }

    public static SteamLobby FromID(ulong id)
    {
        if (!Game.Instance.IsClient) return new SteamLobby(id);
        return Game.Instance.Steam.lobbies[id];
    }

    public static SteamLobby FromID(CSteamID id)
    {
        return FromID((ulong)id);
    }
}

public class SteamLobbyCollection
{
    public Dictionary<ulong, SteamLobby> lobbies;

    public SteamLobbyCollection()
    {
        lobbies = new Dictionary<ulong, SteamLobby>();
    }

    public SteamLobby this[ulong id]
    {
        get
        {
            if (!lobbies.ContainsKey(id)) lobbies.Add(id, new SteamLobby(id));
            return lobbies[id];
        }
    }

    public SteamLobby[] GetLobbies()
    {
        return lobbies.Values.ToArray();
    }

    internal void LobbyUpdate(ulong id)
    {
        if (lobbies.ContainsKey(id)) lobbies[id].Update();
    }

    internal void LobbyDataUpdate(ulong id)
    {
        if (lobbies.ContainsKey(id)) lobbies[id].DataUpdate();
    }
}