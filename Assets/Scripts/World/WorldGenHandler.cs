using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldGenHandler : MonoBehaviour
{
    public readonly int WORLD_SEED = 666;
    public static int RENDER_DISTANCE = 11;
    public static WorldGenHandler INSTANCE = null;

    public Dictionary<(int, int), Chunk> ChunkDictionary = new Dictionary<(int, int), Chunk>();
    Dictionary<(int, int), List<(Block, Vector3Int)>> WorldgenWaitlist = new Dictionary<(int, int), List<(Block, Vector3Int)>>();

    private float chunkUpdateTimer = 0;
    GameObject player;

    public (Chunk, Vector3Int) WorldPosToChunkPos(Vector3 worldPos)
    {
        int chunkX = Mathf.FloorToInt(worldPos.x / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));
        int chunkZ = Mathf.FloorToInt(worldPos.z / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));
        if(ChunkDictionary.ContainsKey((chunkX, chunkZ)))
        {
            int localX = worldPos.x % Chunk.CHUNK_WIDTH < 0 ? (int)(worldPos.x % Chunk.CHUNK_WIDTH + Chunk.CHUNK_WIDTH) : (int)(worldPos.x % Chunk.CHUNK_WIDTH);
            int localZ = worldPos.z % Chunk.CHUNK_WIDTH < 0 ? (int)(worldPos.z % Chunk.CHUNK_WIDTH + Chunk.CHUNK_WIDTH) : (int)(worldPos.z % Chunk.CHUNK_WIDTH);
            if(worldPos.y > Chunk.CHUNK_HEIGHT || worldPos.y < 0)
            {
                Debug.LogWarning("Tried to get chunk coordinate with invalid y value " + worldPos.y);
                return (null, Vector3Int.zero);
            }
            int localY = (int)worldPos.y;
            return (ChunkDictionary[(chunkX, chunkZ)], new Vector3Int(localX, localY, localZ));
        }
        return (null, Vector3Int.zero);
    }
    public void TryGenerateBlock(Block block, Vector3Int worldPos)
    {
        int chunkX = Mathf.FloorToInt(worldPos.x / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));
        int chunkZ = Mathf.FloorToInt(worldPos.z / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));
        if (ChunkDictionary.ContainsKey((chunkX, chunkZ)))
        {
            int localX = worldPos.x % Chunk.CHUNK_WIDTH < 0 ? (int)(worldPos.x % Chunk.CHUNK_WIDTH + Chunk.CHUNK_WIDTH) : (int)(worldPos.x % Chunk.CHUNK_WIDTH);
            int localZ = worldPos.z % Chunk.CHUNK_WIDTH < 0 ? (int)(worldPos.z % Chunk.CHUNK_WIDTH + Chunk.CHUNK_WIDTH) : (int)(worldPos.z % Chunk.CHUNK_WIDTH);
            ChunkDictionary[(chunkX, chunkZ)].SetBlock(localX, worldPos.y, localZ, block);
        }
        else
        {
            // Add to waitlist
            if(!WorldgenWaitlist.ContainsKey((chunkX, chunkZ)))
            {
                WorldgenWaitlist[(chunkX, chunkZ)] = new List<(Block, Vector3Int)>();
            }
            WorldgenWaitlist[(chunkX, chunkZ)].Add((block, worldPos));
        }
    }

    private void Awake()
    {
        if(INSTANCE != null)
        {
            Debug.LogWarning("Destroying duplicate world gen handler!");
            Destroy(this.gameObject);
        }
        INSTANCE = this;
        Random.InitState(WORLD_SEED);

        if (File.Exists(@$"C:\Users\GGPC\Documents\{WorldGenHandler.INSTANCE.WORLD_SEED}.world"))
        {
            WorldSave.LoadWorldFromDisk(@$"C:\Users\GGPC\Documents\{WorldGenHandler.INSTANCE.WORLD_SEED}.world");
        }
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        for(int x = -RENDER_DISTANCE; x < RENDER_DISTANCE; x++)
        {
            for(int z = -RENDER_DISTANCE; z < RENDER_DISTANCE; z++)
            {
                TryGenNewChunk(x, z);
            }
        }
    }

    public Chunk GetChunk(int chunkX, int chunkZ)
    {
        if(!ChunkDictionary.ContainsKey((chunkX, chunkZ)))
        {
            Debug.LogError($"Tried to get chunk {chunkX},{chunkZ} that doesn't exist!");
        }
        return ChunkDictionary[(chunkX, chunkZ)];
    }

    public bool ChunkLoaded(int chunkX, int chunkZ)
    {
        return ChunkDictionary.ContainsKey((chunkX, chunkZ));
    }

    private void Update()
    {
        chunkUpdateTimer += Time.deltaTime;
        if(chunkUpdateTimer > 1)
        {
            chunkUpdateTimer = 0;

            // Get player chunk coordinates
            int playerChunkX = Mathf.FloorToInt(player.transform.position.x / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));
            int playerChunkZ = Mathf.FloorToInt(player.transform.position.z / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));

            for (int renderX = playerChunkX - RENDER_DISTANCE; renderX < playerChunkX + RENDER_DISTANCE; renderX++)
            {
                for (int renderZ = playerChunkZ - RENDER_DISTANCE; renderZ < playerChunkZ + RENDER_DISTANCE; renderZ++)
                {
                    if (!ChunkDictionary.ContainsKey((renderX, renderZ))) {
                        TryGenNewChunk(renderX, renderZ); 
                    }
                }
            }

            // Unload other chunks
            List<(int, int)> keysToRemove = new List<(int, int)>();
            foreach(Chunk chunk in ChunkDictionary.Values)
            {
                int chunkX = Mathf.FloorToInt(chunk.transform.position.x / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));
                int chunkZ = Mathf.FloorToInt(chunk.transform.position.z / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));

                // Out of render distance (square)
                if(Mathf.Abs(chunkX - playerChunkX) > RENDER_DISTANCE || Mathf.Abs(chunkZ - playerChunkZ) > RENDER_DISTANCE)
                {
                    keysToRemove.Add((chunkX, chunkZ));
                }
            }

            foreach((int, int) key in keysToRemove)
            {
                Destroy(ChunkDictionary[key].gameObject);
                ChunkDictionary.Remove(key);
            }
        }
    }

    void TryGenNewChunk(int chunkX, int chunkZ)
    {
        if(ChunkDictionary.ContainsKey((chunkX, chunkZ))) {
            // Don't generate chunks that already exist!
            return;
        }
        GameObject newChunk = new GameObject("Chunk " + chunkX + "," + chunkZ);
        newChunk.transform.position = new Vector3(chunkX * Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE, 0, chunkZ * Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE);

        // Create, store, init chunk
        Chunk comp = newChunk.AddComponent<Chunk>();
        ChunkDictionary[(chunkX, chunkZ)] = comp; // Store first so chunk can detect itself as existing
        comp.Init();

        ReRenderNeighbours(chunkX, chunkZ);
    }

    public void LoadChunk(int chunkX, int chunkZ, Block[,,] blocks)
    {
        if (ChunkDictionary.ContainsKey((chunkX, chunkZ)))
        {
            Debug.LogError($"Tried to load existing chunk at coordinates {chunkX},{chunkZ}");
            return;
        }
        GameObject newChunk = new GameObject("Chunk " + chunkX + "," + chunkZ);
        newChunk.transform.position = new Vector3(chunkX * Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE, 0, chunkZ * Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE);

        // Create, store, init chunk
        Chunk comp = newChunk.AddComponent<Chunk>();
        comp.SetBlocks(blocks);
        ChunkDictionary[(chunkX, chunkZ)] = comp; // Store first so chunk can detect itself as existing
        comp.Init(genNewChunk: false);

        ReRenderNeighbours(chunkX, chunkZ);
    }

    public void ReRenderNeighbours(int chunkX, int chunkZ)
    {
        // re-render neighbouring chunks
        if (ChunkDictionary.ContainsKey((chunkX + 1, chunkZ)))
        {
            ChunkDictionary[(chunkX + 1, chunkZ)].MarkUpdate();
        }
        if (ChunkDictionary.ContainsKey((chunkX - 1, chunkZ)))
        {
            ChunkDictionary[(chunkX - 1, chunkZ)].MarkUpdate();
        }
        if (ChunkDictionary.ContainsKey((chunkX, chunkZ + 1)))
        {
            ChunkDictionary[(chunkX, chunkZ + 1)].MarkUpdate();
        }
        if (ChunkDictionary.ContainsKey((chunkX, chunkZ - 1)))
        {
            ChunkDictionary[(chunkX, chunkZ - 1)].MarkUpdate();
        }
    }
}
