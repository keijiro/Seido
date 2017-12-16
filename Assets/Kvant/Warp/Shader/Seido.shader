Shader "Kvant/Warp/Seido"
{
    CGINCLUDE

    #include "Common.cginc"

    half3 _Emission;
    float _NormalizedTime;

    float4 Vertex(float4 position : POSITION, float3 texcoord : TEXCOORD0) : SV_Position
    {
        float3 p = ApplyLineParams(position.xyz, texcoord);
        p += GetLinePosition(texcoord, _NormalizedTime);
        return UnityObjectToClipPos(p);
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
        /*
        Pass
        {
            Tags { "LightMode"="ShadowCaster" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 3.0
            ENDCG
        }
        */
    }
}
