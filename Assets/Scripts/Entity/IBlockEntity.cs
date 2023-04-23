using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEntity
{
    protected Chunk parentChunk;
    protected Vector3Int chunkPos;
    public BlockEntity(Chunk parentChunk, Vector3Int chunkPos)
    {
        this.parentChunk = parentChunk;
        this.chunkPos = chunkPos;
    }
    public virtual void Tick() { }
    public virtual void OnDelete() { }
}
