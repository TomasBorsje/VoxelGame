using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipe : IHasId
{
    public readonly IList<Ingredient> inputs;
    public readonly Item output;
    public readonly int outputCount;
    string id;
    public CraftingRecipe(string id, IList<Ingredient> inputs, Item output, int outputCount = 1)
    {
        this.id = id;
        this.inputs = inputs;
        this.output = output;
        this.outputCount = outputCount;
    }

    public CraftingRecipe(string id, Ingredient input, Item output, int outputCount = 1)
    {
        this.id = id;
        this.inputs = new List<Ingredient>() { input };
        this.output = output;
        this.outputCount = outputCount;
    }

    public string Id => id;

    public bool CanCraft(ItemContainer inventory)
    {
        foreach(Ingredient ingredient in inputs)
        {
            bool ingredientSatisfied = false;
            foreach (ItemStack stack in inventory.GetStacks())
            {
                if(ingredient.SatisfiedBy(stack))
                {
                    ingredientSatisfied = true;
                    break;
                }
            }
            if(!ingredientSatisfied)
            {
                return false;
            }
        }
        return true;
    }
    // Returns ItemStack.EMPTY if not craftable
    public ItemStack TryCraft(ItemContainer inventory)
    {
        if(!CanCraft(inventory))
        {
            return ItemStack.EMPTY;
        }

        // We can craft it. Reduce stacks and return crafted stack
        ItemStack[] invStacks = inventory.GetStacks();
        foreach (Ingredient ingredient in inputs)
        {
            for(int i = 0; i < invStacks.Length; i++)
            {
                if (ingredient.SatisfiedBy(invStacks[i]))
                {
                    invStacks[i] = invStacks[i].ChangeCount(-ingredient.count);
                }
            }
        }

        return GetOutput();
    }

    public ItemStack GetOutput()
    {
        return new ItemStack(output, outputCount);
    }
}
