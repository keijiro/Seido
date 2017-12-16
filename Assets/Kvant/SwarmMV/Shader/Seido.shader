Shader "Kvant/SwarmMV/Seido"
{
    Properties
    {
        [HideInInspector] _PositionBuffer("", 2D) = "black"{}
        [HideInInspector] _PreviousPositionBuffer("", 2D) = "black"{}

        [HideInInspector] _BinormalBuffer("", 2D) = "red"{}
        [HideInInspector] _PreviousBinormalBuffer("", 2D) = "red"{}
    }

    CGINCLUDE

    #include "Common.cginc"

    float4 Vertex(float4 position : POSITION, float2 texcoord : TEXCOORD0) : SV_Position
    {
        float4 basis = SampleBasis(texcoord);
        float3 normal = StereoInverseProjection(basis.xy);
        float3 binormal = StereoInverseProjection(basis.zw);

        float3 p = SamplePosition(texcoord);
        p += binormal * LineWidth(texcoord) * position.x;
        return UnityObjectToClipPos(float4(p, 1));
    }

    half4 Fragment() : SV_Target { return 1; }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Cull off
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            Tags { "LightMode"="ShadowCaster" }
            Cull off
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 3.0
            ENDCG
        }
    }
}
