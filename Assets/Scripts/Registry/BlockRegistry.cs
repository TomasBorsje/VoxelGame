using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkRenderer;

public static class BlockRegistry
{
    public static Registry<Block> Blocks = new Registry<Block>();

    public static readonly Block AIR = Blocks.Register(new Block("game:air", "Air", RenderLayer.Transparent));
    public static readonly Block STONE = Blocks.Register(new Block("game:stone", "Stone", RenderLayer.Opaque));
    public static readonly Block GRASS = Blocks.Register(new Block("game:grass", "Grass", RenderLayer.Opaque));
    public static readonly Block DIRT = Blocks.Register(new Block("game:dirt", "Dirt", RenderLayer.Opaque));
    public static readonly Block PLANKS = Blocks.Register(new Block("game:planks", "Planks", RenderLayer.Opaque));
    public static readonly Block GLASS = Blocks.Register(new Block("game:glass", "Glass", RenderLayer.Transparent));
    public static readonly Block WATER = Blocks.Register(new Block("game:water", "Water", RenderLayer.Water, hasCustomCollider: true));
    public static readonly Block LOG = Blocks.Register(new Block("game:log", "Log", RenderLayer.Opaque));
    public static readonly Block LEAVES = Blocks.Register(new Block("game:leaves", "Leaves", RenderLayer.Leaves));

    public static readonly Block DANDELION = Blocks.Register(new DandelionBlock());
}
