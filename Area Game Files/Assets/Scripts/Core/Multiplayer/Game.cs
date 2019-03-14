using System;
using UnityEngine;

public enum GameState
{
    Login,
    Menu,
    Browse,
    Game,
    Settings
}

public class Game : MonoBehaviour
{
    public static Game Instance;

    public bool IsClient;

    public UI UI;
    public Settings Settings;
    public SteamClient Steam;
    public NetworkScene NetworkScene;

    public GameState State;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UI.gameObject);
    }

    private void Start()
    {
        State = GameState.Login;
        NetworkScene = new NetworkScene();

        if (IsClient)
        {
            Steam = new SteamClient();
            if (!Steam.Init()) throw new Exception("Steam is not running.");
            Settings = new Settings();
            Settings.Load();

            ChangeState(GameState.Menu);
            Debug.Log($"Logged in as {Steam.Player.Name}");
            Steam.OnLobbyEvent += OnLobbyEvent;

            ChangeState(GameState.Settings);
        }
    }

    public void ChangeState(GameState state)
    {
        UI.UpdateState(State, state);
        State = state;
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
        if (e == LobbyEvent.Created || e == LobbyEvent.Joined)
        {
            ChangeState(GameState.Game);

            // This is basically pressing 'Start Game' immediatly
            Steam.StartListen();
            Steam.LoadLevel("GameTestScene");
        }
    }
}