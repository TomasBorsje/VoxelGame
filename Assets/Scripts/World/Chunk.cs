using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static ChunkRenderer;

public class Chunk : MonoBehaviour
{
    public static int BLOCK_SIZE = 1;
    public static int CHUNK_WIDTH = 16;
    public static int CHUNK_HEIGHT = 32;
    public static float PERLIN_SCALE = 0.1f;
    public static readonly int FLOOR_LEVEL = 16;
    public static readonly int SEA_LEVEL = 20;

    int rootX, rootY, rootZ;
    int chunkX, chunkZ;

    ChunkRenderer[] renderers = new ChunkRenderer[Enum.GetNames(typeof(RenderLayer)).Length];

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;
    bool shouldUpdate = false;
    bool initialized = false;

    // X, Y, Z
    Block[,,] blocks = new Block[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH];
    public void Init()
    {
        mesh = new Mesh();
        mesh.name = "ChunkMesh";
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Registries.TextureAtlas.AtlasMaterial;
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        rootX = (int)transform.position.x;
        rootY = (int)transform.position.y;
        rootZ = (int)transform.position.z;

        chunkX = Mathf.FloorToInt(transform.position.x / (CHUNK_WIDTH * BLOCK_SIZE));
        chunkZ = Mathf.FloorToInt(transform.position.z / (CHUNK_WIDTH * BLOCK_SIZE));

        // TODO: World gen function
        // Init to air
        for (int localX = 0; localX < CHUNK_WIDTH; localX++)
        {
            for (int localZ = 0; localZ < CHUNK_WIDTH; localZ++)
            {
                for (int localY = 0; localY < CHUNK_HEIGHT; localY++)
                {
                    blocks[localX, localY, localZ] = Registries.AIR;
                }
            }
        }

        for (int localX = 0; localX < CHUNK_WIDTH; localX++)
        {
            for (int localZ = 0; localZ < CHUNK_WIDTH; localZ++)
            {
                bool isDirt = Mathf.PerlinNoise(((rootX + localX) * 5) * 0.1f, ((rootZ + localZ)*5) * 0.1f) > 0.5f;
                for (int localY = 0; localY < CHUNK_HEIGHT; localY++)
                {
                    if(localY < FLOOR_LEVEL + (CHUNK_HEIGHT-FLOOR_LEVEL) * Mathf.PerlinNoise((rootX + localX) * PERLIN_SCALE, (rootZ + localZ) * PERLIN_SCALE) * 0.6f)
                    {
                        blocks[localX, localY, localZ] = isDirt ? Registries.DIRT : Registries.STONE;
                    }
                }
            }
        }

        for (int localX = 0; localX < CHUNK_WIDTH; localX++)
        {
            for (int localZ = 0; localZ < CHUNK_WIDTH; localZ++)
            {
                for (int localY = 0; localY < SEA_LEVEL; localY++)
                {
                    if (blocks[localX, localY, localZ].Empty) { blocks[localX, localY, localZ] = Registries.WATER; }
                }
            }
        }
        initialized = true;
        RenderMesh();
    }
    
    public Vector3Int GetChunkCoords()
    {
        return new Vector3Int(chunkX, 0, chunkZ);
    }
    public Block[,,] GetBlocks()
    {
        return blocks;
    }

    public Block GetBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        blocks[x, y, z] = block;
        MarkUpdate();
        WorldGenHandler.INSTANCE.ReRenderNeighbours(chunkX, chunkZ);
    }

    public void MarkUpdate()
    {
        shouldUpdate = true;
    }

    void Update()
    {
        if (initialized && shouldUpdate) { RenderMesh(); }
    }

    void RenderMesh()
    {
        foreach(ChunkRenderer renderer in renderers)
        {
            renderer.RenderChunk();
        }
        shouldUpdate = false;

        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < CHUNK_WIDTH; x++)
        {
            for (int z = 0; z < CHUNK_WIDTH; z++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    Block block = blocks[x, y, z];
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
                                if (neighbour.GetBlock(x, y, CHUNK_WIDTH-1).Empty || (!block.Transparent && neighbour.GetBlock(x, y, CHUNK_WIDTH - 1).Transparent))
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
                                if (neighbour.GetBlock(CHUNK_WIDTH-1, y, z).Empty || (!block.Transparent && neighbour.GetBlock(CHUNK_WIDTH - 1, y, z).Transparent))
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

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = mesh;

        shouldUpdate = false;
    }
}
