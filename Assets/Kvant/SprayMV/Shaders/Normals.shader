Shader "Kvant/SprayMV/Normals"
{
    Properties
    {
        [HideInInspector] _PositionBuffer("", 2D) = "black"{}
        [HideInInspector] _RotationBuffer("", 2D) = "red"{}
    }

    CGINCLUDE

    #include "Common.cginc"

    struct Varyings
    {
        float4 position : SV_Position;
        half3 normal : NORMAL;
    };

    Varyings Vertex(
        float4 position : POSITION,
        float2 texcoord : TEXCOORD1,
        half3 normal : NORMAl
    )
    {
        float4 uv = float4(texcoord, 0, 0);

        float4 p = tex2Dlod(_PositionBuffer, uv);
        float4 r = tex2Dlod(_RotationBuffer, uv);

        float l = p.w + 0.5;
        float s = ScaleAnimation(uv, l);

        position.xyz = RotateVector(position.xyz, r) * s + p.xyz;

        Varyings output;
        output.position = UnityObjectToClipPos(position);
        output.normal = RotateVector(normal, r);
        return output;
    }

    half4 Fragment(Varyings input) : SV_Target
    {
        return half4((input.normal + 1) / 2, 1);
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
