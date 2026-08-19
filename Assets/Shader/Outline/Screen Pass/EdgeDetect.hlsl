#ifndef EDGEDETECT_INCLUDED
#define EDGEDETECT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

void EdgeDetect_float(float2 UV, float2 TexelSize, float DepthSensitivity, float NormalSensitivity, out float Edge)
{
    float2 uv0 = UV;
    float2 uv1 = UV + TexelSize * float2(1, 1);
    float2 uv2 = UV + TexelSize * float2(1, 0);
    float2 uv3 = UV + TexelSize * float2(0, 1);

    float d0 = SampleSceneDepth(uv0);
    float d1 = SampleSceneDepth(uv1);
    float d2 = SampleSceneDepth(uv2);
    float d3 = SampleSceneDepth(uv3);

    // Linearize for consistent thresholding across depth range
    d0 = Linear01Depth(d0, _ZBufferParams);
    d1 = Linear01Depth(d1, _ZBufferParams);
    d2 = Linear01Depth(d2, _ZBufferParams);
    d3 = Linear01Depth(d3, _ZBufferParams);

    float depthDiff = abs(d0 - d1) + abs(d2 - d3);
    float depthEdge = step(DepthSensitivity, depthDiff);

    float3 n0 = SampleSceneNormals(uv0);
    float3 n1 = SampleSceneNormals(uv1);
    float3 n2 = SampleSceneNormals(uv2);
    float3 n3 = SampleSceneNormals(uv3);

    float normalDiff = distance(n0, n1) + distance(n2, n3);
    float normalEdge = step(NormalSensitivity, normalDiff);

    Edge = saturate(depthEdge + normalEdge);
}

#endif // EDGEDETECT_INCLUDED
