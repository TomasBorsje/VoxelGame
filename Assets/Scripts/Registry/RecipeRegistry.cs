using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RecipeRegistry
{
    public static Registry<CraftingRecipe> Recipes = new Registry<CraftingRecipe>();

    // Uses recipe ids, which can be anything
    public static readonly CraftingRecipe LOG_TO_PLANKS = Recipes.Register(new CraftingRecipe("game:log_to_planks", new Ingredient(ItemRegistry.LOG()), output: ItemRegistry.PLANKS(), outputCount: 4));
    public static readonly CraftingRecipe STONE_TO_GLASS = Recipes.Register(new CraftingRecipe("game:stone_to_glass", new Ingredient(ItemRegistry.STONE()), output: ItemRegistry.GLASS(), outputCount: 2));


    public static List<CraftingRecipe> GetCraftableRecipes(ItemContainer inventory)
    {
        List<CraftingRecipe> craftables = new();

        foreach(CraftingRecipe recipe in Recipes.GetValues())
        {
            if(recipe.CanCraft(inventory))
            {
                craftables.Add(recipe);
            }
        }

        return craftables;
    }
}
