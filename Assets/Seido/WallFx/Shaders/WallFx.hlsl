#if defined(WALLFX_SLITLINES)

half Mask(float2 uv)
{
    float freq = 3;
    float width = 0.2 * _Amplitude;
    float t = _LocalTime * 3;

    half2 p1 = half2(uv.x * freq * 2, t);
    half2 p2 = half2(uv.x * freq * 1, t);
    half n = snoise(p1) + snoise(p2) / 2;

    half c1 = 1 - smoothstep(width * 0.99, width,  n);
    half c2 = 1 - smoothstep(width * 0.99, width, -n);
    return c1 * c2;
}

#elif defined(WALLFX_WAVEBARS)

half Mask(float2 uv)
{
    float reso = 50;
    float width = 0.6;

    float x = uv.x * reso;
    float x_c = floor(x) / reso;

    half2 p1 = half2(x_c * 3.31, _LocalTime * 4);
    half2 p2 = half2(x_c * 14.1, _LocalTime * 9);

    half n = snoise(p1) + snoise(p2) / 2;
    half p = abs(uv.y - 0.5) < n * _Amplitude;

    return p * (frac(x) < width);
}

#elif defined(WALLFX_SHUTTERS)

half Mask(float2 uv)
{
    float t = _LocalTime * 1.8;

    half n0 = snoise(half2(0, t)) * 0.7 + 0.5;
    half n1 = snoise(half2(1, t)) * 0.7 + 0.5;
    half n2 = snoise(half2(2, t)) * 0.7 + 0.5;
    half n3 = snoise(half2(3, t)) * 0.7 + 0.5;
    half n4 = snoise(half2(4, t)) * 0.7 + 0.5;
    half n5 = snoise(half2(5, t)) * 0.7 + 0.5;

    half th1 = lerp(lerp(n0, n1, saturate(uv.y * 2)), n2, saturate(uv.y * 2 - 1));
    half th2 = lerp(lerp(n3, n4, saturate(uv.y * 2)), n5, saturate(uv.y * 2 - 1));

    th1 *= _Amplitude;
    th2 *= _Amplitude;

    return abs((th1 > uv.x) - (th2 > 1 - uv.x));
}

#elif defined(WALLFX_SQUARES)

half Mask(float2 uv)
{
    float2 p = uv * float2(7, 4);
    float2 p_c = floor(p) + 0.5;

    float2 p_f = abs(p - p_c);
    float dist = max(p_f.x, p_f.y);

    half n = snoise(half2(p_c * 0.15 + _LocalTime * 1.4));
    return saturate(2 - abs((1 + n) / 3 * _Amplitude - 0.1 - dist) * 60);
}

#else

half Mask(float2 uv)
{
    return 0;
}

#endif
