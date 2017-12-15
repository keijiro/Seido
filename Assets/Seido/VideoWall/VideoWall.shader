Shader "Hidden/Seido/VideoWall"
{
    Properties
    {
        [HideInInspector] _MainTex("Texture", 2D) = "black" {}
    }

    HLSLINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _Repeat;
    float2 _Offset;
    half _Opacity;

    struct Varyings
    {
        float4 position : SV_Position;
        float2 texcoord : TEXCOORD0;
    };

    Varyings Vertex(float4 position : POSITION, float2 texcoord : TEXCOORD0)
    {
        Varyings output;
        output.position = UnityObjectToClipPos(position);
        output.texcoord = texcoord;
        return output;
    }

    half4 Fragment(Varyings input) : SV_Target
    {
        float2 uv = frac(input.texcoord * _Repeat + _Offset);
        half3 c = tex2D(_MainTex, uv).rgb;
        // c = LinearToGammaSpace(c);
        return half4(c, _Opacity);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
}
