Shader "Hidden/Seido/WallFx"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _FlyerTex("", 2D) = "" {}
    }

    HLSLINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise2D.hlsl"

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
    sampler2D _FlyerTex;
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

    half Mask(float2 uv);

    half4 Fragment(float4 sv_position : SV_Position, float2 uv : TEXCOORD0) : SV_Target
    {
        half4 c = tex2D(_MainTex, uv);
        return half4(lerp(c.rgb, 1 - c.rgb, Mask(uv)), c.a);
    }

    half4 FragmentFlyer(float4 sv_position : SV_Position, float2 uv : TEXCOORD0) : SV_Target
    {
        return tex2D(_FlyerTex, uv) * 0.2;
    }

    half4 FragmentFlyerFx(float4 sv_position : SV_Position, float2 uv : TEXCOORD0) : SV_Target
    {
        half4 c = tex2D(_FlyerTex, uv);
        return lerp(0, c, 0.2 + _Amplitude * 0.8);
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
            #define WALLFX_THRU
            #include "WallFx.hlsl"
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #define WALLFX_SLITLINES
            #include "WallFx.hlsl"
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #define WALLFX_WAVEBARS
            #include "WallFx.hlsl"
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #define WALLFX_SHUTTERS
            #include "WallFx.hlsl"
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #define WALLFX_SQUARES
            #include "WallFx.hlsl"
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentFlyer
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentFlyerFx
            ENDHLSL
        }
    }
}
