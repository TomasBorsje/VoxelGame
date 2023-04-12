using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenHandler : MonoBehaviour
{
    public static int RENDER_DISTANCE = 8;
    public static WorldGenHandler INSTANCE = null;

    Dictionary<(int, int), Chunk> ChunkDictionary = new Dictionary<(int, int), Chunk>();

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

    private void Awake()
    {
        if(INSTANCE != null)
        {
            Debug.LogWarning("Destroying duplicate world gen handler!");
            Destroy(this.gameObject);
        }
        INSTANCE = this;
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
            int playerChunkX = 0;//Mathf.FloorToInt(player.transform.position.x / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));
            int playerChunkZ = 0;// Mathf.FloorToInt(player.transform.position.z / (Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE));
            Debug.Log($"Player is in {playerChunkX},{playerChunkZ}");

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
                    Debug.Log($"Removing Chunk {chunkX},{chunkZ}");
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
            Debug.LogError($"Tried to generate new chunk for existing coordinate {chunkX},{chunkZ}");
            return;
        }
        //Debug.Log($"Creating Chunk {chunkX},{chunkZ}");
        GameObject newChunk = new GameObject("Chunk " + chunkX + "," + chunkZ);
        Chunk comp = newChunk.AddComponent<Chunk>();
        newChunk.transform.position = new Vector3(chunkX * Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE, 0, chunkZ * Chunk.CHUNK_WIDTH * Chunk.BLOCK_SIZE);
        comp.Init();
        ChunkDictionary[(chunkX, chunkZ)] = comp;

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
