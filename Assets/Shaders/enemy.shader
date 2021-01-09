// This shader draws a texture on the mesh.
Shader "custom/enemy"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    { 
        [HDR] _Color1("Color1", Color) = (1,1,1,1)
        [HDR] _Color2("Color2", Color) = (1,1,1,1)
        _Alpha("Alpha", Float) = 1.0
//        [HDR] _Color2("Color 2", Color) = (1,1,1,1)
//        _FadeStrength("Fade Strength", Float) = 0.1
//        _VertexScale("Vertex Scale", Float) = 0
        
        _M("M", Float) = 0.5
        _C("C", Float) = 0.0
        
        _NoiseMap("Noise Map", 2D) = "black"
        _VertexDistortion("Vertex Distortion", Float) = 1.0
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
                half3 normal        : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
                float3 wpos         : TEXCOORD1;
                half3 normal        : NORMAL;
            };

            float _M = 0.5;
            float _C = 0.0;
            float4 _Color1;
            float4 _Color2;
            float _Alpha;
            float _TimeToNextBeat;

            float _VertexDistortion;
            // float _VertexScale;
            // float _FadeStrength;
            
            // This macro declares _BaseMap as a Texture2D object.
            TEXTURE2D(_BaseMap);
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);

            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseMap_ST variable, so that you
                // can use the _BaseMap variable in the fragment shader. The _ST 
                // suffix is necessary for the tiling and offset function to work.
                float4 _BaseMap_ST;
                float4 _NoiseMap_ST;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                float3 vpos = IN.positionOS.xyz;
                float3 wpos = TransformObjectToWorld(vpos);
                OUT.wpos = wpos;
                OUT.normal = IN.normal;
                
                float d = 1.0+length(wpos)*0.5;
                float2 uv = wpos.zx;
                float noise = SAMPLE_TEXTURE2D_LOD(_NoiseMap, sampler_NoiseMap, uv, 0) - 0.5;
                
                // vpos += _VertexScale*float3(0,0,5)*d;
                vpos += float3(_VertexDistortion,0,0)*noise*d;
                // vpos += sin(180)*sin(1800)*float3(0,0,5)*d;

                // noise *= d*d * 0.1;
                
                // vpos *= lerp (0.9,1.1,noise);
                
                OUT.positionHCS = TransformObjectToHClip(vpos);
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

                float2 toCentre = TransformObjectToWorld(-IN.wpos).xy;
                toCentre = normalize(toCentre);

                float t = dot(IN.normal, toCentre) * 0.5 + 0.5;
                
                // float t = dot(IN.normal, float3(0,1,0)) * 0.5 + 0.5;
                
                // float t = length(p);
                // return lerp(_Color1,_Color2,t * _FadeStrength);
                float4 c = float4(lerp(_Color1, _Color2, t).xyz, _Alpha);
                return c;
            }
            ENDHLSL
        }
    }
}