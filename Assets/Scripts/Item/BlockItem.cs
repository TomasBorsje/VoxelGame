using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockItem : Item
{
    private static int RaycastMask;
    Block _block;
    public Block Block { get => _block; }
    public BlockItem(Block block)
    {
        _block = block;
        _id = _block.Id;
    }
    public override UseResult Use(LivingEntity entity)
    {
        // init raycast mask if not init
        if(RaycastMask==0)
        {
            RaycastMask = LayerMask.GetMask(new string[] { "Chunk", "SelectionRaycast" });
        }
        RaycastHit hit;
        Transform head = entity.GetHeadTransform();
        if (Physics.Raycast(head.position, head.TransformDirection(Vector3.forward), out hit, entity is Player ? ((Player)entity).reachDistance : 5, layerMask: RaycastMask))
        {
            Debug.DrawRay(head.position, head.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            Vector3 backwardHit = hit.point - head.TransformDirection(Vector3.forward) * 0.001f;
            (Chunk, Vector3Int) chunkPos = WorldGenHandler.INSTANCE.WorldPosToChunkPos(backwardHit);
            if (chunkPos.Item1.GetBlock(chunkPos.Item2.x, chunkPos.Item2.y, chunkPos.Item2.z).Empty)
            {
                chunkPos.Item1.SetBlock(chunkPos.Item2.x, chunkPos.Item2.y, chunkPos.Item2.z, _block);
                return UseResult.Used;
            }
        }
        return UseResult.Pass;
    }
    public override string ToString()
    {
        return _block.Id;
    }
}
