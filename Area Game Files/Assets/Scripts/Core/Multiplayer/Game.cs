using System;
using System.Collections.Generic;
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
    public GamemodeData GamemodeData;

    public GameState State;

    private Queue<Action> toUnity;

    private void Awake ()
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

    private void Start ()
    {
        State = GameState.Login;
        toUnity = new Queue<Action>();
        NetworkScene = new NetworkScene();

        if (IsClient)
        {
            Steam = new SteamClient();
            if (!Steam.Init()) throw new Exception("Steam is not running.");
            Debug.Log($"Logged in as {Steam.Player.Name}");
            Steam.OnLobbyEvent += OnLobbyEvent;

            Settings = new Settings();
            Settings.Load();

            // Delay the menu one frame
            ToUnity(() => ChangeState(GameState.Menu));
        }
    }

    public void ChangeState (GameState state)
    {
        UI.UpdateState(State, state);
        State = state;
    }

    public void ToUnity (Action action)
    {
        lock (toUnity) toUnity.Enqueue(action);
    }

    private void Update ()
    {
        if (IsClient)
        {
            Steam.Update();
        }
        
        lock (toUnity)
        {
            while (toUnity.Count > 0)
            {
                toUnity.Dequeue()();
            }
        }
    }

    private void OnDestroy ()
    {
        if (IsClient)
        {
            Steam.Stop();
        }

        Instance = null;
    }

    private void OnLobbyEvent (LobbyEvent e)
    {
        if (e == LobbyEvent.Created || e == LobbyEvent.Joined)
        {
            ChangeState(GameState.Game);
            GamemodeData = new GamemodeData();

            // This is basically pressing 'Start Game' immediatly
            Steam.StartListen();
            Steam.LoadLevel("Game");
        }
    }
}