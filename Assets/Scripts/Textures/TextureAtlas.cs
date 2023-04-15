using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static ChunkRenderer;

public class TextureAtlas
{
    private static readonly string BLOCK_TEXTURES = "Textures/Block";
    public static readonly TextureAtlas Instance = new TextureAtlas();
    private const string OpaqueShaderName = "HDRP/Lit";
    private const string WaterShaderName = "Shader Graphs/WaterShader";
    private const string LeavesShaderName = "Shader Graphs/LeavesShader";
    private const int TEXTURE_SIZE = 16;
    private Material _opaqueMaterial;
    private Material _transparentMaterial;
    private Material _waterMaterial;
    private Material _leavesMaterial;
    private Dictionary<string, Rect> uvDict = new();

    public Material GetAtlasMaterial(RenderLayer layer)
    { 
        if (layer == RenderLayer.Opaque) 
        {
            return _opaqueMaterial;
        }
        if(layer == RenderLayer.Transparent)
        {
            return _transparentMaterial;
        }
        if(layer == RenderLayer.Water)
        {
            return _waterMaterial;
        }
        if (layer == RenderLayer.Leaves)
        {
            return _leavesMaterial;
        }
        return _opaqueMaterial;
    }

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
        _opaqueMaterial = new Material(Shader.Find(OpaqueShaderName));
        _opaqueMaterial.mainTexture = atlasTex;
        _opaqueMaterial.name = "OpaqueAtlasMat";

        // generate transparent material
        _transparentMaterial = new Material(Shader.Find(OpaqueShaderName));
        _transparentMaterial.mainTexture = atlasTex;
        _transparentMaterial.name = "TransparentAtlasMat";
        _transparentMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        // generate water material
        _waterMaterial = new Material(Shader.Find(WaterShaderName));
        _waterMaterial.SetTexture("_MainTexture", atlasTex);
        _waterMaterial.name = "WaterAtlasMat";
        _waterMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        // generate leaves material
        _leavesMaterial = new Material(Shader.Find(LeavesShaderName));
        _leavesMaterial.SetTexture("_MainTexture", atlasTex);
        _leavesMaterial.name = "LeafAtlasMat";
        _leavesMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        Debug.Log("Loaded texture atlas!");
    }
    public Rect GetUVs(string id)
    {
        return uvDict[id];
    }
}
