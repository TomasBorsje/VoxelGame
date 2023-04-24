using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Chunk;

public class WorldPersistence
{
    public static string WORLD_FILENAME = "world.dat";
    public static string PLAYER_FILENAME = "player.dat";
    static Dictionary<Block, int> blockToId = new Dictionary<Block, int>();
    static Dictionary<string, int> itemToId = new Dictionary<string, int>();
    static Dictionary<int, Block> idToBlock = new Dictionary<int, Block>();
    static Dictionary<int, System.Func<Item>> idToItem = new Dictionary<int, System.Func<Item>>();
    static bool initialised = false;
    public static string GetFilePath(string worldname, string fileName)
    {
        return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), worldname, fileName);
    }
    static void Init()
    {
        int i = 0;
        foreach (Block b in BlockRegistry.Blocks)
        {
            blockToId[b] = i;
            i++;
        }
        i = 0;
        foreach (System.Func<Item> item in ItemRegistry.ITEMS)
        {
            itemToId[item().Id] = i;
            i++;
        }
        i = 0;
        foreach (Block b in BlockRegistry.Blocks)
        {
            idToBlock[i] = b;
            i++;
        }
        i = 0;
        foreach (System.Func<Item> item in ItemRegistry.ITEMS)
        {
            idToItem[i] = item;
            i++;
        }
        initialised = true;
    }
    public static void SaveWorldToDisk(string worldname)
    {
        if(!initialised) { Init(); }
        string filePath = GetFilePath(worldname, WORLD_FILENAME);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(fileStream);

        // File format
        // 1 or more chunks:
        // Int Int Int - Chunk position (x,y,z)
        // 16*32*16 Ints - Block ID x then loop z then loop y

        // Chunks
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

    public static void LoadWorldFromDisk(string worldName)
    {
        if(WorldGenHandler.INSTANCE == null)
        {
            return;
        }
        if(!initialised) { Init(); }

        string filePath = GetFilePath(worldName, WORLD_FILENAME);

        FileStream fileStream = new FileStream(filePath, FileMode.Open);
        BinaryReader reader = new BinaryReader(fileStream);

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
        }
        reader.Close();
        fileStream.Close();
    }

    public static void LoadPlayerFromDisk(string worldName)
    {
        if (WorldGenHandler.INSTANCE == null)
        {
            return;
        }
        if (!initialised) { Init(); }

        string filePath = GetFilePath(worldName, PLAYER_FILENAME);

        FileStream fileStream = new FileStream(filePath, FileMode.Open);
        BinaryReader reader = new BinaryReader(fileStream);

        // Float Float Float -- Player Position
        // Float Float Float -- player rot as euler angles
        // Inventory:
        // INVENTORY_SIZE stacks as such: EMPTY is -1, other stacks are {itemId, Count}

        // Read player pos
        Vector3 playerPos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        WorldGenHandler.INSTANCE.player.transform.position = playerPos;

        // Read player rotation
        Vector3 playerRot = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        WorldGenHandler.INSTANCE.player.transform.rotation = Quaternion.Euler(playerRot.x, playerRot.y, playerRot.z);

        // Read inventory
        ItemContainer inventory = WorldGenHandler.INSTANCE.player.GetComponent<Player>().Inventory;
        for (int slot = 0; slot < Player.INVENTORY_SIZE; slot++)
        {
            int id = reader.ReadInt32();
            if (id == -1)
            {
                inventory.SetStackInSlot(slot, ItemStack.EMPTY);
            }
            else
            {
                int count = reader.ReadInt32();
                ItemStack newStack = new ItemStack(idToItem[id](), count);
                inventory.SetStackInSlot(slot, newStack);
            }
        }

        reader.Close();
        fileStream.Close();
    }
    public static void SavePlayerToDisk(string worldName)
    {
        if (WorldGenHandler.INSTANCE == null)
        {
            return;
        }
        if (!initialised) { Init(); }

        string filePath = GetFilePath(worldName, PLAYER_FILENAME);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(fileStream);

        // Player pos
        writer.Write(WorldGenHandler.INSTANCE.player.transform.position.x);
        writer.Write(WorldGenHandler.INSTANCE.player.transform.position.y);
        writer.Write(WorldGenHandler.INSTANCE.player.transform.position.z);

        // Player rotation
        writer.Write(WorldGenHandler.INSTANCE.player.transform.eulerAngles.x);
        writer.Write(WorldGenHandler.INSTANCE.player.transform.eulerAngles.x);
        writer.Write(WorldGenHandler.INSTANCE.player.transform.eulerAngles.x);

        // Inventory - write 
        foreach (ItemStack stack in WorldGenHandler.INSTANCE.player.GetComponent<Player>().Inventory.GetStacks())
        {
            if (stack == ItemStack.EMPTY)
            {
                writer.Write(-1);
            }
            else
            {
                writer.Write(itemToId[stack.Item.Id]);
                writer.Write(stack.Count);
            }
        }

        writer.Flush();
        writer.Close();
        fileStream.Close();
    }
}
