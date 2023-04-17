using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Chunk;
public abstract class ChunkRenderer : MonoBehaviour
{
    public enum RenderLayer { Opaque, Transparent, Water, Leaves, Model }

    protected Chunk chunk;
    protected RenderLayer layer;
    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;
    protected Mesh mesh;

    // Sets up the components, etc. for this renderlayer
    public void Init(Chunk chunk, RenderLayer type)
    {
        this.chunk = chunk;
        this.layer = type;
        mesh = new Mesh();
        mesh.name = "ChunkMesh-" + type.ToString();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = TextureAtlas.Instance.GetAtlasMaterial(this.layer);
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        if (layer == RenderLayer.Transparent)
        {
            meshRenderer.material.SetFloat("_AlphaCutoffEnable", 1.0f);
        }
    }
    public abstract void RenderChunk();
}
