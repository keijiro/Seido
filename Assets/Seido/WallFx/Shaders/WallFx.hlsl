#if defined(WALLFX_SLITLINES)

half Mask(float2 uv)
{
    float freq = 4;
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
    float reso = 60;
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
    half n = snoise(half2(1, _LocalTime * 13.12));
    return uv.x < _Amplitude * (1 + n * 0.5);
}

#elif defined(WALLFX_SQUARES)

half Mask(float2 uv)
{
    float2 p = uv * float2(16, 9);
    float2 p_c = floor(p) + 0.5;

    float2 p_f = abs(p - p_c);
    float dist = max(p_f.x, p_f.y);

    half n = snoise(half2(p_c * 0.881 + _LocalTime * 3.82));
    float thresh = (0.5 + n * 0.3) * _Amplitude;

    return 1 - smoothstep(thresh - 0.1, thresh, dist);
}

#else

half Mask(float2 uv)
{
    return 0;
}

#endif
