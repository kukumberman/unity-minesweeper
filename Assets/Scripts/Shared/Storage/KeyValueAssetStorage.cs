using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public sealed class KeyValueEntry<T>
{
    public string Key;
    public T Value;
}

public abstract class KeyValueAssetStorage : ScriptableObject
{
    public abstract int Count();

    public abstract string KeyAt(int index);

    public abstract object ValueAt(int index);

    public abstract void SetKeyAt(int index, string key);

    public abstract void SetValueAt(int index, object value);
}

public class KeyValueAssetStorage<T> : KeyValueAssetStorage
    where T : Object
{
    public List<KeyValueEntry<T>> Items;

    private Dictionary<string, T> _map;

    public override int Count()
    {
        return Items.Count;
    }

    public void Initialize()
    {
        _map = Items.ToDictionary(item => item.Key, item => item.Value);
    }

    public void Dispose()
    {
        _map.Clear();
        _map.TrimExcess();
        _map = null;
    }

    public T Get(string key)
    {
        if (TryGet(key, out var value))
        {
            return value;
        }

        return null;
    }

    public bool TryGet(string key, out T value)
    {
        return _map.TryGetValue(key, out value);
    }

    public override string KeyAt(int index)
    {
        return Items[index].Key;
    }

    public override object ValueAt(int index)
    {
        return Items[index].Value;
    }

    public override void SetKeyAt(int index, string key)
    {
        Items[index].Key = key;
    }

    public override void SetValueAt(int index, object value)
    {
        if (value is T instance)
        {
            Items[index].Value = instance;
        }
        else
        {
            throw new ArgumentException();
        }
    }
}
