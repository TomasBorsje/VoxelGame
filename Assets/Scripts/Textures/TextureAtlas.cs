using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TextureAtlas
{
    private static readonly string BLOCK_TEXTURES = "Textures/Block";
    private const string AtlasShaderName = "HDRP/Lit";
    private const int TEXTURE_SIZE = 16;
    private Material _opaqueMaterial;
    private Material _transparentMaterial;
    private Dictionary<string, Rect> uvDict = new();

    public Material AtlasMaterial { get => _opaqueMaterial; }
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

        // generate materials
        _opaqueMaterial = new Material(Shader.Find(AtlasShaderName));
        _opaqueMaterial.mainTexture = atlasTex;
        _opaqueMaterial.name = "OpaqueAtlasMat";

        // generate transparent material
        _transparentMaterial = new Material(Shader.Find(AtlasShaderName));
        _transparentMaterial.mainTexture = atlasTex;
        _transparentMaterial.name = "TransparentAtlasMat";

        Debug.Log("Loaded texture atlas!");
    }
    public Rect GetUVs(string id)
    {
        return uvDict[id];
    }
}
