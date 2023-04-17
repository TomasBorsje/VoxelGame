using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceCache
{
    private static readonly string BLOCK_TEXTURES = "Textures/Block";
    private static readonly string ITEM_TEXTURES = "Textures/Item";
    private static readonly string MISSING_TEXTURE = "Textures/missing";
    public static ResourceCache Instance = new ResourceCache();

    private readonly Texture2D[] blockTextures;
    private readonly Texture2D[] itemTextures;
    private readonly Texture2D missingTexture;
    public Texture2D[] BlockTextures { get => blockTextures; }
    public Texture2D[] ItemTextures { get => itemTextures; }

    private ResourceCache() 
    {
        missingTexture = Resources.Load<Texture2D>(MISSING_TEXTURE);
        blockTextures = Resources.LoadAll<Texture2D>(BLOCK_TEXTURES);
        Debug.Log($"Loaded {blockTextures.Length} block textures!");
        itemTextures = Resources.LoadAll<Texture2D>(ITEM_TEXTURES);
        Debug.Log($"Loaded {itemTextures.Length} item textures!");
    }
    public Texture2D GetTextureForId(string id)
    {
        var tex = blockTextures.FirstOrDefault(tex => tex.name == "game:" + id);
        if(tex != null) { return tex; }
        tex = itemTextures.FirstOrDefault(tex => tex.name == "game:" + id);
        if (tex != null) { return tex; }
        return missingTexture;
    }
}
