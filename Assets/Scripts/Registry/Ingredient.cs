using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Ingredient
{
    public readonly Item item;
    public readonly int count;
    public Ingredient(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }
    public Ingredient(Item item)
    {
        this.item = item;
        this.count = 1;
    }
    public bool SatisfiedBy(ItemStack stack)
    {
        return stack.Item.Id == this.item.Id && stack.Count >= this.count;
    }
}
