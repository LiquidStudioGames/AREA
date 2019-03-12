using Steamworks;
using UnityEngine;
using System;
using System.Collections.Generic;

public class SteamPlayer
{
    public ulong ID;
    public string Name;
    public CSteamID LobbyID;

    // Avatar: 4 pixels, 64px by 64px Image
    private static byte[] avatarBuffer = new byte[4 * 64 * 64];

    public event Action OnUpdated = delegate { };

    internal SteamPlayer(ulong id)
    {
        ID = id;
        Update();
    }

    internal SteamPlayer(CSteamID id) : this((ulong)id) { }

    internal void SetLobby(CSteamID lobby)
    {
        LobbyID = lobby;
    }

    public void Update()
    {
        if (!Game.Instance.IsClient || ID <= 1000)
        {
            Name = ID.ToString();
            OnUpdated();
            return;
        }

        if (this == Game.Instance.Steam.Player) Name = SteamFriends.GetPersonaName();

        else if (!SteamFriends.RequestUserInformation((CSteamID)ID, false))
        {
            Name = SteamFriends.GetFriendPersonaName((CSteamID)ID);
        }

        else
        {
            Name = "Loading ...";
        }

        OnUpdated();
    }

    public Texture2D GetAvatar()
    {
        int avatar;

        if ((avatar = SteamFriends.GetMediumFriendAvatar((CSteamID)ID)) == 0)
        {
            Debug.Log("Player has no avatar");
            return null;
        }

        if (SteamUtils.GetImageRGBA(avatar, avatarBuffer, avatarBuffer.Length))
        {
            Texture2D t2 = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            t2.LoadRawTextureData(avatarBuffer);
            t2.Apply();
            return t2;
        }

        Debug.Log("Failed to laod avatar");
        return null;
    }

    public static SteamPlayer FromID(ulong id)
    {
        if (!Game.Instance.IsClient) return new SteamPlayer(id);
        return Game.Instance.Steam.players.GetPlayer(id);
    }

    public static SteamPlayer FromID(CSteamID id)
    {
        return FromID((ulong)id);
    }

    public static SteamPlayer FromID(CSteamID id, CSteamID lobby)
    {
        SteamPlayer result = FromID((ulong)id);
        result.SetLobby(lobby);
        return result;
    }
}

public class SteamPlayerCollection
{
    public Dictionary<ulong, SteamPlayer> players;

    public SteamPlayerCollection()
    {
        players = new Dictionary<ulong, SteamPlayer>();
    }

    public SteamPlayer GetPlayer(ulong id)
    {
        if (!players.ContainsKey(id)) players.Add(id, new SteamPlayer(id));
        return players[id];
    }

    public SteamPlayer[] GetFriends()
    {
        List<SteamPlayer> players = new List<SteamPlayer>();
        int count = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

        for (int i = 0; i < count; i++)
        {
            FriendGameInfo_t friendGameInfo;
            CSteamID steamIDFriend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);

            if (SteamFriends.GetFriendGamePlayed(steamIDFriend, out friendGameInfo) && friendGameInfo.m_steamIDLobby.IsValid())
                players.Add(SteamPlayer.FromID(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate), friendGameInfo.m_steamIDLobby));

            else if (friendGameInfo.m_gameID.AppID() == SteamUtils.GetAppID())
                players.Add(SteamPlayer.FromID(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate)));
        }

        return players.ToArray();
    }

    public SteamPlayer[] GetSentFriendRequests()
    {
        List<SteamPlayer> players = new List<SteamPlayer>();
        int count = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagRequestingFriendship);
        for (int i = 0; i < count; i++) players.Add(SteamPlayer.FromID(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagRequestingFriendship)));
        return players.ToArray();
    }

    public SteamPlayer[] GetReceivedFriendRequests()
    {
        List<SteamPlayer> players = new List<SteamPlayer>();
        int count = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagFriendshipRequested);
        for (int i = 0; i < count; i++) players.Add(SteamPlayer.FromID(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagFriendshipRequested)));
        return players.ToArray();
    }

    internal void PlayerUpdated(PersonaStateChange_t callback)
    {
        if (players.ContainsKey(callback.m_ulSteamID))
        {
            players[callback.m_ulSteamID].Update();
        }
    }
}