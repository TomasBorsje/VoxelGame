using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshUtils
{
    public enum FaceDirection { North, East, South, West, Top, Bottom }
    public static void AddBlockMesh(Block block, IList<Vector3> vertices, IList<Vector2> uvs, IList<int> triangles, Vector3 pos, FaceDirection face)
    {
        int vStart = vertices.Count;
        Rect uvArea = TextureAtlas.Instance.GetUVs(block.Id);

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

    public static void AddCrossMesh(Block block, IList<Vector3> vertices, IList<Vector2> uvs, IList<int> triangles, Vector3 pos)
    {
        int vStart = vertices.Count - 1;
        Rect uvArea = TextureAtlas.Instance.GetUVs(block.Id);

        Vector2 bottomLeft = uvArea.position;
        Vector2 top = bottomLeft + new Vector2(0, uvArea.height);
        Vector2 right = bottomLeft + new Vector2(uvArea.width, 0);
        Vector2 topRight = bottomLeft + new Vector2(uvArea.width, uvArea.height);

        //     7     8
        //  3      4
        //
        //     5     6
        //  1      2

        vertices.Add(pos); // 1
        vertices.Add(pos + Vector3.right); // 2
        vertices.Add(pos + Vector3.up); // 3
        vertices.Add(pos + Vector3.right + Vector3.up); // 4
        vertices.Add(pos + Vector3.forward); // 5
        vertices.Add(pos + Vector3.forward + Vector3.right); // 6
        vertices.Add(pos + Vector3.forward + Vector3.up); // 7
        vertices.Add(pos + Vector3.forward + Vector3.right + Vector3.up); // 8

        uvs.Add(bottomLeft); // 1
        uvs.Add(right); // 2
        uvs.Add(top); // 3
        uvs.Add(topRight); // 4
        uvs.Add(bottomLeft); // 5
        uvs.Add(right); // 6
        uvs.Add(top); // 7
        uvs.Add(topRight); // 8

        // tris need to intersect, going clockwise
        // So 6 1 8, 8 1 3, 4 2 5, 7 4 5
        // And in reverse
        // So 8 1 6, 3 1 8, 5 2 4, 5 4 7
        triangles.Add(vStart + 6);
        triangles.Add(vStart + 1);
        triangles.Add(vStart + 8);
        triangles.Add(vStart + 8);
        triangles.Add(vStart + 1);
        triangles.Add(vStart + 3);
        triangles.Add(vStart + 4);
        triangles.Add(vStart + 2);
        triangles.Add(vStart + 5);
        triangles.Add(vStart + 7);
        triangles.Add(vStart + 4);
        triangles.Add(vStart + 5);
        triangles.Add(vStart + 8);
        triangles.Add(vStart + 1);
        triangles.Add(vStart + 6);
        triangles.Add(vStart + 3);
        triangles.Add(vStart + 1);
        triangles.Add(vStart + 8);
        triangles.Add(vStart + 5);
        triangles.Add(vStart + 2);
        triangles.Add(vStart + 4);
        triangles.Add(vStart + 5);
        triangles.Add(vStart + 4);
        triangles.Add(vStart + 7);
    }

    // DOES NOT ADD UVS!
    public static void AddCubeMesh(IList<Vector3> vertices, IList<int> triangles, Vector3 pos, FaceDirection face)
    {
        int vStart = vertices.Count;

        switch (face)
        {
            case FaceDirection.North:
                vertices.Add(pos + Vector3.forward + Vector3.right);
                vertices.Add(pos + Vector3.forward + Vector3.up + Vector3.right);
                vertices.Add(pos + Vector3.forward + Vector3.up);
                vertices.Add(pos + Vector3.forward);


                triangles.Add(vStart);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 2);
                triangles.Add(vStart + 3);
                triangles.Add(vStart);
                triangles.Add(vStart + 2);
                break;

            case FaceDirection.South:
                vertices.Add(pos);
                vertices.Add(pos + Vector3.up);
                vertices.Add(pos + Vector3.up + Vector3.right);
                vertices.Add(pos + Vector3.right);

                triangles.Add(vStart);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 2);
                triangles.Add(vStart + 3);
                triangles.Add(vStart);
                triangles.Add(vStart + 2);
                break;

            case FaceDirection.East:
                vertices.Add(pos + Vector3.right);
                vertices.Add(pos + Vector3.up + Vector3.right);
                vertices.Add(pos + Vector3.up + Vector3.forward + Vector3.right);
                vertices.Add(pos + Vector3.forward + Vector3.right);

                triangles.Add(vStart);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 2);
                triangles.Add(vStart + 3);
                triangles.Add(vStart);
                triangles.Add(vStart + 2);
                break;

            case FaceDirection.West:
                vertices.Add(pos + Vector3.forward);
                vertices.Add(pos);
                vertices.Add(pos + Vector3.up);
                vertices.Add(pos + Vector3.forward + Vector3.up);

                triangles.Add(vStart + 2);
                triangles.Add(vStart + 1);
                triangles.Add(vStart);
                triangles.Add(vStart + 2);
                triangles.Add(vStart);
                triangles.Add(vStart + 3);
                break;

            case FaceDirection.Bottom:
                vertices.Add(pos);
                vertices.Add(pos + Vector3.forward);
                vertices.Add(pos + Vector3.forward + Vector3.right);
                vertices.Add(pos + Vector3.right);

                triangles.Add(vStart + 2);
                triangles.Add(vStart + 1);
                triangles.Add(vStart);
                triangles.Add(vStart);
                triangles.Add(vStart + 3);
                triangles.Add(vStart + 2);
                break;

            case FaceDirection.Top:
                vertices.Add(pos + Vector3.up);
                vertices.Add(pos + Vector3.up + Vector3.forward);
                vertices.Add(pos + Vector3.up + Vector3.forward + Vector3.right);
                vertices.Add(pos + Vector3.up + Vector3.right);

                triangles.Add(vStart);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 3);
                triangles.Add(vStart + 1);
                triangles.Add(vStart + 2);
                triangles.Add(vStart + 3);
                break;
        }
    }

    public static void AddSizedCubeMesh(List<Vector3> vertices, List<int> triangles, Vector3 blockPos, Vector3 size, Vector3 offset)
    {
        int verticesStart = vertices.Count;

        // Add faces
        vertices.Add(new Vector3(0, 0, 0) + offset + blockPos);
        vertices.Add(new Vector3(size.x, 0, 0) + offset + blockPos);
        vertices.Add(new Vector3(size.x, size.y, 0) + offset + blockPos);
        vertices.Add(new Vector3(0, size.y, 0) + offset + blockPos);
        vertices.Add(new Vector3(0, size.y, size.z) + offset + blockPos);
        vertices.Add(new Vector3(size.x, size.y, size.z) + offset + blockPos);
        vertices.Add(new Vector3(size.x, 0, size.z) + offset + blockPos);
        vertices.Add(new Vector3(0, 0, size.z) + offset + blockPos);

        // Store LOCAL!!!! triangle indexes
        int[] newTriangles = new int[] {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        foreach (int t in newTriangles)
        {
            triangles.Add(verticesStart + t);
        }
    }
}
