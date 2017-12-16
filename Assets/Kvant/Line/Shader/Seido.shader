Shader "Kvant/Line/Seido"
{
    Properties
    {
        [HideInInspector] _PositionBuffer("", 2D) = "black"{}
        [HideInInspector] _RotationBuffer("", 2D) = "red"{}
    }

    CGINCLUDE

    #include "Common.cginc"

    float4 Vertex(float4 position : POSITION, float2 texcoord : TEXCOORD1) : SV_Position
    {
        float4 p = SamplePosition(texcoord);
        float4 r = SampleRotation(texcoord);

        position.xyz = RotateVector(position.xyz, r) * p.w + p.xyz;
        return UnityObjectToClipPos(position);
    }

    half4 Fragment() : SV_Target { return 1; }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            Tags { "LightMode"="ShadowCaster" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 3.0
            ENDCG
        }
    }
}
