using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    public GameObject LobbyObj;
    public RectTransform List;

    private SteamClient Steam;

    void Start()
    {
        Steam = Game.Instance.Steam;
        Refresh();
        Steam.OnLobbyListReceived += ShowLobbyList;
    }

    public void Refresh()
    {
        Steam.GetLobbyList();
    }

    public void CreateLobby()
    {
        Steam.CreateLobby();
    }

    private void OnDestroy()
    {
        Steam.OnLobbyListReceived -= ShowLobbyList;
    }

    private void ShowLobbyList()
    {
        for (int i = 0; i < List.childCount; i++)
        {
            Destroy(List.GetChild(i).gameObject);
        }

        foreach (SteamLobby lobby in Steam.lobbies.GetLobbies())
        {
            GameObject o = Instantiate(LobbyObj, List);
            o.GetComponentInChildren<Text>().text = lobby.ID.ToString();
            o.GetComponent<Button>().onClick.AddListener(() => Steam.JoinLobby(lobby.ID));
        }
    }
}
