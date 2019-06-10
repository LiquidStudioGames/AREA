using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[Serializable]
public class ReplacedComponent
{
    public string type;
    public string data;
}

[DisallowMultipleComponent]
public sealed class ComponentAssembler : MonoBehaviour
{
    public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { ContractResolver = new MyContractResolver() };

    public List<ReplacedComponent> components;

    private void Awake()
    {
        if (components != null)
        {
            foreach (ReplacedComponent rc in components)
            {
                Type t = Type.GetType(rc.type);
                Component c = gameObject.AddComponent(t);
                JsonConvert.PopulateObject(rc.data, c, JsonSettings);
            }
        }

        Destroy(this);
    }
}

public class MyContractResolver : DefaultContractResolver
{
    public static Type[] DontSerialize = new Type[] { typeof(MonoBehaviour), typeof(Behaviour), typeof(Component), typeof(UnityEngine.Object) };

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(x => !DontSerialize.Contains(x.DeclaringType))
                        .Select(f => base.CreateProperty(f, memberSerialization))
                        .ToList();

        props.ForEach(p => { p.Writable = true; p.Readable = true; });
        return props;
    }
}