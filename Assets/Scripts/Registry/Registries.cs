using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkRenderer;

public static class Registries
{
    public static Registry<Block> BlockRegistry = new Registry<Block>();
    public static TextureAtlas TextureAtlas = new TextureAtlas();

    public static readonly Block AIR = BlockRegistry.Register(new Block("game:air", "Air", RenderLayer.Transparent));
    public static readonly Block STONE = BlockRegistry.Register(new Block("game:stone", "Stone", RenderLayer.Opaque));
    public static readonly Block DIRT = BlockRegistry.Register(new Block("game:dirt", "Dirt", RenderLayer.Opaque));
    public static readonly Block PLANKS = BlockRegistry.Register(new Block("game:planks", "Planks", RenderLayer.Opaque));
    public static readonly Block GLASS = BlockRegistry.Register(new Block("game:glass", "Glass", RenderLayer.Transparent));
    public static readonly Block WATER = BlockRegistry.Register(new Block("game:water", "Water", RenderLayer.Water));
}
