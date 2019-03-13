using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class SteamClient
{
    #region Common
    public bool Ready;
    public SteamLobby Lobby;
    public SteamPlayer Player;
    public SteamLobbyCollection lobbies;
    public SteamPlayerCollection players;

    public event Action OnLobbyListReceived = delegate { };
    public event Action<LobbyEvent> OnLobbyEvent = delegate { };
    public event Action<SteamPlayer, PlayerStateChange> OnLobbyUpdated = delegate { };

    private CallResult<LobbyEnter_t> lobbyJoin;
    private CallResult<LobbyCreated_t> lobbyCreate;
    private CallResult<LobbyMatchList_t> lobbyListRequest;
    private Callback<LobbyChatMsg_t> lobbyMessage;
    private Callback<LobbyChatUpdate_t> lobbyUpdate;
    private Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    private Callback<GameLobbyJoinRequested_t> lobbyInvite;

    private Callback<PersonaStateChange_t> playerUpdate;
    private Callback<P2PSessionRequest_t> sessionRequest;
    private Callback<P2PSessionConnectFail_t> sessionFail;

    private bool online;
    private Thread thread;
    private Queue<Action> toUnity;

    public bool LoadingLevel = false;

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

        SceneManager.sceneLoaded += SceneLoaded;
        lobbies = new SteamLobbyCollection();
        players = new SteamPlayerCollection();
        Player = SteamPlayer.FromID(SteamUser.GetSteamID());
        lobbyJoin = CallResult<LobbyEnter_t>.Create(LobbyJoined);
        lobbyCreate = CallResult<LobbyCreated_t>.Create(LobbyCreated);
        lobbyListRequest = CallResult<LobbyMatchList_t>.Create(OnLobbyList);
        lobbyMessage = Callback<LobbyChatMsg_t>.Create(LobbyMessageReceived);
        lobbyUpdate = Callback<LobbyChatUpdate_t>.Create(LobbyChatUpdated);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(LobbyDataUpdated);
        lobbyInvite = Callback<GameLobbyJoinRequested_t>.Create(LobbyInvite);
        playerUpdate = Callback<PersonaStateChange_t>.Create(players.PlayerUpdated);
        sessionRequest = Callback<P2PSessionRequest_t>.Create(SessionRequest);
        sessionFail = Callback<P2PSessionConnectFail_t>.Create(SessionFail);
        return true;
    }

    public void Update()
    {
        if (!Ready) return;
        SteamAPI.RunCallbacks();

        if (toUnity == null) return;
        lock (toUnity)
        {
            while (toUnity.Count > 0)
            {
                toUnity.Dequeue()();
            }
        }
    }

    public void Stop()
    {
        if (online) StopListen();
        if (Lobby != null) LeaveLobby();
        SteamAPI.Shutdown();
    }

    private void ToUnity(Action action)
    {
        lock (toUnity)
            toUnity.Enqueue(action);
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

    #region Networking
    public void StartListen()
    {
        online = true;
        toUnity = new Queue<Action>();
        thread = new Thread(Listen);
    }

    public void StopListen()
    {
        online = false;
        toUnity.Clear();
        thread.Abort();
    }

    public void LoadLevel(string level)
    {
        if (!Lobby.isHost) return;

        LoadingLevel = true;
        BitStream stream = new BitStream().Write((byte)PacketType.Level).Write(level);
        SendPackets(stream.GetBytes(), SendType.Reliable, Player);
        Game.Instance.NetworkScene.Reset();
        SceneManager.LoadScene(level);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadingLevel = false;

        if (Lobby.isHost)
        {
            Game.Instance.NetworkScene.HandleServerSpawn();
            ushort count = (ushort)Game.Instance.NetworkScene.spawns.Count;

            BitStream stream = new BitStream().Write((byte)PacketType.Spawns).Write(count);
            for (int i = 0; i < count; i++) stream.Write(Game.Instance.NetworkScene.spawns[i]);

            SendPackets(stream.GetBytes(), SendType.Reliable, Player);
        }
    }

    private void CheckSceneLoaded(List<NetworkSpawn> spawns)
    {
        if (Lobby.isHost) return;

        if (LoadingLevel)
        {
            ToUnity(() => CheckSceneLoaded(spawns));
        }

        else
        {
            Game.Instance.NetworkScene.spawns = spawns;
            Game.Instance.NetworkScene.HandleClientSpawn();
        }
    }

    private void Listen()
    {
        uint length;
        byte[] data;
        CSteamID remote;

        while (online)
        {
            try
            {
                while (SteamNetworking.IsP2PPacketAvailable(out length))
                {
                    data = new byte[length];

                    if (SteamNetworking.ReadP2PPacket(data, 0, out length, out remote))
                    {
                        if (data.Length != length)
                            Debug.Log($"Received different lengths {length}/{data.Length}");

                        ReceivePacket(data, SteamPlayer.FromID(remote));
                    }
                }

            }

            catch (Exception e)
            {
                Debug.Log($"Error in Listen {e.Message}");
                StopListen();
            }

            Thread.Sleep(5);
        }
    }

    internal void ReceivePacket(byte[] data, SteamPlayer sender)
    {
        BitStream stream = new BitStream(data);
        PacketType packetType = (PacketType)stream.ReadByte();

        switch (packetType)
        {
            case PacketType.Call:
                ToUnity(() => Game.Instance.NetworkScene.ReceiveCall(stream, sender));
                break;

            case PacketType.Level:
                {
                    string level = stream.ReadString();
                    Game.Instance.NetworkScene.Reset();
                    LoadingLevel = true;
                    ToUnity(() => SceneManager.LoadScene(level));
                }
                break;

            case PacketType.Spawns:
                {
                    ushort count = stream.ReadUShort();
                    List<NetworkSpawn> spawns = new List<NetworkSpawn>();
                    for (int i = 0; i < count; i++) spawns.Add(stream.ReadNetworkObject<NetworkSpawn>());
                    ToUnity(() => CheckSceneLoaded(spawns));
                }
                break;

            case PacketType.Proxy:
                {
                    SteamPlayer target = SteamPlayer.FromID((ulong)stream.ReadLong());
                    SendType type = (SendType)stream.ReadByte(1);
                    byte[] packet = stream.ReadBytes();
                    SendPacket(packet, target, type);
                }
                break;

            case PacketType.ProxyTarget:
                {
                    NetworkTarget target = (NetworkTarget)stream.ReadByte(2);
                    SendType type = (SendType)stream.ReadByte(1);
                    byte[] packet = stream.ReadBytes();

                    switch (target)
                    {
                        case NetworkTarget.All:
                            SendPackets(packet, type, null);
                            break;

                        case NetworkTarget.Others:
                            SendPackets(packet, type, sender);
                            break;
                    }
                }
                break;
        }
    }

    internal void SendPacket(byte[] packet, SteamPlayer target, SendType sendType = SendType.Unreliable)
    {
        if (Game.Instance.Steam.Lobby.isHost || target == Game.Instance.Steam.Lobby.host)
        {
            if (!SteamNetworking.SendP2PPacket((CSteamID)target.ID, packet, (uint)packet.Length, sendType == SendType.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable))
            {
                Debug.LogError($"Failed to send packet to {target.ID}");
            }
        }

        else
        {
            byte[] proxyPacket = new BitStream().Write((byte)PacketType.Proxy).Write((long)target.ID).Write((byte)sendType, 1).Write(packet).GetBytes();

            if (!SteamNetworking.SendP2PPacket((CSteamID)Lobby.host.ID, proxyPacket, (uint)proxyPacket.Length, sendType == SendType.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable))
            {
                Debug.LogError($"Failed to send packet to {Lobby.host.ID}");
            }
        }
    }

    internal void SendPacket(byte[] packet, NetworkTarget target, SendType sendType = SendType.Unreliable)
    {
        if (Game.Instance.Steam.Lobby.isHost)
        {
            switch (target)
            {
                case NetworkTarget.All:
                    SendPackets(packet, sendType, null);
                    break;

                case NetworkTarget.Others:
                    SendPackets(packet, sendType, Player);
                    break;

                case NetworkTarget.Host:
                    ReceivePacket(packet, Player);
                    break;
            }
        }

        else
        {
            if (target == NetworkTarget.Host)
            {
                SendPacket(packet, Lobby.host, sendType);
            }

            else
            {
                byte[] proxyPacket = new BitStream().Write((byte)PacketType.ProxyTarget).Write((byte)target, 2).Write((byte)sendType, 1).Write(packet).GetBytes();

                if (!SteamNetworking.SendP2PPacket((CSteamID)Lobby.host.ID, proxyPacket, (uint)proxyPacket.Length, sendType == SendType.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable))
                {
                    Debug.LogError($"Failed to send packet to {Lobby.host.ID}");
                }
            }
        }
    }

    private void SendPackets(byte[] packet, SendType sendType, SteamPlayer exception = null)
    {
        foreach (SteamPlayer player in Lobby.playerList)
        {
            if (player != exception)
            {
                if (player == Player)
                {
                    ReceivePacket(packet, Player);
                }

                else if (!SteamNetworking.SendP2PPacket((CSteamID)player.ID, packet, (uint)packet.Length, sendType == SendType.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable))
                {
                    Debug.LogError($"Failed to send packet to {player.ID}");
                }
            }
        }
    }

    private void SessionRequest(P2PSessionRequest_t request)
    {
        if (Lobby != null)
        {
            if (Lobby.isHost && !Lobby.playerList.Any(x => x.ID == (ulong)request.m_steamIDRemote))
            {
                Debug.LogWarning($"Player outside this lobby tried to connect {request.m_steamIDRemote}");
            }

            else if (Lobby.host.ID != (ulong)request.m_steamIDRemote)
            {
                Debug.LogWarning($"Player who is not host of this lobby tried to connect {request.m_steamIDRemote}");
            }

            if (SteamNetworking.AcceptP2PSessionWithUser(request.m_steamIDRemote))
            {
                // Connected with remote
                Debug.Log($"Connected to {request.m_steamIDRemote}");
            }

            else Debug.LogError($"Failed to accept session with {request.m_steamIDRemote}");
        }
    }

    private void SessionFail(P2PSessionConnectFail_t fail)
    {
        Debug.LogWarning($"Failed to start session with {fail.m_steamIDRemote}: {fail.m_eP2PSessionError}");
    }
    #endregion

    #region Lobbies
    public void CreateLobby()
    {
        lobbyCreate.Set(SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4));
    }

    public void JoinLobby(ulong id)
    {
        lobbyJoin.Set(SteamMatchmaking.JoinLobby((CSteamID)id));
    }

    public void GetLobbyList()
    {
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
        SteamMatchmaking.AddRequestLobbyListStringFilter("Game", "Area", ELobbyComparison.k_ELobbyComparisonEqual);
        lobbyListRequest.Set(SteamMatchmaking.RequestLobbyList());
    }

    public void LeaveLobby()
    {
        if (Lobby != null)
        {
            SteamMatchmaking.LeaveLobby((CSteamID)Lobby.ID);
            Debug.Log("Left lobby " + Lobby.ID.ToString());
            Lobby = null;
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

    private void OnLobbyList(LobbyMatchList_t callback, bool failed)
    {
        if (failed)
        {
            Debug.LogError("Requesting lobbylist failed.");
            return;
        }

        Debug.Log("Current Lobbies: " + callback.m_nLobbiesMatching);
        lobbies.lobbies.Clear();

        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            ulong lobbyListID = (ulong)SteamMatchmaking.GetLobbyByIndex(i);
            var lobby = lobbies[lobbyListID];
        }

        OnLobbyListReceived();
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

        Lobby = lobbies[callback.m_ulSteamIDLobby];
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

        Lobby = lobbies[callback.m_ulSteamIDLobby];
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
        lobbies.LobbyDataUpdate(callback.m_ulSteamIDLobby);

        if (Lobby.ID == callback.m_ulSteamIDLobby)
        {
            OnLobbyEvent(LobbyEvent.Updated);
        }
    }

    private void LobbyChatUpdated(LobbyChatUpdate_t callback)
    {
        Debug.Log("Lobby updated: " + callback.m_ulSteamIDLobby);
        lobbies.LobbyUpdate(callback.m_ulSteamIDLobby);

        if (Lobby.ID == callback.m_ulSteamIDLobby)
        {
            OnLobbyUpdated(players[callback.m_ulSteamIDUserChanged], (PlayerStateChange)callback.m_rgfChatMemberStateChange);
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
        sessionRequest.Dispose();
        sessionFail.Dispose();
    }
}

public enum SendType : byte // 1 bit
{
    Unreliable = 0,
    Reliable = 1
}

public enum NetworkTarget : byte // 2 bits
{
    All = 0,
    Others = 1,
    Host = 2
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