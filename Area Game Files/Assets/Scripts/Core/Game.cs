using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public bool IsClient;
    public SteamClient Steam;

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
        if (IsClient)
        {
            Steam = new SteamClient();
            if (!Steam.Init()) throw new Exception("Steam is not running.");
            Debug.Log($"Logged in as {Steam.Player.Name}");
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
}