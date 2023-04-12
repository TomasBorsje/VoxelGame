using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshUtils
{
    public enum FaceDirection { North, East, South, West, Top, Bottom }
    public static void AddBlockFaceVertices(Block block, IList<Vector3> vertices, IList<Vector2> uvs, IList<int> triangles, Vector3 pos, FaceDirection face)
    {
        int vStart = vertices.Count;
        Rect uvArea = Registries.TextureAtlas.GetUVs(block.Id);

        Vector2 bottomLeft = uvArea.position;
        Vector2 top = bottomLeft + new Vector2(0, uvArea.height);
        Vector2 right = bottomLeft + new Vector2(uvArea.width, 0);
        Vector2 topRight = bottomLeft + new Vector2(uvArea.width, uvArea.height);

        switch (face)
        {
            case FaceDirection.North:
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE + Vector3.up * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE + Vector3.up * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE);

                uvs.Add(bottomLeft);
                uvs.Add(top);
                uvs.Add(topRight);
                uvs.Add(right);

                triangles.Add(vStart);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 2);
                triangles.Add(vStart + 3);
                triangles.Add(vStart);
                triangles.Add(vStart + 2);
                break;

            case FaceDirection.South:
                vertices.Add(pos);
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.right * Chunk.BLOCK_SIZE);
                uvs.Add(bottomLeft);
                uvs.Add(top);
                uvs.Add(topRight);
                uvs.Add(right);

                triangles.Add(vStart);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 2);
                triangles.Add(vStart + 3);
                triangles.Add(vStart);
                triangles.Add(vStart + 2);
                break;

            case FaceDirection.East:
                vertices.Add(pos + Vector3.right * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE + Vector3.forward * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);

                uvs.Add(bottomLeft);
                uvs.Add(top);
                uvs.Add(topRight);
                uvs.Add(right);

                triangles.Add(vStart);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 2);
                triangles.Add(vStart + 3);
                triangles.Add(vStart);
                triangles.Add(vStart + 2);
                break;

            case FaceDirection.West:
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE);
                vertices.Add(pos);
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE + Vector3.up * Chunk.BLOCK_SIZE);

                uvs.Add(bottomLeft);
                uvs.Add(right);
                uvs.Add(topRight);
                uvs.Add(top);

                triangles.Add(vStart + 2);
                triangles.Add(vStart + 1);
                triangles.Add(vStart);
                triangles.Add(vStart + 2);
                triangles.Add(vStart);
                triangles.Add(vStart + 3);
                break;

            case FaceDirection.Bottom:
                vertices.Add(pos);
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.forward * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.right * Chunk.BLOCK_SIZE);

                uvs.Add(top);
                uvs.Add(bottomLeft);
                uvs.Add(right);
                uvs.Add(topRight);

                triangles.Add(vStart + 2);
                triangles.Add(vStart + 1);
                triangles.Add(vStart);
                triangles.Add(vStart);
                triangles.Add(vStart + 3);
                triangles.Add(vStart + 2);
                break;

            case FaceDirection.Top:
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE + Vector3.forward * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE + Vector3.forward * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);
                vertices.Add(pos + Vector3.up * Chunk.BLOCK_SIZE + Vector3.right * Chunk.BLOCK_SIZE);

                uvs.Add(bottomLeft);
                uvs.Add(top);
                uvs.Add(topRight);
                uvs.Add(right);

                triangles.Add(vStart);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 3);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 2);
                triangles.Add(vStart + 3);
                break;
        }
    }
}
