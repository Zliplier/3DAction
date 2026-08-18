#ifndef MAINLIGHT_INCLUDED
#define MAINLIGHT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#ifndef SHADERGRAPH_PREVIEW
float3 CalculateDiffuse(Light l, float3 normal, float3 view)
{
    //Attenuation
    float attenuation = l.shadowAttenuation * l.distanceAttenuation;
    //Diffuse
    float diffuse = saturate(dot(normal, l.direction));
    float3 h = SafeNormalize(l.direction + view);
    diffuse *= attenuation;
    
    return l.color * diffuse;
}
#endif

void MainLight_float(float3 Position, float3 Normal, float3 View, out float3 Color, out float3 Direction, out float Attenuation)
{
    #if defined(SHADERGRAPH_PREVIEW)
    Color = float3(0.5f, 0.5f, 0.5f);
    Direction = float3(0.5f, 0.5f, 0.5f);
    #else
    //Calculate Shadow Coord
    #if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(Position);
    float4 shadowCoord = ComputeScreenPos(clipPos);
    #else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
    #endif
    
    Normal = normalize(Normal);
    View = SafeNormalize(View);

    Light light = GetMainLight(shadowCoord);
    Color = CalculateDiffuse(light, Normal, View);
    Direction = light.direction;
    Attenuation = light.shadowAttenuation;
    #endif
}

#endif // MAINLIGHT_INCLUDED
