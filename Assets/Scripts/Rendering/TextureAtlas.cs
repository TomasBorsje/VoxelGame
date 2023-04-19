using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
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
        HDMaterial.ValidateMaterial(_opaqueMaterial);
        renderMatDict[RenderLayer.Opaque] = _opaqueMaterial;
        

        // generate transparent material
        Material _transparentMaterial = new Material(Shader.Find(OpaqueShaderName));
        _transparentMaterial.mainTexture = atlasTex;
        _transparentMaterial.name = "TransparentAtlasMat";
        _transparentMaterial.SetFloat("_TransparentDepthPrepassEnable", 1f);
        HDMaterial.SetSurfaceType(_transparentMaterial, true);
        HDMaterial.SetAlphaClipping(_transparentMaterial, true);
        HDMaterial.SetAlphaCutoff(_transparentMaterial, 0.5f);
        HDMaterial.ValidateMaterial(_transparentMaterial);
        renderMatDict[RenderLayer.Transparent] = _transparentMaterial;

        // generate water material
        Material _waterMaterial = new Material(Shader.Find(WaterShaderName));
        _waterMaterial.SetTexture("_MainTexture", atlasTex);
        _waterMaterial.name = "WaterAtlasMat";
        _waterMaterial.SetFloat("_TransparentSortPriority", 1f);
        _waterMaterial.SetFloat("_TransparentDepthPrepassEnable", 1f);
        HDMaterial.SetSurfaceType(_waterMaterial, true);
        HDMaterial.SetAlphaClipping(_waterMaterial, true);
        HDMaterial.SetAlphaCutoff(_waterMaterial, 0.5f);
        HDMaterial.ValidateMaterial(_waterMaterial);
        renderMatDict[RenderLayer.Water] = _waterMaterial;

        // generate leaves material
        // Transparent depth prepass
        Material _leavesMaterial = new Material(Shader.Find(LeavesShaderName));
        _leavesMaterial.SetTexture("_MainTexture", atlasTex);
        _leavesMaterial.name = "LeafAtlasMat";
        _leavesMaterial.SetFloat("_TransparentDepthPrepassEnable", 1f);
        HDMaterial.SetSurfaceType(_leavesMaterial, true);
        HDMaterial.SetAlphaClipping(_leavesMaterial, true);
        HDMaterial.SetAlphaCutoff(_leavesMaterial, 0.5f);
        HDMaterial.ValidateMaterial(_leavesMaterial);
        renderMatDict[RenderLayer.Leaves] = _leavesMaterial;

        Debug.Log($"Loaded texture atlas with {renderMatDict.Count} materials!");
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
