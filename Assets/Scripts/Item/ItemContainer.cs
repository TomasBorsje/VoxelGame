using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ItemContainer
{
    int _size;
    ItemStack[] _itemStacks;
    public ItemContainer(int size)
    {
        _size = size;
        _itemStacks = new ItemStack[_size];
        Array.Fill(_itemStacks, ItemStack.EMPTY);
    }
    public int Size { get => _size; }
    public void SetStackInSlot(int slot, ItemStack stack)
    {
        _itemStacks[slot] = stack;
    }
    public ItemStack GetStackInSlot(int slot)
    {
        return _itemStacks[slot];
    }
    public ItemStack AddStack()
    {
        // Todo: merging and returning leftover itemstack etc.
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        string output = "{";
        for (int i = 0; i < _itemStacks.Length; i++)
        {
            output += _itemStacks[i].ToString();
            if (i < _itemStacks.Length - 1) { output += ", "; }
        }
        output += "}";
        return output;
    }
}
