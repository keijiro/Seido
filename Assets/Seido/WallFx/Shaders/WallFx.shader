Shader "Hidden/Seido/WallFx"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
        _Color1("", Color) = (0, 0, 0, 1)
        _Color2("", Color) = (1, 1, 1, 1)
    }

    HLSLINCLUDE

    #include "UnityCG.cginc"

    // Hash function from H. Schechter & R. Bridson, goo.gl/RXiKaH
    uint Hash(uint s)
    {
        s ^= 2747636419u;
        s *= 2654435769u;
        s ^= s >> 16;
        s *= 2654435769u;
        s ^= s >> 16;
        s *= 2654435769u;
        return s;
    }

    float Random(uint seed)
    {
        return float(Hash(seed)) / 4294967295.0; // 2^32-1
    }

    // Hue value -> RGB color
    half3 Hue2RGB(half h)
    {
        h = frac(h) * 6 - 2;
        half3 rgb = saturate(half3(abs(h - 1) - 1, 2 - abs(h), 2 - abs(h - 2)));
        return rgb;
    }

    sampler2D _MainTex;

    half4 _Color1;
    half4 _Color2;

    float _Amplitude;
    float _LocalTime;

    void Vertex(
        float4 position : POSITION,
        float2 uv : TEXCOORD0,
        out float4 outPosition : SV_Position,
        out float2 outUV : TEXCOORD0
    )
    {
        outPosition = UnityObjectToClipPos(position);
        outUV = uv;
    }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "SlitLines.hlsl"
            ENDHLSL
        }
    }
}
