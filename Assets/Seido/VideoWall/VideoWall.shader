Shader "Seido/VideoWall"
{
    Properties
    {
        [HideInInspector] _MainTex("Texture", 2D) = "black" {}
        _Repeat("X Repeat", Float) = 1
        _Scroll("Scroll Speed", Float) = 1
    }

    HLSLINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float _Repeat;
    float _Scroll;

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
        float2 uv = input.texcoord;
        uv.x = frac(uv.x * _Repeat + _Scroll * _Time.y);
        half4 c = tex2D(_MainTex, uv);
        c.rgb = LinearToGammaSpace(c.rgb);
        return c;
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
}
