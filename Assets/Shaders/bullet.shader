// This shader draws a texture on the mesh.
Shader "custom/bullet"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    { 
        [HDR] _Color1("Color1", Color) = (1,1,1,1)
        [HDR] _Color2("Color2", Color) = (1,1,1,1)
        [HDR] _FarColor("Far Color", Color) = (1,1,1,1)
        
        _Alpha("Alpha", Float) = 1.0
//        [HDR] _Color2("Color 2", Color) = (1,1,1,1)
//        _FadeStrength("Fade Strength", Float) = 0.1
//        _VertexScale("Vertex Scale", Float) = 0
        _PulseIntensity("Pulse Intensity", Float) = 1.0
        _TrackWidth("Track Width", Float) = 20
        
        _M("M", Float) = 0.5
        _C("C", Float) = 0.0
        
        _NoiseMap("Noise Map", 2D) = "black"
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
            float4 _FarColor;
            float _Alpha;
            float _PulseIntensity;
            

            // float _VertexScale;
            // float _FadeStrength;
            
            // This macro declares _BaseMap as a Texture2D object.
            TEXTURE2D(_BaseMap);
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);

            CBUFFER_START(UnityPerMaterial)
                float _DistanceToNextBeat;
                float _DistanceFromLastBeat;
                float _TrackWidth;
                float _PlayerXPos;
            
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
                float radius = 4.0;
                float d = _TrackWidth;
                float r = radius - vpos.y;
                float x = wpos.x;
                x -= _PlayerXPos;

                float a = 2*PI*(x/d+0.5);
                a += 0.5*PI;

                wpos = float3(cos(a)*r,sin(a)*r, wpos.z);
                
                vpos = TransformWorldToObject(wpos);

                OUT.normal = IN.normal;
                OUT.wpos = wpos;
                OUT.positionHCS = TransformObjectToHClip(vpos);
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // calculate pixel
                float2 res = _ScreenParams.xy;
                float2 p = -1.0+2.0*IN.positionHCS.xy/res.xy;

                float2 toCentre = TransformObjectToWorld(-IN.wpos).xy;
                toCentre = normalize(toCentre);

                float t = dot(IN.normal.xy, toCentre) * 0.5 + 0.5;
                
                // float t = dot(IN.normal, float3(0,1,0)) * 0.5 + 0.5;
                
                // float t = length(p);
                // return lerp(_Color1,_Color2,t * _FadeStrength);
                float4 c = float4(lerp(_Color1, _Color2, t).xyz, _Alpha);
                c = lerp(c, _FarColor, IN.wpos.z * 0.0075);
                
                return c;
            }
            ENDHLSL
        }
    }
}