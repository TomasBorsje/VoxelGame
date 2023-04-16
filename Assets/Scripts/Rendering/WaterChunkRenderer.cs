using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Chunk;
using static MeshUtils;

public class WaterChunkRenderer : ChunkRenderer
{
    public override void RenderChunk()
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
                    // Only render our selected layer!
                    if (block.Empty || block.RenderLayer != layer) { continue; }

                    Vector3 blockPos = new Vector3(x, y, z);
                    // Render a block

                    // Top
                    // Always render the top of water, as we are not a full block!
                    AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.Top);
                    
                    // Bottom
                    if (y == 0 || blocks[x, y - 1, z].Empty || !blocks[x, y - 1, z].Transparent)
                    {
                        AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.Bottom);
                    }
                    // North
                    if (z == CHUNK_WIDTH - 1)
                    {
                        if (WorldGenHandler.INSTANCE.ChunkLoaded(chunkX, chunkZ + 1))
                        {
                            Block neighbourBlock = WorldGenHandler.INSTANCE.GetChunk(chunkX, chunkZ + 1).GetBlock(x, y, 0);
                            if (neighbourBlock.Empty || !neighbourBlock.Transparent)
                            {
                                AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.North);
                            }
                        }
                    }
                    else if (blocks[x, y, z + 1].Empty || !blocks[x, y, z + 1].Transparent)
                    {
                        AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.North);
                    }
                    // South
                    if (z == 0)
                    {
                        if (WorldGenHandler.INSTANCE.ChunkLoaded(chunkX, chunkZ - 1))
                        {
                            Block neighbourBlock = WorldGenHandler.INSTANCE.GetChunk(chunkX, chunkZ - 1).GetBlock(x, y, CHUNK_WIDTH - 1);
                            if (neighbourBlock.Empty || !neighbourBlock.Transparent)
                            {
                                AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.South);
                            }
                        }
                    }
                    else if (blocks[x, y, z - 1].Empty || !blocks[x, y, z - 1].Transparent)
                    {
                        AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.South);
                    }
                    // East
                    if (x == CHUNK_WIDTH - 1)
                    {
                        if (WorldGenHandler.INSTANCE.ChunkLoaded(chunkX + 1, chunkZ))
                        {
                            Block neighbourBlock = WorldGenHandler.INSTANCE.GetChunk(chunkX + 1, chunkZ).GetBlock(0, y, z);
                            if (neighbourBlock.Empty || !neighbourBlock.Transparent)
                            {
                                AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.East);
                            }
                        }
                    }
                    else if (blocks[x + 1, y, z].Empty || !blocks[x + 1, y, z].Transparent)
                    {
                        AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.East);
                    }
                    // West
                    if (x == 0)
                    {
                        if (WorldGenHandler.INSTANCE.ChunkLoaded(chunkX - 1, chunkZ))
                        {
                            Block neighbourBlock = WorldGenHandler.INSTANCE.GetChunk(chunkX - 1, chunkZ).GetBlock(CHUNK_WIDTH - 1, y, z);
                            if (neighbourBlock.Empty || !neighbourBlock.Transparent)
                            {
                                AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.West);
                            }
                        }
                    }
                    else if (blocks[x - 1, y, z].Empty || !blocks[x - 1, y, z].Transparent)
                    {
                        AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.West);
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
    }
}
