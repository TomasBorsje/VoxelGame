using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public enum UseResult { Used, Pass }

    bool _consumable = true;
    public bool Consumable { get => _consumable; }
    public virtual UseResult Use(Entity entity)
    {
        return UseResult.Used;
    }
}
