Shader "Hidden/SlitLines"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
        _Color1("", Color) = (0, 0, 0, 1)
        _Color2("", Color) = (1, 1, 1, 1)
    }

    CGINCLUDE

    #include "Common.hlsl"
    #include "SimplexNoise2D.hlsl"

    sampler2D _MainTex;

    half4 _Color1;
    half4 _Color2;
    float _LocalTime;
    float _Frequency;
    float _Width;

    half4 Fragment(
        float4 sv_position : SV_Position,
        float2 uv : TEXCOORD0
    ) : SV_Target
    {
        half2 p1 = half2(uv.x * _Frequency * 2, _LocalTime);
        half2 p2 = half2(uv.x * _Frequency * 1, _LocalTime);
        half n = snoise(p1) + snoise(p2) / 2;
        half c1 = 1 - smoothstep(_Width * 0.99, _Width,  n);
        half c2 = 1 - smoothstep(_Width * 0.99, _Width, -n);
        half4 c = lerp(_Color1, _Color2, c1 * c2);
        return lerp(tex2D(_MainTex, uv), c, c.a);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment Fragment
            ENDCG
        }
    }
}
