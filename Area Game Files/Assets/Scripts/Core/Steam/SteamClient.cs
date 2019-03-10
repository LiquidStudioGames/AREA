using System;
using UnityEngine;
using Steamworks;

public class SteamClient
{
    #region Common
    public bool Ready;
    public SteamLobby Lobby;
    public SteamPlayer Player;
    public SteamPlayerCollection players;

    public event Action<LobbyEvent> OnLobbyEvent = delegate { };
    public event Action<SteamPlayer, PlayerStateChange> OnLobbyUpdated = delegate { };

    private CallResult<LobbyEnter_t> lobbyJoin;
    private CallResult<LobbyCreated_t> lobbyCreate;
    private Callback<LobbyChatMsg_t> lobbyMessage;
    private Callback<LobbyChatUpdate_t> lobbyUpdate;
    private Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    private Callback<GameLobbyJoinRequested_t> lobbyInvite;
    private Callback<PersonaStateChange_t> playerUpdate;

    public bool Init()
    {
        if (Ready)
            throw new Exception("Tried to Initialize the SteamAPI twice in one session!");

        if (!Packsize.Test())
            Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");

        if (!DllCheck.Test())
            Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");

        try
        {
            if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
            {
                Application.Quit();
                return false;
            }
        }

        catch (DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);
            Application.Quit();
            return false;
        }

        Ready = SteamAPI.Init();

        if (!Ready)
        {
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
            return false;
        }

        if (m_SteamAPIWarningMessageHook == null)
        {
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            Steamworks.SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
        }
        
