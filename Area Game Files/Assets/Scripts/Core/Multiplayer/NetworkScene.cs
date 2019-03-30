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

    internal void AddSpawn(NetworkSpawn spawn)
    {
        spawns.Add(spawn);
    }

    internal void HandleServerSpawn()
    {
        NetworkTag[] spawnTags = Object.FindObjectsOfType<NetworkTag>();

        foreach (NetworkTag tag in spawnTags)
        {
            SteamPlayer owner = SteamPlayer.FromID(0);
            SetTag(tag, owner);

            spawns.Add(new NetworkSpawn()
            {
                asset = new AssetObject(tag.transform.name, "UnityEngine"),
                owner = owner,
                position = (tag.transform is RectTransform ? Vector3.zero : tag.transform.position),
                rotation = tag.transform.rotation,
                tags = new List<uint>() { tag.ID }
            });
        }
    }

    internal uint[] SetTags(GameObject obj, SteamPlayer owner)
    {
        NetworkTag[] tags = obj.GetComponentsInChildren<NetworkTag>();

        for (int i = 0; i < tags.Length; i++)
        {
            SetTag(tags[i], owner);
        }

        return tags.Select(x => x.ID).ToArray();
    }

    private void SetTag(NetworkTag tag, SteamPlayer owner)
    {
        tag.Owner = owner;
        AddTag(tag);
        tag.EnableTag();
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
            NetworkSpawn spawn = spawns.Where(x => x.tags.Contains(current.ID)).FirstOrDefault();
            spawn.tags.Remove(current.ID);
            AddTag(current);
            if (spawn != null) spawn.tags.Add(current.ID);
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
                SetTag(tag, SteamPlayer.FromID(0), s.tags[0]);
                scenespawns.Remove(s);
            }
        }

        foreach (NetworkSpawn spawn in spawns)
        {
            if (spawn.asset.bundle == "UnityEngine") continue;
            GameObject obj = Object.Instantiate(AssetBundleLoader.GetAssetFromBundle<GameObject>(spawn.asset), spawn.position, spawn.rotation);
            SetTags(obj, spawn.owner, spawn.tags.ToArray());
        }
    }

    internal void SetTags(GameObject obj, SteamPlayer owner, uint[] tagids)
    {
        NetworkTag[] tags = obj.GetComponentsInChildren<NetworkTag>();
        if (tags.Length != tagids.Length) throw new System.Exception("Something went wrong.");

        for (int i = 0; i < tags.Length; i++)
        {
            SetTag(tags[i], owner, tagids[i]);
        }
    }

    private void SetTag(NetworkTag tag, SteamPlayer owner, uint tagid)
    {
        if (tag.ID != 0) tags.Remove(tag.ID);
        tag.ID = tagid;
        tag.Owner = owner;
        tags.Add(tagid, tag);
        tag.EnableTag();
    }

    internal void RemoveTag(NetworkTag tag)
    {
        tags.Remove(tag.ID);

        for (int i = 0; i < spawns.Count; i++)
        {
            if (spawns[i].tags.Contains(tag.ID))
            {
                spawns[i].tags.Remove(tag.ID);
                if (spawns[i].tags.Count == 0) spawns.RemoveAt(i--);
            }
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
    public List<uint> tags;
    public AssetObject asset;
    public Vector3 position;
    public Quaternion rotation;
    public SteamPlayer owner;

    public void Serialize(BitStream stream)
    {
        stream.Write(tags.ToArray());
        stream.Write(asset);
        stream.Write(position);
        stream.Write(rotation);
        stream.Write((long)owner.ID);
    }

    public void Deserialize(BitStream stream)
    {
        tags = new List<uint>(stream.ReadUInts());
        asset = stream.ReadNetworkObject<AssetObject>();
        position = stream.ReadVector3();
        rotation = stream.ReadQuaternion();
        owner = SteamPlayer.FromID((ulong)stream.ReadLong());
    }
}