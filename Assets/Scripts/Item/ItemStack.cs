using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class ItemStack
{ 
    public static readonly ItemStack EMPTY = new ItemStack(null);
    Item _item;
    public Item Item { get => _item; }
    int _count { get; set; }
    public ItemStack(Item item)
    {
        _item = item;
        _count = 1;
    }
    public ItemStack(Item item, int count)
    {
        _item = item;
        _count = count;
    }

    public UseResult Use(Entity entity, out ItemStack stack)
    {
        UseResult result = _item.Use(entity);
        // Consume item if we used successfully
        if(result == UseResult.Used && _item.Consumable)
        {
            _count--;
        }
        // Output empty if we're out of items, otherwise return self
        if(_count <= 0)
        { 
            stack = ItemStack.EMPTY; 
        }
        else
        {
            stack = this;
        }
        return result;
    }

    public override string ToString()
    {
        return this == EMPTY ? "Empty" : $"{_count}x {_item}";
    }
}
