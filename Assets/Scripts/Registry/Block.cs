using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkRenderer;

public class Block : IHasId
{
    public static bool TRANSPARENT = true;
    public static bool OPAQUE = false;

    private string _name;
    private string _id;
    private RenderLayer _layer;
    public string Name { get => _name; }
    public string Id { get => _id; }
    public RenderLayer RenderLayer { get => _layer; }

    public Block(string id, string name, RenderLayer layer)
    {
        _id = id;
        _name = name;
        _layer = layer;
    }

    public bool Transparent => _layer == RenderLayer.Transparent || _layer == RenderLayer.Water;
    public bool Empty => _id == "game:air";
}
