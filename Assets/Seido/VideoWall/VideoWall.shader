Shader "Hidden/Seido/VideoWall"
{
    Properties
    {
        _MainTex("", 2D) = "black" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "VideoWall.hlsl"
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #define VIDEOWALL_VERTICAL
            #define VIDEOWALL_OFFSET 0.5
            #include "VideoWall.hlsl"
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #define VIDEOWALL_VERTICAL
            #define VIDEOWALL_OFFSET -0.5
            #include "VideoWall.hlsl"
            ENDHLSL
        }
    }
}
