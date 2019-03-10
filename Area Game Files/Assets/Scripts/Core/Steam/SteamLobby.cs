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

    public SteamLobby(ulong id)
    {
        ID = id;

        if (id != 0)
        {
            Update();
        }
    }

    public SteamLobby(CSteamID id) : this((ulong)id) { }

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
}