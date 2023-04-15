using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockItem : Item
{
    Block _block;
    public BlockItem(Block block)
    {
        _block = block;
        _id = _block.Id;
    }
    public override UseResult Use(Entity entity)
    {
        RaycastHit hit;
        Transform head = entity.GetHeadTransform();
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(head.position, head.TransformDirection(Vector3.forward), out hit, entity is Player ? ((Player)entity).reachDistance : 5))
        {
            Debug.DrawRay(head.position, head.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            Vector3 backwardHit = hit.point - head.TransformDirection(Vector3.forward) * 0.001f;
            Debug.Log($"Did Hit at {backwardHit.x},{backwardHit.y},{backwardHit.z}");
            (Chunk, Vector3Int) chunkPos = WorldGenHandler.INSTANCE.WorldPosToChunkPos(backwardHit);
            chunkPos.Item1.SetBlock(chunkPos.Item2.x, chunkPos.Item2.y, chunkPos.Item2.z, _block);
            return UseResult.Used;
        }
        return UseResult.Pass;
    }
    public override string ToString()
    {
        return _block.Id;
    }
}
