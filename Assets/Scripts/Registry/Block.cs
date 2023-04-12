using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : IHasId
{
    private string _name;
    private string _id;
    private bool _opaque;
    public string Name { get => _name; }
    public string Id { get => _id; }
    public bool Opaque { get => _opaque; }

    public Block(string id, string name, bool opaque)
    {
        _id = id;
        _name = name;
        _opaque = opaque;
    }
}
