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
        for(int i = 0; i < _itemStacks.Length; i++)
        {
            _itemStacks[i] = ItemStack.EMPTY;
        }
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
    public ItemStack[] GetStacks()
    {
        return _itemStacks;
    }
    // Adds an itemstack to an inventory, merging stacks along the way. Returns the remainder.
    public ItemStack AddStack(ItemStack stack)
    {
        ItemStack remainder = stack;

        // Try and merge with non empty stacks first
        for(int i = 0; i < _size; i++)
        {
            // If empty slot, move to there.
            if (_itemStacks[i] != ItemStack.EMPTY)
            {
                // Try to merge and continue with any remainder
                remainder = _itemStacks[i].Merge(remainder);
                if (remainder == ItemStack.EMPTY)
                {
                    return ItemStack.EMPTY;
                }
            }
        }

        // Now try to replace any empty stacks
        for(int i = 0; i < _size; i++)
        {
            if(_itemStacks[i] == ItemStack.EMPTY)
            {
                _itemStacks[i] = remainder;
                return ItemStack.EMPTY;
            }
        }
        return remainder;
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
