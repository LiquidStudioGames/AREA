using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public bool IsClient;
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
    }

    private void Start()
    {
        NetworkScene = new NetworkScene();

        if (IsClient)
        {
            Steam = new SteamClient();
            if (!Steam.Init()) throw new Exception("Steam is not running.");
            Debug.Log($"Logged in as {Steam.Player.Name}");
            Steam.CreateLobby();
            Steam.OnLobbyEvent += OnLobbyEvent;
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

    private void OnLobbyEvent(LobbyEvent e)
    {
        if (e == LobbyEvent.Created)
        {
            Steam.StartListen();
            Steam.LoadLevel("GameTestScene");
        }
    }
}