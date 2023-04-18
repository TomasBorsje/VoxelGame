using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionBlock : Block
{
    public DandelionBlock() : base("game:dandelion", "Dandelion", ChunkRenderer.RenderLayer.Transparent, true) { }
    public override void ApplyCustomModel(List<Vector3> vertices, List<Vector2> uvs, List<int> tris, Vector3 blockPos)
    {
        MeshUtils.AddCrossMesh(this, vertices, uvs, tris, blockPos);
    }
}
