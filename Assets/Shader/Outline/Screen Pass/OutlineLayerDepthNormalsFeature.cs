using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Shader.Outline
{
    public class OutlineLayerDepthNormalsFeature : ScriptableRendererFeature
    {
        class OutlinePass : ScriptableRenderPass
        {
            FilteringSettings filteringSettings;
            List<ShaderTagId> shaderTagIds = new List<ShaderTagId> { new ShaderTagId("DepthNormals") };

            class PassData
            {
                public RendererListHandle rendererList;
            }

            public OutlinePass(LayerMask mask)
            {
                filteringSettings = new FilteringSettings(RenderQueueRange.opaque, mask);
                renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                UniversalLightData lightData = frameData.Get<UniversalLightData>();

                var colorDesc = cameraData.cameraTargetDescriptor;
                colorDesc.depthBufferBits = 0;
                colorDesc.colorFormat = RenderTextureFormat.ARGBHalf;
                colorDesc.msaaSamples = 1;
                TextureHandle colorTarget = renderGraph.CreateTexture(new TextureDesc(colorDesc)
                {
                    name = "_OutlineLayerDepthNormals",
                    clearBuffer = true,
                    clearColor = Color.clear
                });

                var depthDesc = cameraData.cameraTargetDescriptor;
                depthDesc.depthBufferBits = 32;
                TextureHandle depthTarget = renderGraph.CreateTexture(new TextureDesc(depthDesc)
                {
                    name = "_OutlineLayerDepthTemp",
                    clearBuffer = true,
                    clearColor = Color.clear
                });

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Outline Layer DepthNormals", out var passData))
                {
                    var sortFlags = cameraData.defaultOpaqueSortFlags;
                    var drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagIds, renderingData, cameraData, lightData, sortFlags);
                    var param = new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings);
                    passData.rendererList = renderGraph.CreateRendererList(param);

                    builder.UseRendererList(passData.rendererList);
                    builder.SetRenderAttachment(colorTarget, 0);
                    builder.SetRenderAttachmentDepth(depthTarget);
                    builder.SetGlobalTextureAfterPass(colorTarget, UnityEngine.Shader.PropertyToID("_OutlineLayerDepthNormals"));

                    builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                    {
                        ctx.cmd.DrawRendererList(data.rendererList);
                    });
                }
            }
        }

        public LayerMask outlineLayer;
        OutlinePass pass;

        public override void Create()
        {
            pass = new OutlinePass(outlineLayer);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }
    }
}