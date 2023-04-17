using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Registry<T> : IEnumerable where T : IHasId
{
    private Dictionary<string, T> _registry = new Dictionary<string, T>();
    public T Register(T item)
    {
        if(_registry.ContainsKey(item.Id))
        {
            throw new System.Exception($"Tried to register already existing {typeof(T).Name} " + item.Id);
        }
        _registry[item.Id] = item;
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
}
