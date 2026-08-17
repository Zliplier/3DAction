#ifndef TOONSTEPS_INCLUDED
#define TOONSTEPS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void ToonStep_float(float NdotL, float ShadowAtten, float Steps, float Smoothing, out float RampValue)
{
    float atten = saturate(NdotL) * ShadowAtten;
    float stepped = floor(atten * Steps) / max(Steps - 1, 1);
    stepped = smoothstep(0, Smoothing + 0.0001, atten - stepped) + stepped;
    RampValue = saturate(stepped);
}

#endif // TOONSTEPS_INCLUDED
