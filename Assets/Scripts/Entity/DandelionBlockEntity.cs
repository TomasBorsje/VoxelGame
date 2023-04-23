using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionBlockEntity : BlockEntity
{
    public DandelionBlockEntity(Chunk parentChunk, Vector3Int chunkPos) : base(parentChunk, chunkPos)
    {
    }

    public override void Tick()
    {
        parentChunk.SetBlock(chunkPos.x, chunkPos.y + 1, chunkPos.z, BlockRegistry.GLASS);
    }
}
