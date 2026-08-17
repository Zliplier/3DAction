#ifndef ADDITIONALLIGHTS_INCLUDED
#define ADDITIONALLIGHTS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void AdditionalLights_float(float3 WorldPos, float3 WorldNormal, float3 ViewDir,
    float Steps, float Smoothing, float SpecSize, float SpecSmoothing,
    out float3 Diffuse, out float3 Specular)
{
    float3 diffuseColor = 0;
    float3 specColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; i++)
    {
        Light light = GetAdditionalLight(i, WorldPos);
        float atten = light.distanceAttenuation * light.shadowAttenuation;

        float NdotL = saturate(dot(WorldNormal, light.direction));
        float stepped = floor(NdotL * Steps) / max(Steps - 1, 1);
        stepped = smoothstep(0, Smoothing + 0.0001, NdotL - stepped) + stepped;
        stepped = saturate(stepped);

        diffuseColor += light.color * atten * stepped;

        float3 halfVec = normalize(light.direction + ViewDir);
        float NdotH = saturate(dot(WorldNormal, halfVec));
        float spec = smoothstep(SpecSize - SpecSmoothing, SpecSize + SpecSmoothing, NdotH);
        specColor += light.color * atten * spec * stepped;
    }
    #endif

    Diffuse = diffuseColor;
    Specular = specColor;
}

#endif // ADDITIONALLIGHTS_INCLUDED
