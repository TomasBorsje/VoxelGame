using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MeshUtils;

public class ItemModelManager
{
    public static ItemModelManager Instance = new ItemModelManager();
    private Dictionary<string, Mesh> itemModels = new Dictionary<string, Mesh>();
    Vector3 modelOffset = new Vector3(-0.5f,-0.5f,-0.5f);
    private ItemModelManager()
    {
        foreach(Block block in BlockRegistry.Blocks)
        {
            itemModels[block.Id] = CreateBlockItemMesh(block);
        }
    }
    // Returns the item model for a given id
    public Mesh GetItemModel(string id)
    {
        return itemModels[id];
    }
    public void PopulateMeshRendererBlockItem(BlockItem item, MeshFilter filter, MeshRenderer renderer)
    {
        filter.mesh = itemModels[item.Id];
        renderer.material = TextureAtlas.Instance.GetAtlasMaterial(item.Block.RenderLayer);
    }
    private Mesh CreateBlockItemMesh(Block block)
    {
        if(block.Empty) { return new Mesh(); }
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        

        AddBlockFaceVertices(block, vertices, uvs, triangles, modelOffset, FaceDirection.Top);

        AddBlockFaceVertices(block, vertices, uvs, triangles, modelOffset, FaceDirection.Bottom);

        AddBlockFaceVertices(block, vertices, uvs, triangles, modelOffset, FaceDirection.North);

        AddBlockFaceVertices(block, vertices, uvs, triangles, modelOffset, FaceDirection.South);

        AddBlockFaceVertices(block, vertices, uvs, triangles, modelOffset, FaceDirection.East);

        AddBlockFaceVertices(block, vertices, uvs, triangles, modelOffset, FaceDirection.West);

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
