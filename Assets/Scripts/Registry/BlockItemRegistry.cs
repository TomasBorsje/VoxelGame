using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockItemRegistry
{
    public static DelegateRegistry<Func<BlockItem>> ITEMS = new DelegateRegistry<Func<BlockItem>>();

    public static readonly Func<BlockItem> AIR = ITEMS.Register("game:air", () => { return new BlockItem(BlockRegistry.AIR); });
    public static readonly Func<BlockItem> STONE = ITEMS.Register("game:stone", () => { return new BlockItem(BlockRegistry.STONE); });
    public static readonly Func<BlockItem> DIRT = ITEMS.Register("game:dirt", () => { return new BlockItem(BlockRegistry.DIRT); });
    public static readonly Func<BlockItem> PLANKS = ITEMS.Register("game:planks", () => { return new BlockItem(BlockRegistry.PLANKS); });
    public static readonly Func<BlockItem> GLASS = ITEMS.Register("game:glass", () => { return new BlockItem(BlockRegistry.GLASS); });
    public static readonly Func<BlockItem> WATER = ITEMS.Register("game:water", () => { return new BlockItem(BlockRegistry.WATER); });
}
