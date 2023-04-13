using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Registries
{
    public static Registry<Block> BlockRegistry = new Registry<Block>();
    public static TextureAtlas TextureAtlas = new TextureAtlas();

    public static readonly Block AIR = BlockRegistry.Register(new Block("game:air", "Air", Block.TRANSPARENT));
    public static readonly Block STONE = BlockRegistry.Register(new Block("game:stone", "Stone", Block.OPAQUE));
    public static readonly Block DIRT = BlockRegistry.Register(new Block("game:dirt", "Dirt", Block.OPAQUE));
    public static readonly Block PLANKS = BlockRegistry.Register(new Block("game:planks", "Planks", Block.OPAQUE));
    public static readonly Block GLASS = BlockRegistry.Register(new Block("game:glass", "Glass", Block.TRANSPARENT));
    public static readonly Block WATER = BlockRegistry.Register(new Block("game:water", "Water", Block.TRANSPARENT));
}
