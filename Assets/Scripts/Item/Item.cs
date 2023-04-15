using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : IHasId
{
    public enum UseResult { Used, Pass }

    bool _consumable = true;
    protected string _id = "";
    int _maxStackSize = 100;
    public bool Consumable { get => _consumable; }
    public string Id { get => _id; }
    public int MaxStackSize { get => _maxStackSize; }

    public virtual UseResult Use(Entity entity)
    {
        return UseResult.Used;
    }
}
