using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Chunk;
public class ChunkRenderer : MonoBehaviour
{
    public enum RenderLayer { Opaque, Transparent, Water, Model }

    Chunk chunk;
    RenderLayer layer;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;

    // Sets up the components, etc. for this renderlayer
    public void Init(Chunk chunk, RenderLayer type)
    {
        this.chunk = chunk;
        this.layer = type;
        mesh = new Mesh();
        mesh.name = "ChunkMesh-"+type.ToString();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Registries.TextureAtlas.GetAtlasMaterial(type);
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
    public void RenderChunk()
    {
        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        Block[,,] blocks = chunk.GetBlocks();

        Vector3Int coords = chunk.GetChunkCoords();
        int chunkX = coords.x;
        int chunkZ = coords.z;

        for (int x = 0; x < CHUNK_WIDTH; x++)
        {
            for (int z = 0; z < CHUNK_WIDTH; z++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    Block block = blocks[x, y, z];
                    // Only render our selected type!
                    if(block.RenderLayer != layer) { continue; }

                    Vector3 blockPos = new Vector3(x, y, z);
                    // Render a block
                    if (block != Registries.AIR)
                    {
                        // Top
                        if (y == CHUNK_HEIGHT - 1 || blocks[x, y + 1, z].Empty || (!block.Transparent && blocks[x, y + 1, z].Transparent))
                        {
                            MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.Top);
                        }
                        // Bottom
                        if (y == 0 || blocks[x, y - 1, z].Empty || (!block.Transparent && blocks[x, y - 1, z].Transparent))
                        {
                            MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.Bottom);
                        }
                        // North
                        if (z == CHUNK_WIDTH - 1)
                        {
                            if (WorldGenHandler.INSTANCE.ChunkLoaded(chunkX, chunkZ + 1))
                            {
                                Chunk neighbour = WorldGenHandler.INSTANCE.GetChunk(chunkX, chunkZ + 1);
                                if (neighbour.GetBlock(x, y, 0).Empty || (!block.Transparent && neighbour.GetBlock(x, y, 0).Transparent))
                                {
                                    MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.North);
                                }
                            }
                        }
                        else if (blocks[x, y, z + 1].Empty || (!block.Transparent && blocks[x, y, z + 1].Transparent))
                        {
                            MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.North);
                        }
                        // South
                        if (z == 0)
                        {
                            if (WorldGenHandler.INSTANCE.ChunkLoaded(chunkX, chunkZ - 1))
                            {
                                Chunk neighbour = WorldGenHandler.INSTANCE.GetChunk(chunkX, chunkZ - 1);
                                if (neighbour.GetBlock(x, y, CHUNK_WIDTH - 1).Empty || (!block.Transparent && neighbour.GetBlock(x, y, CHUNK_WIDTH - 1).Transparent))
                                {
                                    MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.South);
                                }
                            }
                        }
                        else if (blocks[x, y, z - 1].Empty || (!block.Transparent && blocks[x, y, z - 1].Transparent))
                        {
                            MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.South);
                        }
                        // East
                        if (x == CHUNK_WIDTH - 1)
                        {
                            if (WorldGenHandler.INSTANCE.ChunkLoaded(chunkX + 1, chunkZ))
                            {
                                Chunk neighbour = WorldGenHandler.INSTANCE.GetChunk(chunkX + 1, chunkZ);
                                if (neighbour.GetBlock(0, y, z).Empty || (!block.Transparent && neighbour.GetBlock(0, y, z).Transparent))
                                {
                                    MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.East);
                                }
                            }
                        }
                        else if (blocks[x + 1, y, z].Empty || (!block.Transparent && blocks[x + 1, y, z].Transparent))
                        {
                            MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.East);
                        }
                        // West
                        if (x == 0)
                        {
                            if (WorldGenHandler.INSTANCE.ChunkLoaded(chunkX - 1, chunkZ))
                            {
                                Chunk neighbour = WorldGenHandler.INSTANCE.GetChunk(chunkX - 1, chunkZ);
                                if (neighbour.GetBlock(CHUNK_WIDTH - 1, y, z).Empty || (!block.Transparent && neighbour.GetBlock(CHUNK_WIDTH - 1, y, z).Transparent))
                                {
                                    MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.West);
                                }
                            }
                        }
                        else if (blocks[x - 1, y, z].Empty || (!block.Transparent && blocks[x - 1, y, z].Transparent))
                        {
                            MeshUtils.AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, MeshUtils.FaceDirection.West);
                        }
                    }
                }
            }
        }

        if (vertices.Count > 0)
        {
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            meshCollider.sharedMesh = mesh;
        }
    }
}
