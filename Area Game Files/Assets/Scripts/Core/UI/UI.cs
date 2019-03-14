using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI : MonoBehaviour
{
    public GameObject Main;
    public GameObject Lobbies;
    public GameObject Settings;

    void Awake()
    {
        Main.SetActive(true);
        Lobbies.SetActive(false);
        Settings.SetActive(false);
    }

    public void ChangeState(string state)
    {
        Game.Instance.ChangeState((GameState)Enum.Parse(typeof(GameState), state, true));
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void UpdateState(GameState previous, GameState current)
    {
        switch (previous)
        {
            case GameState.Menu:
                Main.SetActive(false);
                break;

            case GameState.Browse:
                Lobbies.SetActive(false);
                break;

            case GameState.Settings:
                Settings.SetActive(false);
                break;

        }

        switch (current)
        {
            case GameState.Menu:
                Main.SetActive(true);
                break;

            case GameState.Browse:
                Lobbies.SetActive(true);
                break;

            case GameState.Settings:
                Settings.SetActive(true);
                break;
        }
    }
}
