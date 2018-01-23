#include "UnityCG.cginc"

sampler2D _MainTex;
float _Width;

void Vertex(
    float4 position : POSITION, float2 texcoord : TEXCOORD,
    out float4 outPosition : SV_Position, out float2 outTexcoord : TEXCOORD
)
{
    outPosition = UnityObjectToClipPos(position);

#if defined(VIDEOWALL_VERTICAL)
    texcoord.xy = 1 - float2(texcoord.y, texcoord.x);
#endif

#if defined(VIDEOWALL_OFFSET)
    texcoord.x += VIDEOWALL_OFFSET;
#endif

    outTexcoord.x = (texcoord.x - 0.5) / _Width + 0.5;
    outTexcoord.y = texcoord.y;
}

half4 Fragment(
    float4 position : SV_Position, float2 texcoord : TEXCOORD
) : SV_Target
{
    return (all(texcoord >= 0) && all(texcoord < 1)) ? tex2D(_MainTex, texcoord) : 0;
}
