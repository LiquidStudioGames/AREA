using UnityEngine;

public class Gamemode : MonoBehaviour
{
    public GameObject playerObj;
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
        GameObject o = Instantiate(playerObj, spawn.position, spawn.rotation);
        uint tag = Game.Instance.NetworkScene.SetTag(o, sender).ID;
        networkTag.Call(SpawnPlayer, NetworkTarget.Others, new BitStream().Write(tag).Write(sender.ID), SendType.Reliable);
    }

    [NetworkCall]
    private void SpawnPlayer(BitStream stream, SteamPlayer sender)
    {
        uint tag = stream.ReadUInt();
        SteamPlayer player = SteamPlayer.FromID(stream.ReadULong());
        GameObject o = Instantiate(playerObj, spawn.position, spawn.rotation);
        Game.Instance.NetworkScene.SetTag(o, player, tag);
    }
}