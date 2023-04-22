using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : IHasId
{
    public enum UseResult { Used, Pass }

    protected bool _consumable = true;
    protected string _id = "";
    protected int _maxStackSize = 100;
    public bool Consumable { get => _consumable; }
    public string Id { get => _id; }
    public int MaxStackSize { get => _maxStackSize; }

    public virtual UseResult Use(LivingEntity entity)
    {
        return UseResult.Used;
    }
}

public class EmptyItem : Item
{
    public EmptyItem()
    {
        _id = "game:empty";
    }
}