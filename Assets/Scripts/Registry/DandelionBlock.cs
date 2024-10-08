using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionBlock : Block
{
    private readonly static Vector3 selectionPos = new Vector3(0.3f,0.55f,0.3f);
    private readonly static Vector3 selectionOffset = new Vector3(0.35f, 0, 0.35f);
    public DandelionBlock() : base("game:dandelion", "Dandelion", ChunkRenderer.RenderLayer.Model, hasCustomModel: true, hasCustomCollider: true, hasCustomSelectionCollider: true) {
        _hasBlockEntity = true;
    }
    public override void ApplyCustomModel(List<Vector3> vertices, List<Vector2> uvs, List<int> tris, Vector3 blockPos)
    {
        MeshUtils.AddCrossMesh(this, vertices, uvs, tris, blockPos);
    }
    public override void AddSelectionCollider(List<Vector3> vertices, List<int> tris, Vector3 blockPos)
    {
        MeshUtils.AddSizedCubeMesh(vertices, tris, blockPos, selectionPos, selectionOffset);
    }
    internal override void Tick(Vector3Int worldPos)
    {
        WorldGenHandler.INSTANCE.TryGenerateBlock(BlockRegistry.LEAVES, worldPos + Vector3Int.up);
    }
    public override BlockEntity GetNewBlockEntity(Chunk chunk, Vector3Int pos)
    {
        return new DandelionBlockEntity(chunk, pos);
    }
}
