using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject Main;
    public GameObject Lobbies;

    void Start()
    {
        Main.SetActive(true);
        Lobbies.SetActive(true);
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
        }

        switch (current)
        {
            case GameState.Menu:
                Main.SetActive(true);
                break;

            case GameState.Browse:
                Lobbies.SetActive(true);
                break;
        }
    }
}
