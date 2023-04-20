using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Chunk;

public class WorldSave
{
    public static void SaveWorldToDisk(string filename)
    {
        FileStream fileStream = new FileStream(filename, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(fileStream);

        Dictionary<Block, int> blockToId = new Dictionary<Block, int>();

        int i = 0;
        foreach(Block b in BlockRegistry.Blocks)
        {
            blockToId[b] = i;
            i++;
        }

        // File format
        // Float Float Float -- Player Position
        // 1 or more chunks:
        // Int Int Int - Chunk position (x,y,z)
        // 16*32*16 Ints - Block ID x then loop z then loop y

        foreach (Chunk c in WorldGenHandler.INSTANCE.ChunkDictionary.Values)
        {
            Vector3Int coords = c.GetChunkCoords();
            writer.Write(coords.x);
            writer.Write(coords.y);
            writer.Write(coords.z);

            Block[,,] blocks = c.GetBlocks();
            for (int localX = 0; localX < CHUNK_WIDTH; localX++)
            {
                for (int localZ = 0; localZ < CHUNK_WIDTH; localZ++)
                {
                    for (int localY = 0; localY < CHUNK_HEIGHT; localY++)
                    {
                        Block b = blocks[localX, localY, localZ];
                        writer.Write(blockToId[b]);
                    }
                }
            }
        }

        // Dispose
        writer.Flush();
        writer.Close();
        fileStream.Close();
    }

    public static void LoadWorldFromDisk(string filename)
    {
        if(WorldGenHandler.INSTANCE == null)
        {
            return;
        }

        FileStream fileStream = new FileStream(filename, FileMode.Open);
        BinaryReader reader = new BinaryReader(fileStream);

        Dictionary<int, Block> idToBlock = new Dictionary<int, Block>();

        int i = 0;
        foreach (Block b in BlockRegistry.Blocks)
        {
            idToBlock[i] = b;
            i++;
        }

        // While we have content to read, read a chunk
        while (reader.BaseStream.Position != reader.BaseStream.Length)
        {
            Vector3Int chunkCoord = new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

            Block[,,] blocks = new Block[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH];
            for (int localX = 0; localX < CHUNK_WIDTH; localX++)
            {
                for (int localZ = 0; localZ < CHUNK_WIDTH; localZ++)
                {
                    for (int localY = 0; localY < CHUNK_HEIGHT; localY++)
                    {
                        Block b = idToBlock[reader.ReadInt32()];
                        blocks[localX, localY, localZ] = b;
                    }
                }
            }

            WorldGenHandler.INSTANCE.LoadChunk(chunkCoord.x, chunkCoord.z, blocks);
            Debug.Log($"Loaded chunk {chunkCoord} from disk!");
        }
    }
}
