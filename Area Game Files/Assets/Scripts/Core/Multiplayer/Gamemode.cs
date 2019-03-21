﻿using UnityEngine;

public class Gamemode : MonoBehaviour
{
    public AssetObject playerAsset;
    public Transform spawn;

    public GamemodeData data;

    private NetworkTag networkTag;

    private void Start()
    {
        data = Game.Instance?.GamemodeData;
        networkTag = GetComponent<NetworkTag>();
        
        networkTag.Call(PlayerJoin, NetworkTarget.Host, new BitStream(), SendType.Reliable);
    }

    [NetworkCall]
    private void PlayerJoin(BitStream stream, SteamPlayer sender)
    {
        networkTag.Instantiate(playerAsset, sender, out uint tag, spawn.position, spawn.rotation);
        networkTag.Call(SpawnPlayer, NetworkTarget.Others, new BitStream().Write(tag).Write(sender.ID), SendType.Reliable);
    }

    [NetworkCall]
    private void SpawnPlayer(BitStream stream, SteamPlayer sender)
    {
        uint tag = stream.ReadUInt();
        SteamPlayer player = SteamPlayer.FromID(stream.ReadULong());
        networkTag.Instantiate(playerAsset, player, tag, spawn.position, spawn.rotation);
    }
}