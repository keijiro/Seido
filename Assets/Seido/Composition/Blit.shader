Shader "Seido/Blit"
{
    Properties
    {
        _MainTex("Texture", 2D) = "black" {}
        [Gamma] _Brightness("Brightness", Range(0, 1)) = 1
    }

    HLSLINCLUDE

    #include "UnityCG.cginc"

    half UVRandom(float2 uv)
    {
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    sampler2D _MainTex;
    half _Brightness;

    void Vertex(
        float4 position : POSITION, float2 uv : TEXCOORD0,
        out float4 outPosition : SV_Position, out float2 outUV : TEXCOORD0
    )
    {
        outPosition = UnityObjectToClipPos(position);
        outUV = uv;
    }

    half4 Fragment(float4 sv_position : SV_Position, float2 uv : TEXCOORD0) : SV_Target
    {
        half4 c = tex2D(_MainTex, uv);
        c.rgb = lerp(0, c.rgb, _Brightness);
        c.rgb += (UVRandom(uv) - 0.5) / 200;
        return c;
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
}
