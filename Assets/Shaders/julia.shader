// This shader draws a texture on the mesh.
Shader "custom/julia"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    { 
        _Color1("Color 1", Color) = (1,1,1,1)
        _Color2("Color 2", Color) = (1,1,1,1)
        
        _BassPower("Bass Power", Float) = 0.0
        
        _Fractality("Fractality", Float) = 1.02
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

            float4 _Color1;
            float4 _Color2;
            float _Fractality;
            float _BassAmplitude;
            
            // This macro declares _BaseMap as a Texture2D object.
            TEXTURE2D(_BaseMap);
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseMap_ST variable, so that you
                // can use the _BaseMap variable in the fragment shader. The _ST 
                // suffix is necessary for the tiling and offset function to work.
                float4 _BaseMap_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // calculate pixel
                float2 res = _ScreenParams;
                float2 p = -1.0+2.0*IN.positionHCS.xy/res.xy;

                float2 z = p * 1.5;
                float2 c;
                float an = _Time + _BassAmplitude;
                c.x = 0.5*cos(an) - 0.25*cos(2.0*an);
                c.y = 0.5*sin(an) - 0.25*sin(2.0*an);
                c *= _Fractality; 
                
                float t = 0.0;
                int iterations = 64;
                for (int i = 0; i < iterations; ++i)
                {
                    float2 nz = float2(z.x*z.x-z.y*z.y,2.0*z.x*z.y) + c;
                    float m2 = dot(nz,nz);
                    if (m2 > 4.0) break;
                    z = nz;
                    t+= 1.0 / (iterations-1.0);
                }
                
                return lerp(_Color1,_Color2,t);
            }
            ENDHLSL
        }
    }
}