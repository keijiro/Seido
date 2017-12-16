Shader "Hidden/Seido/PostFx"
{
    Properties
    {
        _MainTex("", 2D) = "black" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise3D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

    half4 _GradientA;
    half4 _GradientB;
    half4 _GradientC;
    half4 _GradientD;
    half _Frequency;
    half4 _LineColor;
    half _ColorThreshold;
    half _DepthThreshold;
    float _LocalTime;

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
        float2 uv00 = input.texcoord;
        float2 uv01 = input.texcoord + float2(_MainTex_TexelSize.x, 0);
        float2 uv10 = input.texcoord + float2(0, _MainTex_TexelSize.y);
        float2 uv11 = input.texcoord + _MainTex_TexelSize.xy;

        half c00 = tex2D(_MainTex, uv00).r;
        half c01 = tex2D(_MainTex, uv01).r;
        half c10 = tex2D(_MainTex, uv10).r;
        half c11 = tex2D(_MainTex, uv11).r;

        half g_c = length(half2(c00 - c11, c01 - c10));
        g_c = saturate((g_c - _ColorThreshold) * 40);

        float d00 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv00);
        float d01 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv01);
        float d10 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv10);
        float d11 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv11);

        half g_d = length(half2(d00 - d11, d01 - d10));
        g_d = saturate((g_d - _DepthThreshold) * 40);

        float3 np = float3(uv00 * float2(1, 0.25) * _Frequency, _LocalTime);

        half grad_x = 0.5 + snoise(np) / 2 + tex2D(_MainTex, uv00).g / 2;
        half3 grad = saturate(_GradientA + _GradientB * cos(_GradientC * grad_x + _GradientD));

        half3 c_out = lerp(grad, _LineColor.rgb, max(g_c, g_d) * _LineColor.a);
        return fixed4(GammaToLinearSpace(c_out), 1);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