        players = new SteamPlayerCollection();
        Player = SteamPlayer.FromID(SteamUser.GetSteamID());
        lobbyJoin = CallResult<LobbyEnter_t>.Create(LobbyJoined);
        lobbyCreate = CallResult<LobbyCreated_t>.Create(LobbyCreated);
        lobbyMessage = Callback<LobbyChatMsg_t>.Create(LobbyMessageReceived);
        lobbyUpdate = Callback<LobbyChatUpdate_t>.Create(LobbyChatUpdated);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(LobbyDataUpdated);
        lobbyInvite = Callback<GameLobbyJoinRequested_t>.Create(LobbyInvite);
        playerUpdate = Callback<PersonaStateChange_t>.Create(players.PlayerUpdated);
        return true;
    }

    public void Update()
    {
        if (!Ready) return;
        SteamAPI.RunCallbacks();
    }

    public void Stop()
    {
        if (Lobby != null) LeaveLobby();
        SteamAPI.Shutdown();
    }

    private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
    private void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
    {
        Debug.LogWarning(pchDebugText);
    }
    #endregion

    #region Identity
    private CallResult<EncryptedAppTicketResponse_t> ticketRequest;
    public void GetAuthenticationToken(Action<byte[]> callback)
    {
        ticketRequest = new CallResult<EncryptedAppTicketResponse_t>();
        ticketRequest.Set(SteamUser.RequestEncryptedAppTicket(BitConverter.GetBytes(0x5444), sizeof(uint)), (c, f) => OnTicket(c, f, callback));
    }

    private void OnTicket(EncryptedAppTicketResponse_t response, bool failed, Action<byte[]> callback)
    {
        if (!failed && response.m_eResult == EResult.k_EResultOK)
        {
            Debug.Log("Creating ticket");
            byte[] ticket = new byte[1024];
            uint length;

            if (SteamUser.GetEncryptedAppTicket(ticket, ticket.Length, out length))
            {
                BitStream stream = new BitStream();
                stream.Write(ticket);
                stream.Write(length, 11);
                stream.Write(Player.ID);
                callback(stream.GetBytes());
            }

            else Debug.LogError("Error creating ticket: " + response.m_eResult);
        }

        else Debug.LogError("Error requesting ticket: " + response.m_eResult);
    }
    #endregion

    #region Lobbies
    public void CreateLobby()
    {
        lobbyCreate.Set(SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 6));
    }

    public void JoinLobby(ulong id)
    {
        lobbyJoin.Set(SteamMatchmaking.JoinLobby((CSteamID)id));
    }

    public void LeaveLobby()
    {
        if (Lobby != null)
        {
            SteamMatchmaking.LeaveLobby((CSteamID)Lobby.ID);
            Lobby = null;
            Debug.Log("Left lobby " + Lobby.ID.ToString());
            OnLobbyEvent(LobbyEvent.Left);
        }
    }

    public void InvitePlayer(SteamPlayer player)
    {
        if (!SteamMatchmaking.InviteUserToLobby((CSteamID)Lobby.ID, (CSteamID)player.ID))
        {
            Debug.Log("Failed to invite player.");
        }
    }

    public void SetLobbyClosed(bool closed)
    {
        if (Lobby != null && Lobby.isHost)
        {
            if (SteamMatchmaking.SetLobbyJoinable((CSteamID)Lobby.ID, !closed))
            {
                if (!SteamMatchmaking.SetLobbyData((CSteamID)Lobby.ID, "matchmaking", closed.ToString()))
                {
                    Debug.Log($"Failed to set {(closed ? "close" : "open")} lobby.");
                }
            }

            else Debug.Log($"Failed to {(closed ? "close" : "open")} lobby.");
        }
    }

    private void LobbyCreated(LobbyCreated_t callback, bool failed)
    {
        if (failed)
        {
            Debug.LogError("Creating lobby failed.");
            return;
        }

        if (!SteamMatchmaking.RequestLobbyData((CSteamID)callback.m_ulSteamIDLobby))
            Debug.LogError("Failed to retrieve lobby data.");

        Lobby = new SteamLobby(callback.m_ulSteamIDLobby);
        Debug.Log("Lobby created: " + callback.m_ulSteamIDLobby);
        OnLobbyEvent(LobbyEvent.Created);
    }

    private void LobbyJoined(LobbyEnter_t callback, bool failed)
    {
        if (failed)
        {
            Debug.LogError("Joining lobby failed.");
            return;
        }

        Lobby = new SteamLobby((CSteamID)callback.m_ulSteamIDLobby);
        Debug.Log("Lobby Joined: " + callback.m_ulSteamIDLobby);
        OnLobbyEvent(LobbyEvent.Joined);
    }

    private void LobbyInvite(GameLobbyJoinRequested_t callback)
    {
        JoinLobby((ulong)callback.m_steamIDLobby);
    }

    private void LobbyDataUpdated(LobbyDataUpdate_t callback)
    {
        Debug.Log($"Lobby {callback.m_ulSteamIDLobby} data updated: {callback.m_ulSteamIDMember}");

        if (Lobby.ID == callback.m_ulSteamIDLobby)
        {
            Lobby.DataUpdate();
            OnLobbyEvent(LobbyEvent.Updated);
        }
    }

    private void LobbyChatUpdated(LobbyChatUpdate_t callback)
    {
        Debug.Log("Lobby updated: " + callback.m_ulSteamIDLobby);

        if (Lobby.ID == callback.m_ulSteamIDLobby)
        {
            Lobby.Update();
            OnLobbyUpdated(players.GetPlayer(callback.m_ulSteamIDUserChanged), (PlayerStateChange)callback.m_rgfChatMemberStateChange);
        }
    }

    public void SendLobbyMessage(byte[] packet)
    {
        if (!SteamMatchmaking.SendLobbyChatMsg((CSteamID)Lobby.ID, packet, packet.Length))
            Debug.LogError("Failed to send message.");
    }

    private void LobbyMessageReceived(LobbyChatMsg_t callback)
    {
        CSteamID sender;
        byte[] buffer = new byte[1024];
        EChatEntryType type;
        byte[] result = new byte[SteamMatchmaking.GetLobbyChatEntry((CSteamID)callback.m_ulSteamIDLobby, (int)callback.m_iChatID, out sender, buffer, 1024, out type)];
        Buffer.BlockCopy(buffer, 0, result, 0, result.Length);
        Lobby.HandlePacket(result);
    }
    #endregion

    private void DisposeCalls()
    {
        lobbyMessage.Dispose();
        lobbyUpdate.Dispose();
        lobbyDataUpdate.Dispose();
        lobbyInvite.Dispose();
        playerUpdate.Dispose();
    }
}

public enum LobbyEvent
{
    Created,
    Joined,
    Updated,
    Left
}

public enum PlayerStateChange : int
{
    Joined = 0x0001,
    Left = 0x0002,
    Disconnected = 0x0004,
    Kicked = 0x0008,
    Banned = 0x0010
}