using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Chunk;
using static MeshUtils;

public class LeavesChunkRenderer : ChunkRenderer
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
                    // we always render each face of the leaves, as they must be seen through eachother and
                    // the shader leaves gaps next to solid blocks due to the waving
                    AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.Top);

                    AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.Bottom);

                    AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.North);

                    AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.South);

                    AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.East);

                    AddBlockFaceVertices(block, vertices, uvs, triangles, blockPos, FaceDirection.West);

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
