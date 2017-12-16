#include "SimplexNoise2D.hlsl"

half4 Fragment(float4 sv_position : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    const float freq = 4;
    const float width = 0.2 * _Amplitude;

    half2 p1 = half2(uv.x * freq * 2, _LocalTime);
    half2 p2 = half2(uv.x * freq * 1, _LocalTime);
    half n = snoise(p1) + snoise(p2) / 2;

    half c1 = 1 - smoothstep(width * 0.99, width,  n);
    half c2 = 1 - smoothstep(width * 0.99, width, -n);
    half4 c = lerp(_Color1, _Color2, c1 * c2);

    return lerp(tex2D(_MainTex, uv), c, c.a);
}
