using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Chunk;
public abstract class ChunkRenderer
{
    public enum RenderLayer { Opaque, Transparent, Water, Leaves, Model }

    protected Chunk chunk;
    public RenderLayer layer;

    // Sets up the components, etc. for this renderlayer
    public void Init(Chunk chunk, RenderLayer layer)
    {
        this.chunk = chunk;
        this.layer = layer;
    }
    public abstract Mesh RenderChunk();
}
