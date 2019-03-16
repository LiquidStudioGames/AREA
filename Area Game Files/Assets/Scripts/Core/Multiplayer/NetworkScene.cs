using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NetworkScene
{
    internal uint tagCount;
    internal List<NetworkSpawn> spawns;
    internal Dictionary<uint, NetworkTag> tags;

    public NetworkScene()
    {
        Reset();
    }

    public void Reset()
    {
        tagCount = 1;
        spawns = new List<NetworkSpawn>();
        tags = new Dictionary<uint, NetworkTag>();
    }

    internal void HandleServerSpawn()
    {
        NetworkTag[] spawnTags = Object.FindObjectsOfType<NetworkTag>();

        foreach (NetworkTag tag in spawnTags)
        {
            SteamPlayer owner = SteamPlayer.FromID(0);
            SetTag(tag.gameObject, owner);

            spawns.Add(new NetworkSpawn()
            {
                asset = new AssetObject(tag.transform.name, "UnityEngine"),
                owner = owner,
                position = (tag.transform is RectTransform ? Vector3.zero : tag.transform.position),
                rotation = tag.transform.rotation,
                tag = tag.ID
            });
        }
    }

    internal NetworkTag SetTag(GameObject obj, SteamPlayer owner)
    {
        NetworkTag tag = obj.GetComponent<NetworkTag>();

        if (tag != null)
        {
            tag.Owner = owner;
            AddTag(tag);
            return tag;
        }

        return null;
    }

    private void AddTag(NetworkTag tag)
    {
        if (tagCount + 1 > uint.MaxValue)
            CompressTags();

        if (tag.ID != 0) tags.Remove(tag.ID);
        tags.Add(++tagCount, tag);
        tag.ID = tagCount;
    }

    private void CompressTags()
    {
        tagCount = 1;
        NetworkTag[] array = new NetworkTag[tags.Values.Count];
        tags.Values.CopyTo(array, 0);
        tags.Clear();

        for (int i = 0; i < array.Length; i++)
        {
            NetworkTag current = array[i];
            NetworkSpawn spawn = spawns.Where(x => x.tag == current.ID).FirstOrDefault();
            AddTag(current);
            if (spawn != null) spawn.tag = current.ID;
        }
    }
    
    internal void HandleClientSpawn()
    {
        NetworkTag[] spawnTags = Object.FindObjectsOfType<NetworkTag>();
        List<NetworkSpawn> scenespawns = new List<NetworkSpawn>(spawns.Where(x => x.asset.bundle == "UnityEngine"));

        foreach (NetworkTag tag in spawnTags)
        {
            NetworkSpawn s = scenespawns.Where(x =>
            x.asset.path == tag.transform.name &&
            x.position == (tag.transform is RectTransform ? Vector3.zero : tag.transform.position) &&
            x.rotation == tag.transform.rotation).FirstOrDefault();

            if (s == null)
            {
                Object.Destroy(tag.gameObject);
            }

            else
            {
                SetTag(tag.gameObject, SteamPlayer.FromID(0), s.tag);
                scenespawns.Remove(s);
            }
        }

        foreach (NetworkSpawn spawn in spawns)
        {
            if (spawn.asset.bundle == "UnityEngine") continue;
            GameObject obj = Object.Instantiate(AssetBundleLoader.GetAssetFromBundle<GameObject>(spawn.asset), spawn.position, spawn.rotation);
            SetTag(obj, spawn.owner, spawn.tag);
        }
    }

    internal void SetTag(GameObject obj, SteamPlayer owner, uint tagid)
    {
        NetworkTag tag = obj.GetComponent<NetworkTag>();

        if (tag != null)
        {
            if (tag.ID != 0) tags.Remove(tag.ID);
            tag.ID = tagid;
            tag.Owner = owner;
            tags.Add(tagid, tag);
        }
    }

    internal void SendCall(NetworkTag tag, SteamPlayer target, byte index, BitStream stream, SendType sendType)
    {
        BitStream packet = new BitStream()
            .Write((byte)PacketType.Call)
            .Write(tag.ID)
            .Write(index)
            .Write(stream.GetBytes());

        Game.Instance.Steam.SendPacket(packet.GetBytes(), target, sendType);
    }

    internal void SendCall(NetworkTag tag, NetworkTarget target, byte index, BitStream stream, SendType sendType)
    {
        BitStream packet = new BitStream()
            .Write((byte)PacketType.Call)
            .Write(tag.ID)
            .Write(index)
            .Write(stream.GetBytes());

        Game.Instance.Steam.SendPacket(packet.GetBytes(), target, sendType);
    }

    internal void ReceiveCall(BitStream packet, SteamPlayer sender)
    {
        uint tag = packet.ReadUInt();
        byte index = packet.ReadByte();
        BitStream stream = new BitStream(packet.ReadBytes());
        if (tags.ContainsKey(tag)) tags[tag].HandleCall(index, sender, stream);
    }
}

internal class NetworkSpawn : INetworkObject
{
    public uint tag;
    public AssetObject asset;
    public Vector3 position;
    public Quaternion rotation;
    public SteamPlayer owner;

    public void Serialize(BitStream stream)
    {
        stream.Write(tag);
        stream.Write(asset);
        stream.Write(position);
        stream.Write(rotation);
        stream.Write((long)owner.ID);
    }

    public void Deserialize(BitStream stream)
    {
        tag = stream.ReadUInt();
        asset = stream.ReadNetworkObject<AssetObject>();
        position = stream.ReadVector3();
        rotation = stream.ReadQuaternion();
        owner = SteamPlayer.FromID((ulong)stream.ReadLong());
    }
}