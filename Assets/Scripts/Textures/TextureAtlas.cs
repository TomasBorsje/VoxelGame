using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas
{
    private static readonly string BLOCK_TEXTURES = "Textures/Block";
    private const string AtlasShaderName = "HDRP/Lit";
    private const int TEXTURE_SIZE = 16;
    private Material _atlasMaterial;
    private Dictionary<string, Rect> uvDict = new();

    public Material AtlasMaterial { get => _atlasMaterial; }
    public TextureAtlas()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>(BLOCK_TEXTURES);
        if(textures == null)
        {
            throw new System.Exception("Failed to load textures!");
        }

        int atlasSize = Mathf.CeilToInt(Mathf.Sqrt(textures.Length));

        Texture2D atlasTex = new Texture2D(atlasSize*TEXTURE_SIZE, atlasSize*TEXTURE_SIZE);
        atlasTex.filterMode = FilterMode.Point;
        atlasTex.mipMapBias = -16f;
        atlasTex.name = "TextureAtlas";
        // build atlas
        Rect[] rects = atlasTex.PackTextures(textures, 0);

        // store rects
        for(int i = 0; i < textures.Length; i++)
        {
            uvDict["game:"+textures[i].name] = rects[i];
        }

        // generate material
        _atlasMaterial = new Material(Shader.Find(AtlasShaderName));
        _atlasMaterial.mainTexture = atlasTex;
        _atlasMaterial.name = "TextureAtlasMat";

        Debug.Log("Loaded texture atlas!");
    }
    public Rect GetUVs(string id)
    {
        return uvDict[id];
    }
}
