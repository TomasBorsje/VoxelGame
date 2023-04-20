using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class ItemStack
{ 
    public static readonly ItemStack EMPTY = new ItemStack(new EmptyItem());
    Item _item;
    public Item Item { get => _item; }
    int _count;
    public int Count { get => _count; }
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

    public UseResult Use(LivingEntity entity, out ItemStack stack)
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
    // Takes count items from this stack and returns them as a new stack
    public ItemStack TakeAmount(int count)
    {
        if(count < 1)
        {
            return ItemStack.EMPTY;
        }
        _count -= count;
        return new ItemStack(_item, count);
    }

    public override string ToString()
    {
        return this == EMPTY ? "Empty" : $"{_count}x {_item}";
    }
    // Merges another stack into this one, returning the remainder (or the stack unchanged if not possible).
    public ItemStack Merge(ItemStack stackToMerge)
    {
        // If these are not the same items, or this stack is full, don't merge.
        if (stackToMerge == ItemStack.EMPTY 
            || stackToMerge._item.Id != _item.Id
            || _count >= _item.MaxStackSize)
        {
            return stackToMerge;
        }
        // Otherwise, increase our stack count and decrease the remainder stack count
        int moveAmount = Mathf.Min(stackToMerge._count, _item.MaxStackSize - _count);
        _count += moveAmount;
        return stackToMerge.ChangeCount(-moveAmount);
    }

    public ItemStack ChangeCount(int change)
    {
        _count += change;
        if(_count <= 0)
        {
            return ItemStack.EMPTY;
        }
        return this;
    }
}
