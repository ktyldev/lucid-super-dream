// This shader draws a texture on the mesh.
Shader "custom/tunnel"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    { 
        _BackgroundColor("Background Color", Color) = (0,0,0,0)
        [HDR] _NebulaColor1("Nebula Color 1", Color) = (1,1,1,1)
        [HDR] _NebulaColor2("Nebula Color 2", Color) = (1,1,1,1)
        [HDR] _FractalColor("Fractal Color", Color) = (1,1,1,1)
        [HDR] _GroundColor("Ground Color", Color) = (1,1,1,1)
        
        _FractalWeight("Fractal Weight", Range(0.0,1.0)) = 1.0
        _NebulaWeight("Nebula Weight", Range(0.0,1.0)) = 1.0
        _BarsWeight("Bars Weight", Range(0.0,1.0)) = 1.0
        _StarsWeight("Stars Weight", Range(0.0,1.0)) = 1.0
        _GroundWeight("Ground Weight", Range(0.0,1.0)) = 1.0
        
        _FractalScale("Fractal Scale", Float) = 6.0
        _FractalRotateSpeed("Fractal Rotate Speed", Float) = 20.0
        _FractalPower("Fractal Power", Float) = 1.0
        _FractalInner("Fractal Inner", Float) = 0.2
        
        _Nebula1Power("Nebula 1 Power", Float) = 1.0
        _Nebula2Power("Nebula 2 Power", Float) = 1.0
        
        _NebulaMap("Nebula Map", 2D) = "black"
        _StarMap("Star Map", 2D) = "black"
        _FractalMap("Fractal Map", 2D) = "black"
        
//        _ShakeStrength("Shake Strength", Float) = 0.01
        
        _Intensity("Intensity", Float) = 1.0
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
            
            float4 _BackgroundColor;
            float4 _NebulaColor1;
            float4 _NebulaColor2;
            float4 _FractalColor;
            float4 _GroundColor;
            
            float _FractalScale = 6.0;
            float _FractalRotateSpeed = 20.0;
            float _FractalPower = 1.0;
            float _FractalInner = 0.2;
            float _FractalWeight = 1.0;

            float _Nebula1Power = 1.0;
            float _Nebula2Power = 1.0;
            float _NebulaWeight = 1.0;
            
            float _BarsWeight;
            float _StarsWeight;
            float _GroundWeight;
            
            float _Intensity;
            float _CameraShake = 0.0;

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
            
            float4 nebula(float r, float a)
            {
                // sample nebula 1
                float n1_stretch = 1.0/50.0;
                float n1_speed = 20.0;
                float n1_spin = 10.0;
                // float n1_power = 2.0;
                float n1_cutoff = 0.5;
                
                float2 nuv1;
                nuv1.x = n1_stretch/(r*r) + _Time * n1_speed;
                nuv1.y = a/PI + _Time * n1_spin;
                half4 nebula1 = SAMPLE_TEXTURE2D(_NebulaMap, sampler_NebulaMap, nuv1);
                nebula1 *= _NebulaColor1;
                nebula1 *= max(0,_Nebula1Power*r-n1_cutoff);
                
                // sample nebula 2 - same as nebula 1 but in another direction
                float n2_stretch = 1.0/100.0;
                float n2_speed = 30.0;
                float n2_spin = -5.0;
                // float n2_power = 4.0;
                float n2_cutoff = 0.5;

                float2 nuv2;
                nuv2.x = n2_stretch/(r*r) + _Time * n2_speed;
                nuv2.y = a/PI + _Time * n2_spin;
                half4 nebula2 = SAMPLE_TEXTURE2D(_NebulaMap, sampler_NebulaMap, nuv2.yx);
                nebula2 *= _NebulaColor2;
                nebula2 *= max(0,_Nebula2Power*r - n2_cutoff);

                return (nebula1 + nebula2) * _Intensity * _NebulaWeight;
            }

            float4 stars(float r, float a)
            {
                // sample stars
                float starStretch = 1.0/100.0;
                float starSpeed = 10.0 * _Intensity;
                float starWrap = 3.5f;
                
                // star uv
                float2 suv;
                // use the root to make them into radial lines instead of a tunnel
                suv.x = starStretch/sqrt(r) + _Time * starSpeed;
                suv.y = (float)starWrap*a/PI;
                half4 star = SAMPLE_TEXTURE2D(_StarMap, sampler_StarMap, suv);
                star *= r * r * 2.0;

                return star * _Intensity * _StarsWeight;
            }
            
            // same as fractals but calling it from a function turns them into bars?? 
            float4 bars(float r, float p)
            {
                // fractal uv
                float2 fuv1 = p / _FractalScale + float2(0.5,0.5);
                fuv1 = rotateUV(fuv1,_Time*_FractalRotateSpeed);
                half4 fractal1 = SAMPLE_TEXTURE2D(_FractalMap, sampler_FractalMap, fuv1);
                fractal1 *= _FractalColor;
                fractal1 *= max(0, r-_FractalInner);
                fractal1 *= _FractalPower;

                // fractal 2
                float2 fuv2 = p / _FractalScale + float2(0.5,0.5);
                fuv2 = rotateUV(fuv2,-_Time*_FractalRotateSpeed*3.561);
                fuv2.x=1.0-fuv2.x;
                half4 fractal2 = SAMPLE_TEXTURE2D(_FractalMap, sampler_FractalMap, fuv2);
                fractal2 *= _FractalColor * sin(_Time*3);
                fractal2 *= max(0, r-_FractalInner);
                fractal2 *= _FractalPower;

                half4 fractal = max(fractal1,fractal2) * _Intensity;
                
                return fractal * _Intensity * _BarsWeight;
            }

            float4 ground(float2 p, float r, float a)
            {
                float4 nothing = float4(0,0,0,0);
                float angle = PI;

                float t = (abs(a/PI)*PI) * -min(p.y, 0);
                t = min(r*0.4, t);
                // t += ;
                // t *= (1.0-r*0.5);

                // float y = -p.y;
                
                return lerp(nothing, _GroundColor, t) * _GroundWeight;
            }
            
            float4 frag(Varyings IN) : SV_Target
            {
                float2 res = _ScreenParams;
                float2 p = -1.0+2.0*IN.positionHCS.xy/res.xy;
                p.x *= res.x/res.y;

                float2 cameraShake;
                float shake = _Time*50;
                cameraShake.x = cos(shake*12.3341)+sin(shake*19.231057);
                cameraShake.y = cos(shake*17.12311)+sin(shake*14.2315165);
                cameraShake*=_CameraShake;
                p += cameraShake;

                // shared tunnel vars
                float r = length(p);
                float a = atan(p.y/p.x);

                // for some reason trying to sample the fractal in a function turns it into bars
                // looks cool but no idea why it happens so these fractals are inline
                float2 fuv1 = p / _FractalScale + float2(0.5,0.5);
                fuv1 = rotateUV(fuv1,_Time*_FractalRotateSpeed);
                half4 fractal1 = SAMPLE_TEXTURE2D(_FractalMap, sampler_FractalMap, fuv1);
                fractal1 *= _FractalColor;
                fractal1 *= max(0, r-_FractalInner);
                fractal1 *= _FractalWeight;
                fractal1 *= _Intensity;
                fractal1 *= max(2.0*normalize(p).y+0.25,0);
                
                float2 fuv2 = p / _FractalScale + float2(0.5,0.5);
                fuv2 = rotateUV(fuv2,-_Time*_FractalRotateSpeed*3.561);
                fuv2.x=1.0-fuv2.x;
                half4 fractal2 = SAMPLE_TEXTURE2D(_FractalMap, sampler_FractalMap, fuv2);
                fractal2 *= _FractalColor * sin(_Time*3);
                fractal2 *= max(0, r-_FractalInner);
                fractal2 *= _FractalWeight;
                fractal2 *= _Intensity;
                fractal2 *= max(2.0*normalize(p).y+0.25,0);
                
                float4 color = _BackgroundColor;
                color += fractal1;
                color += fractal2;

                color -= ground(p, r, a);
                
                color += nebula(r, a);
                color += bars(r, p);
                color += stars(r, a);

                // bars, nebula, fractal, 
                // average across all fx
                
                color = saturate(color);
                
                return color;
            }
            ENDHLSL
        }
    }
}
