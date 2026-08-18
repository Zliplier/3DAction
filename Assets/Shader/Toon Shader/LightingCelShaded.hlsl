#ifndef LIGHTINGCELSHADED_INCLUDED
#define LIGHTINGCELSHADED_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#ifndef SHADERGRAPH_PREVIEW
struct SurfaceVariables
{
    float3 normal;
    float3 view;
    float smoothness;
    float shininess;
    float rimThreshold;
};

float3 CalculateCelShading(Light l, SurfaceVariables s)
{
    //Attenuation
    float attenuation = l.shadowAttenuation * l.distanceAttenuation;
    //Diffuse
    float diffuse = saturate(dot(s.normal, l.direction));
    float3 h = SafeNormalize(l.direction + s.view);
    diffuse *= attenuation;
    //Specular
    float specular = saturate(dot(s.normal, h));
    specular = pow(specular, s.shininess);
    specular *= diffuse * s.smoothness;
    //Rim
    float rim = 1 - dot(s.view, s.normal);
    rim *= pow(diffuse, s.rimThreshold);
    
    return l.color * (diffuse + max(specular, rim));
}
#endif

// LightingCelShaded.hlsl
void LightingCelShaded_float(float Smoothness, float RimThreshold, float3 Position, float3 Normal, float3 View, out float3 Color, out float3 Direction)
{
    #if defined(SHADERGRAPH_PREVIEW)
    Color = float3(0.5f, 0.5f, 0.5f);
    #else
    SurfaceVariables s;
    s.normal = normalize(Normal);
    s.view = SafeNormalize(View);
    s.smoothness = Smoothness;
    s.shininess = exp2(10 * Smoothness + 1);
    s.rimThreshold = RimThreshold;
    
    #if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(Position);
    float4 shadowCoord = ComputeScreenPos(clipPos);
    #else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
    #endif
    
    Light light = GetMainLight();
    Color = CalculateCelShading(light, s);
    Direction = light.direction;

    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; i++)
    {
        light = GetAdditionalLight(i, Position, 1);
        Color += CalculateCelShading(light, s);
    }
    
    #endif
}

#endif // LIGHTINGCELSHADED_INCLUDED
