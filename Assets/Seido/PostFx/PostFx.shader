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

    float _LocalTime;
    half4 _LineColor;
    half3 _FillColor1;
    half3 _FillColor2;
    half3 _FillColor3;

    half _ColorThreshold;
    half _DepthThreshold;

    half4 _GradientA;
    half4 _GradientB;
    half4 _GradientC;
    half4 _GradientD;

    half3 Hue2RGB(half h)
    {
        h = frac(saturate(h)) * 6 - 2;
        half3 rgb = saturate(half3(abs(h - 1) - 1, 2 - abs(h), 2 - abs(h - 2)));
        rgb = GammaToLinearSpace(rgb);
        return rgb;
    }

    // Select color for posterization
    fixed3 SelectColor(float x, fixed3 c1, fixed3 c2, fixed3 c3)
    {
        return x < 1 ? c1 : (x < 2 ? c2 : c3);
    }

    // Dithering with the 3x3 Bayer matrix
    fixed Dither3x3(float2 uv)
    {
        const float3x3 pattern = float3x3(0, 7, 3, 6, 5, 2, 4, 1, 8) / 9 - 0.5;
        uint2 iuv = uint2(uv * _MainTex_TexelSize.zw) % 3;
        return pattern[iuv.x][iuv.y];
    }

    // Edge detection with the Roberts cross operator
    fixed DetectEdge(float2 uv)
    {
        float4 duv = float4(0, 0, _MainTex_TexelSize.xy);

        float c11 = tex2D(_MainTex, uv + duv.xy).g;
        float c12 = tex2D(_MainTex, uv + duv.zy).g;
        float c21 = tex2D(_MainTex, uv + duv.xw).g;
        float c22 = tex2D(_MainTex, uv + duv.zw).g;

        float g_c = length(float2(c11 - c22, c12 - c21));
        g_c = saturate((g_c - _ColorThreshold) * 40);

        float d11 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv + duv.xy);
        float d12 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv + duv.zy);
        float d21 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv + duv.xw);
        float d22 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv + duv.zw);

        float g_d = length(float2(d11 - d22, d12 - d21));
        g_d = saturate((g_d - _DepthThreshold) * 40);

        return max(g_c, g_d);
    }

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
        fixed3 c_src = tex2D(_MainTex, uv).rgb; // source color

        // Edge detection and posterization
        fixed edge = DetectEdge(uv);
        fixed luma = LinearRgbToLuminance(c_src);
        fixed sel = luma * 3 + Dither3x3(uv);
        fixed3 fill = SelectColor(sel, _FillColor3, _FillColor3, _FillColor3);
        fixed3 c_out = lerp(fill, _LineColor.rgb, edge * _LineColor.a);

        float gr = 0.5 + snoise(float3(uv *float2(4, 1) * 0.12, _Time.y * 0.05));
        //c_out = lerp(c_out, saturate(_GradientA + _GradientB * cos(_GradientC * gr + _GradientD)), dot(c_src, 1) == 0);
        gr += (dot(c_src, 1) == 0) * 0.4;
        c_out = saturate(_GradientA + _GradientB * cos(_GradientC * gr + _GradientD));
        c_out = lerp(c_out, _LineColor.rgb, edge * _LineColor.a);

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
