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
            ChunkRenderer comp = null;
            // todo: rename leaves renderer to wavy
            switch (layer) {
                case RenderLayer.Opaque: { comp = renderer.AddComponent<OpaqueChunkRenderer>(); break; }
                case RenderLayer.Water: { comp = renderer.AddComponent<WaterChunkRenderer>(); break; }
                case RenderLayer.Leaves: { comp = renderer.AddComponent<LeavesChunkRenderer>(); break; }
                case RenderLayer.Transparent: { comp = renderer.AddComponent<TransparentChunkRenderer>(); break; }
                default: { comp = renderer.AddComponent<OpaqueChunkRenderer>(); break; }
            }
            // todo: remove layer arg
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
                    blocks[localX, localY, localZ] = BlockRegistry.AIR;
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
                        blocks[localX, localY, localZ] = isDirt ? BlockRegistry.DIRT : BlockRegistry.STONE;
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
                    if (blocks[localX, localY, localZ].Empty) { blocks[localX, localY, localZ] = BlockRegistry.WATER; }
                }
            }
        }

        // Tree gens
        for (int localX = 0; localX < CHUNK_WIDTH; localX++)
        {
            for (int localZ = 0; localZ < CHUNK_WIDTH; localZ++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < 0.007)
                {
                    Vector3Int topmost = GetTopmostBlock(localX, localZ, new Block[] { BlockRegistry.AIR, BlockRegistry.LEAVES });
                    GenerateTree(topmost.x, topmost.y, topmost.z);
                }
            }
        }


        initialized = true;
        RenderMesh();
    }

    void GenerateTree(int x, int y, int z)
    {
        for (int leavesX = x - 2; leavesX < x + 3; leavesX++)
        {
            for (int leavesZ = z - 2; leavesZ < z + 3; leavesZ++)
            {
                for (int leavesY = y+3; leavesY < y + 7; leavesY++)
                {
                    if((leavesY == y+6 || leavesY == y+3) && (leavesX == x-2 || leavesX == x+2) && (leavesZ == z - 2 || leavesZ == z + 2))
                    {
                        continue;
                    }
                    SetBlock(leavesX, leavesY, leavesZ, BlockRegistry.LEAVES);
                }
            }
        }
        for(int trunkY = y; trunkY < y + 5; trunkY++)
        {
            SetBlock(x, trunkY, z, BlockRegistry.LOG);
        }
    }

    Vector3Int GetTopmostBlock(int x, int z, Block[] ignore)
    {
        for (int y = CHUNK_HEIGHT - 1; y > 0; y--)
        {
            if (!Array.Exists(ignore, element => element.Id == blocks[x, y, z].Id))
            {
                return new Vector3Int(x, y, z);
            }
        }
        return new Vector3Int(x, CHUNK_HEIGHT, z);
    }

    Vector3Int GetTopmostBlock(int x, int z)
    {
        return GetTopmostBlock(x, z, new Block[] {BlockRegistry.AIR});
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
        if(x < 0 || x > CHUNK_WIDTH-1 || y < 0 || y > CHUNK_HEIGHT-1 || z < 0 || z > CHUNK_WIDTH-1)
        {
            // Not in our chunk, delegate to worldhandler
            return;
            //WorldGenHandler.INSTANCE.TryGenerateBlock(block, new Vector3Int(chunkX * CHUNK_WIDTH + x, y, chunkZ * CHUNK_WIDTH + z));
        }
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
