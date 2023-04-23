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
    public static float PERLIN_SCALE = 0.015f;
    public static readonly int FLOOR_LEVEL = 16;
    public static readonly int SEA_LEVEL = 20;
    private int SELECTION_RAYCAST_LAYER = 6;
    private int CHUNK_COLLIDER_LAYER = 7;
    public static readonly int RAYCAST_MASK = (1 << 6) | 7;

    int rootX, rootY, rootZ;
    int chunkX, chunkZ;

    ChunkRenderer[] renderers = new ChunkRenderer[Enum.GetValues(typeof(RenderLayer)).Length];

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    // Child gameobject for raycasting
    public MeshFilter interactFilter;
    public MeshCollider interactCollider;

    Mesh displayMesh;
    Mesh colliderMesh; // Movement collisions, e.g. no flower hitboxes
    Mesh interactMesh; // Interact collisions, e.g. flower outlines for block breaking
    // Note that interact mesh has ONLY custom hitboxes! We use both hitboxes for raycasting.

    bool shouldUpdate = false;
    bool initialized = false;

    // X, Y, Z
    Block[,,] blocks = new Block[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH];
    // Tile entities
    Dictionary<Vector3Int, BlockEntity> blockEntities = new();
    public void Init(bool genNewChunk = true)
    {
        rootX = (int)transform.position.x;
        rootY = (int)transform.position.y;
        rootZ = (int)transform.position.z;

        chunkX = Mathf.FloorToInt(transform.position.x / (CHUNK_WIDTH * BLOCK_SIZE));
        chunkZ = Mathf.FloorToInt(transform.position.z / (CHUNK_WIDTH * BLOCK_SIZE));

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        displayMesh = new Mesh();
        colliderMesh = new Mesh();

        // Raycasting layer
        GameObject interactObject = new GameObject();
        interactObject.transform.SetParent(gameObject.transform);
        interactObject.transform.localPosition = Vector3.zero;
        interactCollider = interactObject.AddComponent<MeshCollider>();
        interactFilter = interactObject.AddComponent<MeshFilter>();
        interactMesh = new Mesh();


        // Create our chunk renderers and store them (no gameobject needed!)
        int i = 0;
        foreach(RenderLayer layer in Enum.GetValues(typeof(RenderLayer)))
        {
            ChunkRenderer comp = null;
            // todo: rename leaves renderer to wavy
            switch (layer) {
                case RenderLayer.Opaque: { comp = new OpaqueChunkRenderer(); break; }
                case RenderLayer.Water: { comp = new WaterChunkRenderer(); break; }
                case RenderLayer.Leaves: { comp = new LeavesChunkRenderer(); break; }
                case RenderLayer.Transparent: { comp = new TransparentChunkRenderer(); break; }
                default: { comp = new OpaqueChunkRenderer(); break; }
            }
            // todo: remove layer arg
            comp.Init(this, layer);
            renderers[i++] = comp;
        }

        // TODO: World gen function
        // Init to air
        if (genNewChunk)
        {
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
                    bool isDirt = Mathf.PerlinNoise((rootX + localX) * PERLIN_SCALE * 7, (rootZ + localZ) * PERLIN_SCALE * 7) < 0.8f;
                    for (int localY = 0; localY < CHUNK_HEIGHT; localY++)
                    {
                        float floorLevel = FLOOR_LEVEL + (CHUNK_HEIGHT - FLOOR_LEVEL) * Mathf.PerlinNoise((rootX + localX) * PERLIN_SCALE, (rootZ + localZ) * PERLIN_SCALE) * 0.8f;
                        if (localY < floorLevel)
                        {
                            if(localY+1 > floorLevel && isDirt)
                            {
                                blocks[localX, localY, localZ] = BlockRegistry.GRASS;
                            }
                            else
                            {
                                blocks[localX, localY, localZ] = isDirt ? BlockRegistry.DIRT : BlockRegistry.STONE;
                            }
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
                    float treeChance = UnityEngine.Random.Range(0f, 1f);
                    if (treeChance < 0.007)
                    {
                        Vector3Int topmost = GetTopmostBlock(localX, localZ, new Block[] { BlockRegistry.AIR, BlockRegistry.LEAVES });
                        if(blocks[topmost.x,topmost.y-1,topmost.z] == BlockRegistry.WATER)
                        {
                            continue;
                        }
                        if (treeChance < 0.0035)
                        {
                            GenerateTree(topmost.x, topmost.y, topmost.z);
                        }
                        else
                        {
                            blocks[topmost.x, topmost.y, topmost.z] = BlockRegistry.DANDELION;
                        }
                    }
                }
            }
        }


        initialized = true;
        RenderMesh();
    }
    public void AddBlockEntity(BlockEntity entity, Vector3Int pos)
    {
        blockEntities[pos] = entity;
    }

    internal void Tick()
    {
        // Tick all blockentities
        foreach(BlockEntity blockEntity in blockEntities.Values)
        {
            blockEntity.Tick();
        }
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
                return new Vector3Int(x, Mathf.Min(y+1, CHUNK_HEIGHT-1), z);
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
    public void SetBlocks(Block[,,] blocks)
    {
        this.blocks = blocks;
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
        if(blocks[x, y, z] == block)
        {
            // Don't reset the same block over and over.
            return;
        }
        blocks[x, y, z] = block;
        if(block == BlockRegistry.AIR)
        {
            Vector3Int chunkPos = new Vector3Int(x, y, z);
            if(blockEntities.ContainsKey(chunkPos))
            {
                blockEntities[chunkPos].OnDelete();
                blockEntities.Remove(chunkPos);
            }
        }
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
        displayMesh.Clear();
        CombineInstance[] combine = new CombineInstance[renderers.Length];
        // track which materials we need (empty meshes aren't merged, so need to align counts)
        List<RenderLayer> materialsNeeded = new List<RenderLayer>(renderers.Length);

        // render each layer
        for(int i = 0; i < renderers.Length; i++)
        {
            Mesh mesh = renderers[i].RenderChunk();
            if(mesh.vertexCount>0)
            {
                materialsNeeded.Add(renderers[i].layer);
            }
            combine[i].mesh = mesh;
        }
        displayMesh.CombineMeshes(combine, false, false);
        meshFilter.sharedMesh = displayMesh;
        // Apply materials to each layer
        Material[] mats = new Material[materialsNeeded.Count];
        for (int i = 0; i < materialsNeeded.Count; i++)
        {
            mats[i] = TextureAtlas.Instance.GetAtlasMaterial(materialsNeeded[i]);
        }
        meshRenderer.materials = mats;

        // Collider mesh for physics
        colliderMesh.Clear();
        colliderMesh = ChunkColliderMeshGenerator.GetColliderMesh(this);
        meshCollider.sharedMesh = colliderMesh;
        gameObject.layer = CHUNK_COLLIDER_LAYER;

        // Collider mesh for interaction
        interactMesh.Clear();
        interactMesh = ChunkColliderMeshGenerator.GetInteractMesh(this);
        interactFilter.sharedMesh = interactMesh;
        interactCollider.sharedMesh = interactMesh;
        interactCollider.gameObject.layer = SELECTION_RAYCAST_LAYER;

        shouldUpdate = false;
    }
}
