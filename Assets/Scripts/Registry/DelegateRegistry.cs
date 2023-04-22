using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DelegateRegistry<T> where T : Delegate
{
    private Dictionary<string, T> _registry = new Dictionary<string, T>();
    public T Register(string id, T item)
    {
        if(_registry.ContainsKey(id))
        {
            throw new System.Exception($"Tried to register already existing {typeof(T).Name} " + id);
        }
        _registry[id] = item;
        return item;
    }
    public T Get(string id)
    {
        if (!_registry.ContainsKey(id))
        {
            throw new System.Exception($"Tried to get non-existing {typeof(T).Name} " + id);
        }
        return _registry[id];
    }

    public IEnumerator GetEnumerator()
    {
        return _registry.Values.GetEnumerator();
    }

    public Dictionary<string, T>.ValueCollection GetValues()
    {
        return _registry.Values;
    }
}
