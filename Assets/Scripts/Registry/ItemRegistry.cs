using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRegistry
{
    public static DelegateRegistry<Func<Item>> ITEMS = new DelegateRegistry<Func<Item>>();

    public static readonly Func<Item> AIR = ITEMS.Register("game:air", () => { return new BlockItem(BlockRegistry.AIR); });
    public static readonly Func<Item> STONE = ITEMS.Register("game:stone", () => { return new BlockItem(BlockRegistry.STONE); });
    public static readonly Func<Item> GRASS = ITEMS.Register("game:grass", () => { return new BlockItem(BlockRegistry.GRASS); });
    public static readonly Func<Item> DIRT = ITEMS.Register("game:dirt", () => { return new BlockItem(BlockRegistry.DIRT); });
    public static readonly Func<Item> PLANKS = ITEMS.Register("game:planks", () => { return new BlockItem(BlockRegistry.PLANKS); });
    public static readonly Func<Item> GLASS = ITEMS.Register("game:glass", () => { return new BlockItem(BlockRegistry.GLASS); });
    public static readonly Func<Item> WATER = ITEMS.Register("game:water", () => { return new BlockItem(BlockRegistry.WATER); });
    public static readonly Func<Item> LOG = ITEMS.Register("game:log", () => { return new BlockItem(BlockRegistry.LOG); });
    public static readonly Func<Item> LEAVES = ITEMS.Register("game:leaves", () => { return new BlockItem(BlockRegistry.LEAVES); });

    public static readonly Func<Item> DANDELION = ITEMS.Register("game:dandelion", () => { return new BlockItem(BlockRegistry.DANDELION); });
}
