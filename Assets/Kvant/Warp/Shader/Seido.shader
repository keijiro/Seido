Shader "Kvant/Warp/Seido"
{
    CGINCLUDE

    #include "Common.cginc"

    half3 _Emission;
    float _NormalizedTime;

    struct Varyings
    {
        float4 position : SV_Position;
        half4 color : COLOR;
    };

    Varyings Vertex(
        float4 position : POSITION,
        float3 texcoord : TEXCOORD0
    )
    {
        float3 p = ApplyLineParams(position.xyz, texcoord);
        p += GetLinePosition(texcoord, _NormalizedTime);

        Varyings output;
        output.position = UnityObjectToClipPos(p);
        output.color = half4(texcoord.x, 1, 1, 1);
        return output;
    }

    half4 Fragment(Varyings input) : SV_Target
    {
        return input.color;
    }

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
