using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkRenderer;

public class Block : IHasId
{
    public static bool TRANSPARENT = true;
    public static bool OPAQUE = false;

    protected string _name;
    protected string _id;
    protected RenderLayer _layer;
    protected bool _hasCustomModel;
    public string Name { get => _name; }
    public string Id { get => _id; }
    public RenderLayer RenderLayer { get => _layer; }
    public bool HasCustomModel => _hasCustomModel;

    public Block(string id, string name, RenderLayer layer, bool hasCustomModel = false)
    {
        _id = id;
        _name = name;
        _layer = layer;
        _hasCustomModel = hasCustomModel;
    }

    public bool Transparent => _layer == RenderLayer.Transparent || _layer == RenderLayer.Water || _layer == RenderLayer.Leaves;
    public bool Empty => _id == "game:air";
    public virtual void ApplyCustomModel(List<Vector3> vertices, List<Vector2> uvs, List<int> tris, Vector3 blockPos) { }
}
