Shader "Hidden/CRTEffect"
{
    Properties
    {
        _MainTex          ("Texture",          2D)           = "white" {}
        _MonochromeStrength ("Monochrome",     Range(0, 1))  = 0.8
        _PhosphorTint     ("Phosphor Tint",    Color)        = (0.65, 1.0, 0.65, 1)
        _ScanlineIntensity("Scanline Intensity",Range(0, 1)) = 0.25
        _ScanlineCount    ("Scanline Count",   Float)        = 300
        _CurvatureAmount  ("Curvature",        Range(0, 0.5))= 0.08
        _VignetteIntensity("Vignette",         Range(0, 1))  = 0.45
        _NoiseAmount      ("Noise",            Range(0, 0.2))= 0.04
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        ZTest Always ZWrite Off Cull Off Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float     _MonochromeStrength;
            float4    _PhosphorTint;
            float     _ScanlineIntensity;
            float     _ScanlineCount;
            float     _CurvatureAmount;
            float     _VignetteIntensity;
            float     _NoiseAmount;

            // Barrel-distort UV to simulate curved CRT glass
            float2 BarrelDistort(float2 uv)
            {
                float2 c = uv * 2.0 - 1.0;
                c *= 1.0 + _CurvatureAmount * dot(c, c);
                return c * 0.5 + 0.5;
            }

            // Per-pixel animated noise
            float Hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = BarrelDistort(i.uv);

                // Black outside the curved screen boundary
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return fixed4(0, 0, 0, 1);

                fixed4 col = tex2D(_MainTex, uv);

                // --- Monochrome + phosphor tint ---
                float luma = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = lerp(col.rgb, luma * _PhosphorTint.rgb, _MonochromeStrength);

                // --- Scanlines ---
                // abs(sin) creates N evenly-spaced dark gaps; pow < 1 keeps bright bands wide
                float scanline = abs(sin(uv.y * _ScanlineCount * UNITY_PI));
                scanline = pow(scanline, 0.4);
                col.rgb *= 1.0 - _ScanlineIntensity * (1.0 - scanline);

                // --- Vignette ---
                float2 vigUV   = uv * 2.0 - 1.0;
                float  vignette = saturate(1.0 - dot(vigUV, vigUV) * _VignetteIntensity);
                col.rgb *= vignette;

                // --- Animated noise / static ---
                float2 pixelUV = floor(uv * _ScreenParams.xy) + floor(_Time.y * 15.0);
                float  noise   = Hash(pixelUV);
                col.rgb += (noise * 2.0 - 1.0) * _NoiseAmount;

                // --- Subtle 60 Hz brightness flicker ---
                col.rgb *= 1.0 + sin(_Time.y * 72.0) * 0.015;

                return saturate(col);
            }
            ENDCG
        }
    }
}
