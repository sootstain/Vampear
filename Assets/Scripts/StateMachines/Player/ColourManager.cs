using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ColourManager : MonoBehaviour{

    public static ColourManager instance;
    public Shader texturePaint;
    public Shader extendIslands;

    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int positionID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int blendOpID = Shader.PropertyToID("_BlendOp");
    int colorID = Shader.PropertyToID("_PainterColor");
    int textureID = Shader.PropertyToID("_MainTex");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");
    int fadeID = Shader.PropertyToID("_Fade");
    
    
    
    Material paintMaterial;
    Material extendMaterial;

    CommandBuffer command;

    private List<Paintable> allPaintables;

    public void Awake(){
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);
        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;
        allPaintables = new List<Paintable>();
    }
    public void initTextures(Paintable paintable){
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        command.SetRenderTarget(mask);
        command.SetRenderTarget(extend);
        command.SetRenderTarget(support);

        paintMaterial.SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        command.DrawRenderer(rend, paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }


    public void paint(Paintable paintable, Vector3 pos, float radius = 1f, float hardness = .5f, float strength = .5f, Color? color = null, int fade = 0){
        if (!allPaintables.Contains(paintable)) allPaintables.Add(paintable);
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        paintMaterial.SetFloat(prepareUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID, support);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        paintMaterial.SetFloat(fadeID, fade);
        extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        extendMaterial.SetTexture(uvIslandsID, uvIslands);
        
        command.SetRenderTarget(mask);
        command.DrawRenderer(rend, paintMaterial, 0);

        command.SetRenderTarget(support);
        command.Blit(mask, support);

        command.SetRenderTarget(extend);
        command.Blit(mask, extend, extendMaterial);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }
    
    [SerializeField] private Material fadeMaterial;
    [SerializeField] private float fadeInterval = 0.5f;

    private float fadeTimer;

    void Update()
    {
        fadeTimer += Time.deltaTime;
        if (fadeTimer >= fadeInterval && allPaintables.Count > 0)
        {
            fadeTimer = 0f;
            FadeAllPaintables();
        }
    }
    
    private void FadeAllPaintables()
    {
        foreach (var p in allPaintables)
        {
            if (p == null) continue;

            if (p.IsBeingRevealed || p.ForceAlphaZero) 
                continue;

            RenderTexture rt = p.getSupport();
            if (rt == null) continue;

            RenderTexture temp = RenderTexture.GetTemporary(rt.width, rt.height, 0, rt.format);
            Graphics.Blit(rt, temp, fadeMaterial);
            Graphics.Blit(temp, rt);
            RenderTexture.ReleaseTemporary(temp);
        }
    }

}
