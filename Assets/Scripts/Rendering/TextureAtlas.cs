using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static ChunkRenderer;

public class TextureAtlas
{
    public static readonly TextureAtlas Instance = new TextureAtlas();
    private const string OpaqueShaderName = "HDRP/Lit";
    private const string WaterShaderName = "Shader Graphs/WaterShader";
    private const string LeavesShaderName = "Shader Graphs/LeavesShader";
    private const int TEXTURE_SIZE = 16;

    Dictionary<RenderLayer, Material> renderMatDict = new Dictionary<RenderLayer, Material>();
    public Texture2D atlasTex;
    private Dictionary<string, Rect> uvDict = new();

    public Material GetAtlasMaterial(RenderLayer layer)
    {
        return renderMatDict.GetValueOrDefault(layer, renderMatDict[RenderLayer.Opaque]);
    }

    public TextureAtlas()
    {
        Texture2D[] textures = ResourceCache.Instance.BlockTextures;
        if(textures == null)
        {
            throw new System.Exception("Failed to load textures!");
        }

        int atlasSize = Mathf.CeilToInt(Mathf.Sqrt(textures.Length));

        atlasTex = new Texture2D(atlasSize*TEXTURE_SIZE, atlasSize*TEXTURE_SIZE);
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
        Material _opaqueMaterial = new Material(Shader.Find(OpaqueShaderName));
        _opaqueMaterial.mainTexture = atlasTex;
        _opaqueMaterial.name = "OpaqueAtlasMat";
        renderMatDict[RenderLayer.Opaque] = _opaqueMaterial;

        // generate transparent material
        Material _transparentMaterial = new Material(Shader.Find(OpaqueShaderName));
        _transparentMaterial.mainTexture = atlasTex;
        _transparentMaterial.name = "TransparentAtlasMat";
        _transparentMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        renderMatDict[RenderLayer.Transparent] = _transparentMaterial;

        // generate water material
        Material _waterMaterial = new Material(Shader.Find(WaterShaderName));
        _waterMaterial.SetTexture("_MainTexture", atlasTex);
        _waterMaterial.name = "WaterAtlasMat";
        _waterMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        renderMatDict[RenderLayer.Water] = _waterMaterial;

        // generate leaves material
        // Transparent depth prepass
        Material _leavesMaterial = new Material(Shader.Find(LeavesShaderName));
        _leavesMaterial.SetTexture("_MainTexture", atlasTex);
        _leavesMaterial.name = "LeafAtlasMat";
        _leavesMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        renderMatDict[RenderLayer.Leaves] = _leavesMaterial;

        Debug.Log("Loaded texture atlas!");
    }
    public Rect GetUVs(string id)
    {
        return uvDict[id];
    }
    public Rect GetRect(string id)
    {
        Rect uv = uvDict[id];
        return new Rect(uv.x * atlasTex.width, uv.y * atlasTex.height, TEXTURE_SIZE, TEXTURE_SIZE);
    }
}
