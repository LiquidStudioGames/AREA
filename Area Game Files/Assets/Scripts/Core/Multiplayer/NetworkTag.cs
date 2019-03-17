using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public delegate void NetworkCall(BitStream stream, SteamPlayer sender);

[AttributeUsage(AttributeTargets.Method)]
public class NetworkCallAttribute : Attribute { }

public enum PacketType : byte
{
    Connect = 1,
    Disconnect = 2,
    Level = 3,
    Proxy = 4,
    ProxyTarget = 5,
    Spawns = 6,
    Call = 10
}

public class NetworkTag : MonoBehaviour
{
    public uint ID;
    public SteamPlayer Owner;

    internal SortedList<string, NetworkCall> calls;

    public bool IsMine
    {
        get
        {
            if (Game.Instance.IsClient) return Owner == Game.Instance.Steam.Player;
            return Owner.ID == 0;
        }
    }

    private void Awake()
    {
        calls = new SortedList<string, NetworkCall>();

        foreach (Component component in GetComponents<Component>())
        {
            ParseComponent(component);
        }
    }

    private void OnDestroy()
    {
        calls.Clear();
    }

    private void ParseComponent(Component component)
    {
        foreach (MethodInfo info in component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            NetworkCallAttribute att = (NetworkCallAttribute)info.GetCustomAttribute(typeof(NetworkCallAttribute), true);

            if (att != null)
            {
                try
                {
                    NetworkCall call = (NetworkCall)Delegate.CreateDelegate(typeof(NetworkCall), component, info);
                    if (calls.ContainsKey(info.Name)) Debug.LogWarning("Duplicate callback for: " + info.Name);
                    else calls.Add(info.Name, call);
                }

                catch (Exception e)
                {
                    throw new Exception("Creating call failed for " + info.Name, e);
                }
            }
        }
    }

    public T AddComponent<T>() where T : Component
    {
        T behaviour = gameObject.AddComponent<T>();
        ParseComponent(behaviour);
        return behaviour;
    }

    internal void HandleCall(byte index, SteamPlayer sender, BitStream stream)
    {
        if (calls.Values.Count > index)
        {
            NetworkCall call = calls.Values[index];
            call(stream, sender);
        }

        else Debug.LogError($"Command {index} not found.");
    }

    public void Call(NetworkCall call, SteamPlayer target, BitStream stream, SendType sendtype = SendType.Unreliable)
    {
        if (!calls.ContainsKey(call.Method.Name)) throw new ArgumentException($"{call.Method.Name} is not registered, check if the method has a NetworkCall Attribute.");
        Game.Instance.NetworkScene.SendCall(this, target, (byte)calls.IndexOfKey(call.Method.Name), stream, sendtype);
    }

    public void Call(NetworkCall call, NetworkTarget target, BitStream stream, SendType sendtype = SendType.Unreliable)
    {
        if (!calls.ContainsKey(call.Method.Name)) throw new ArgumentException($"{call.Method.Name} is not registered, check if the method has a NetworkCall Attribute.");
        Game.Instance.NetworkScene.SendCall(this, target, (byte)calls.IndexOfKey(call.Method.Name), stream, sendtype);
    }
}