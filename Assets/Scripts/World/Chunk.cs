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

    ChunkRenderer[] renderers = new ChunkRenderer[Enum.GetValues(typeof(RenderLayer)).Length];

    bool shouldUpdate = false;
    bool initialized = false;

    // X, Y, Z
    Block[,,] blocks = new Block[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH];
    public void Init()
    {
        rootX = (int)transform.position.x;
        rootY = (int)transform.position.y;
        rootZ = (int)transform.position.z;

        chunkX = Mathf.FloorToInt(transform.position.x / (CHUNK_WIDTH * BLOCK_SIZE));
        chunkZ = Mathf.FloorToInt(transform.position.z / (CHUNK_WIDTH * BLOCK_SIZE));

        // Create our chunk renderers as child gameobjects
        int i = 0;
        foreach(RenderLayer layer in Enum.GetValues(typeof(RenderLayer)))
        {
            GameObject renderer = new GameObject(layer.ToString() + " Renderer");
            renderer.transform.SetParent(gameObject.transform);
            renderer.transform.localPosition = Vector3.zero;
            ChunkRenderer comp = renderer.AddComponent<ChunkRenderer>();
            comp.Init(this, layer);
            renderers[i++] = comp;
        }

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
    }
}
