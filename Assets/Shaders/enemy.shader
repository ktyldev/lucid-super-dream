// This shader draws a texture on the mesh.
Shader "custom/enemy"
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
        _RadiusWithDistance("Radius with Distance", Float) = 0.003
        _SpeedMultiplier("Speed Multiplier", Float) = 2.0
        
        _BaseScale("Base Scale", Float) = 1.0
        
        _M("M", Float) = 0.5
        _C("C", Float) = 0.0
        
        _NoiseMap("Noise Map", 2D) = "black"
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
//            Blend OneMinusDstColor OneMinusSrcAlpha
//            Blend SrcAlpha
            
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
                float2 polar        : TEXCOORD2; // TODO!!
            };

            float _M = 0.5;
            float _C = 0.0;
            float4 _Color1;
            float4 _Color2;
            float4 _FarColor;
            float _Alpha;

            float _SpeedMultiplier;
            float _RadiusWithDistance;
            
            float _TrackWidth;
            float _PlayerXPos;
            
            float _DistanceToNextBeat;
            float _DistanceSinceLastBeat;
            
            float _BaseScale;
            float _PulseIntensity;

            float _BaseTubeRadius;
            float _Intensity;
            
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
                float radius = _BaseTubeRadius;
                float d = _TrackWidth;

                float3 vpos = IN.positionOS.xyz;
                vpos*=2;
                OUT.normal = IN.normal;

                // position of the model's origin in world space
                float3 mo_wpos = TransformObjectToWorld(float3(0,0,0));

                float db = _DistanceSinceLastBeat;
                float beat = db*db;
                vpos *= _BaseScale+beat*_PulseIntensity;

                // float3 wpos = TransformObjectToWorld(vpos);
                float wpz = mo_wpos.z;
                float r = length(radius) + wpz*wpz*wpz*_RadiusWithDistance;
                
                float x = mo_wpos;
                x -= _PlayerXPos;
                
                float a = 2*PI*(x/d);
                a -= 0.5*PI;
                
                float3 wpos = float3(cos(a)*r,sin(a)*r,mo_wpos.z);
                
                float x_p = vpos.x*cos(a)-vpos.y*sin(a);
                float y_p = vpos.x*sin(a)+vpos.y*cos(a);
                vpos.x = x_p;
                vpos.y = y_p;

                float noise1 = SAMPLE_TEXTURE2D_LOD(_NoiseMap, sampler_NoiseMap, wpos.yz, 0) - 0.5;
                float noise2 = SAMPLE_TEXTURE2D_LOD(_NoiseMap, sampler_NoiseMap, wpos.xy, 0) - 0.5;
                wpos += float3(noise1, noise2, 0) * 2.0;
                wpos += float3(vpos.xy,0);
                wpos.y += radius;

                float bounceStrength=5.0*_Intensity;
                float bounce = (_SpeedMultiplier+bounceStrength*(_DistanceToNextBeat*_DistanceSinceLastBeat));
                wpos.z *= bounce;
                // wpos.z *= max(1, (wpos.z-6)* _SpeedMultiplier);
                vpos = TransformWorldToObject(wpos);

                // TransformWorldToObject()
                // vpos += wposOffset;

                // float x_with_distance = normalize(wpos.x)*wpos.z*0.1;
                // x_with_distance = abs(x_with_distance);
                // vpos += float3(x_with_distance,0,0);
                
                OUT.wpos = wpos;
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
                float normalised = IN.wpos.z/1000.0;
                // float4 c = float4(lerp(_Color1, _Color2, t).xyz, _Alpha);
                float4 c = _Color1;
                c = lerp(_Color1, _Color2, length(p));
                
                c = lerp(c, _FarColor, normalised);

                // float distanceAhead = IN.wpos.z - 6;
                // float ca = clamp(distanceAhead * 0.1,0,1);
                // c.a = ca;
                // c.a = clamp((IN.wpos.z-6)*1.0,0,1);
                // c.a = clamp((1.0-IN.polar.r),0,1);
                // c.a = clamp(1.0-normalised,0,1);
                
                return c;
            }
            ENDHLSL
        }
    }
}