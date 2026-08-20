using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

public class OutlineMaskRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public LayerMask outlineLayer;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public Settings settings = new Settings();
    public Shader maskShader; // ← drag OutlineMaskUnlit.shader in here in the Inspector

    Material m_MaskMaterial;
    OutlineMaskPass m_Pass;

    static readonly int MaskTextureId = Shader.PropertyToID("_OutlineMaskTexture");

    class OutlineMaskPass : ScriptableRenderPass
    {
        Settings settings;
        Material maskMaterial;

        class PassData { public RendererListHandle rendererList; }

        public void Setup(Settings s, Material mat)
        {
            settings = s;
            maskMaterial = mat;
            renderPassEvent = s.passEvent;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var renderingData = frameData.Get<UniversalRenderingData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var desc = cameraData.cameraTargetDescriptor;

            var maskTex = renderGraph.CreateTexture(new TextureDesc(desc.width, desc.height)
            {
                colorFormat = GraphicsFormat.R8_UNorm,
                name = "_OutlineMaskTexture",
                clearBuffer = true,
                clearColor = Color.clear
            });

            var sorting = new SortingSettings(cameraData.camera) { criteria = SortingCriteria.CommonOpaque };
            var drawing = new DrawingSettings(new ShaderTagId("UniversalForward"), sorting)
            {
                overrideMaterial = maskMaterial
            };
            var filtering = new FilteringSettings(RenderQueueRange.opaque, settings.outlineLayer);
            var listParams = new RendererListParams(renderingData.cullResults, drawing, filtering);
            var rendererList = renderGraph.CreateRendererList(listParams);

            using var builder = renderGraph.AddRasterRenderPass<PassData>("Outline Mask", out var passData);
            passData.rendererList = rendererList;
            builder.UseRendererList(rendererList);
            builder.SetRenderAttachment(maskTex, 0, AccessFlags.Write);
            builder.SetGlobalTextureAfterPass(maskTex, MaskTextureId); // exposes it to the Full Screen Pass Feature below
            builder.AllowPassCulling(false);
            
            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                ctx.cmd.DrawRendererList(data.rendererList));
        }
    }

    public override void Create()
    {
        if (maskShader != null)
            m_MaskMaterial = CoreUtils.CreateEngineMaterial(maskShader);
        m_Pass = new OutlineMaskPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_MaskMaterial == null) return;
        m_Pass.Setup(settings, m_MaskMaterial);
        renderer.EnqueuePass(m_Pass);
    }

    protected override void Dispose(bool disposing) => CoreUtils.Destroy(m_MaskMaterial);
}