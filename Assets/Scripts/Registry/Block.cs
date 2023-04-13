using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : IHasId
{
    public static bool TRANSPARENT = true;
    public static bool OPAQUE = false;

    private string _name;
    private string _id;
    private bool _transparent;
    public string Name { get => _name; }
    public string Id { get => _id; }
    public bool Transparent { get => _transparent; }

    public Block(string id, string name, bool transparent)
    {
        _id = id;
        _name = name;
        _transparent = transparent;
    }

    public bool Empty => _id == "game:air";
}
