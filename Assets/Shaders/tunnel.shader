// This shader draws a texture on the mesh.
Shader "custom/tunnel"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    { 
        [HDR] _NebulaColor1("Nebula Color 1", Color) = (1,1,1,1)
        [HDR] _NebulaColor2("Nebula Color 2", Color) = (1,1,1,1)
        [HDR] _FractalColor("Fractal Color", Color) = (1,1,1,1)
        
        _NebulaMap("Nebula Map", 2D) = "black"
        _StarMap("Star Map", 2D) = "black"
        _FractalMap("Fractal Map", 2D) = "black"
        
        _OverallPower("Overall Power", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            struct Attributes
            {
                float4 positionOS   : POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
            };
            
            float4 _NebulaColor1;
            float4 _NebulaColor2;
            float4 _FractalColor;

            float _OverallPower;

            TEXTURE2D(_NebulaMap);
            SAMPLER(sampler_NebulaMap);
            TEXTURE2D(_StarMap);
            SAMPLER(sampler_StarMap);
            TEXTURE2D(_FractalMap);
            SAMPLER(sampler_FractalMap);

            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseMap_ST variable, so that you
                // can use the _BaseMap variable in the fragment shader. The _ST 
                // suffix is necessary for the tiling and offset function to work.
                float4 _NebulaMap_ST;
                float4 _StarMap_ST;
                float4 _FractalMap_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }


            float2 rotateUV(float2 uv, float rotation)
            {
                float mid = 0.5;
                return float2(
                    cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
                    cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
                );
            }            
                
            
            half4 frag(Varyings IN) : SV_Target
            {
                float2 res = _ScreenParams;
                float2 p = -1.0+2.0*IN.positionHCS.xy/res.xy;
                p.x *= res.x/res.y;

                // shared tunnel vars
                float r = length(p);
                float a = atan(p.y/p.x);
                
                // sample nebula 1
                float n1_stretch = 1.0/50.0;
                float n1_speed = 20.0;
                float n1_spin = 10.0;
                float n1_power = 2.0;
                float n1_cutoff = 0.5;
                
                float2 nuv1;
                nuv1.x = n1_stretch/(r*r) + _Time * n1_speed;
                nuv1.y = a/PI + _Time * n1_spin;
                half4 nebula1 = SAMPLE_TEXTURE2D(_NebulaMap, sampler_NebulaMap, nuv1);
                nebula1 *= _NebulaColor1;
                nebula1 *= max(0,n1_power*r-n1_cutoff);
                
                // sample nebula 2 - same as nebula 1 but in another direction
                float n2_stretch = 1.0/100.0;
                float n2_speed = 30.0;
                float n2_spin = -5.0;
                float n2_power = 4.0;
                float n2_cutoff = 0.5;

                float2 nuv2;
                nuv2.x = n2_stretch/(r*r) + _Time * n2_speed;
                nuv2.y = a/PI + _Time * n2_spin;
                half4 nebula2 = SAMPLE_TEXTURE2D(_NebulaMap, sampler_NebulaMap, nuv2.yx);
                nebula2 *= _NebulaColor2;
                nebula2 *= max(0,n2_power*r - n2_cutoff);
                
                // sample stars
                float starStretch = 1.0/100.0;
                float starSpeed = 10.0;
                int starWrap = 3;
                
                // star uv
                float2 suv;
                // use the root to make them into radial lines instead of a tunnel
                suv.x = starStretch/sqrt(r) + _Time * starSpeed;
                suv.y = (float)starWrap*a/PI;
                half4 star = SAMPLE_TEXTURE2D(_StarMap, sampler_StarMap, suv);
                star *= r * r * 2.0;
                
                // sample fractal
                float fractalScale = 4.0;
                float fractalRotateSpeed = 20.0;
                float fractalPower = 1.0;
                float fractal_inner = 0.2;
                
                // fractal uv
                float2 fuv = p / fractalScale + float2(0.5,0.5);
                fuv = rotateUV(fuv,_Time*fractalRotateSpeed);
                half4 fractal = SAMPLE_TEXTURE2D(_FractalMap, sampler_FractalMap, fuv);
                fractal *= _FractalColor;
                fractal *= max(0, r-fractal_inner);
                fractal *= fractalPower;
                
                half4 color = half4(0,0,0,0);
                color += nebula1;
                color += nebula2;
                color += fractal;
                color += star;

                saturate(color);
                color *= _OverallPower;
                
                return color;
            }
            ENDHLSL
        }
    }
}