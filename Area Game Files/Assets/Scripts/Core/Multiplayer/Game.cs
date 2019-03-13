using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public bool IsClient;
    public GameObject UI;
    public SteamClient Steam;
    public NetworkScene NetworkScene;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UI);
    }

    private void Start()
    {
        NetworkScene = new NetworkScene();

        if (IsClient)
        {
            Steam = new SteamClient();
            if (!Steam.Init()) throw new Exception("Steam is not running.");
            Debug.Log($"Logged in as {Steam.Player.Name}");
            Steam.OnLobbyEvent += OnLobbyEvent;
            Steam.OnLobbyListReceived += ShowLobbyList;
            Steam.GetLobbyList();
        }
    }

    private void Update()
    {
        if (IsClient)
        {
            Steam.Update();
        }
    }

    private void OnDestroy()
    {
        if (IsClient)
        {
            Steam.Stop();
        }

        Instance = null;
    }

    public void CreateLobby()
    {
        Steam.CreateLobby();
    }

    public void JoinLobby(ulong lobby)
    {
        Steam.JoinLobby(lobby);
    }

    private void ShowLobbyList()
    {
        foreach (SteamLobby lobby in Steam.lobbies.GetLobbies())
        {
            Debug.Log(lobby.ID);
        }
    }

    private void OnLobbyEvent(LobbyEvent e)
    {
        if (e == LobbyEvent.Created)
        {
            // This is basically pressing 'Start Game' immediatly
            Steam.StartListen();
            Steam.LoadLevel("GameTestScene");
        }
    }
}