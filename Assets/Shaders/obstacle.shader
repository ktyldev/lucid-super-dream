// This shader draws a texture on the mesh.
Shader "custom/obstacle"
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
        
        _HorizontalScale("Horizontal Scale", Float) = 0.1
        _VerticalScale("Vertical Scale", Float) = 10
        
        _M("M", Float) = 0.5
        _C("C", Float) = 0.0
        
        _NoiseMap("Noise Map", 2D) = "black"
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            Blend OneMinusDstColor OneMinusSrcAlpha
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
            float _PulseIntensity;

            float _SpeedMultiplier;
            float _RadiusWithDistance;
            
            float _TrackWidth;
            float _PlayerXPos;
            
            float _DistanceToNextBeat;
            float _DistanceSinceLastBeat;
            
            float _HorizontalScale;
            float _VerticalScale;

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

                       //random hash
            float4 hash42(float2 p) {

                float4 p4 = frac(float4(p.xyxy) * float4(443.8975, 397.2973, 491.1871, 470.7827));
                p4 += dot(p4.wzxy, p4 + 19.19);
                return frac(float4(p4.x * p4.y, p4.x * p4.z, p4.y * p4.w, p4.x * p4.w));
            }

            float hash(float n) {
                return frac(sin(n) * 43758.5453123);
            }

            float n(in float3 x) {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);
                float n = p.x + p.y * 57.0 + 113.0 * p.z;
                float res = lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
                    lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
                    lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
                        lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
                return res;
            }

            //tape noise
            float tape_noise(float2 p) {

                float y = p.y;
                float s = _Time.x + 200;

                float v = (n(float3(y * .01 + s, 1., 1.0)) + .0)
                    * (n(float3(y * .011 + 1000.0 + s, 1., 1.0)) + .0)
                    * (n(float3(y * .51 + 421.0 + s, 1., 1.0)) + .0)
                    ;

                v *= hash42(float2(p.x + _Time.y + 200, p.y)).x + 2;


                v = abs(pow(v + .3, 1.));
                if (v < .5) v = 0.;  //threshold
                return v;
            }
 
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float radius = _BaseTubeRadius;
                float d = _TrackWidth;

                float3 vpos = IN.positionOS.xyz;
                vpos*=3;
                OUT.normal = IN.normal;

                // position of the model's origin in world space
                float3 mo_wpos = TransformObjectToWorld(float3(0,0,0));
                
                float beat = _DistanceToNextBeat*_DistanceToNextBeat;
                vpos *= 1.0+beat*_PulseIntensity;

                // float3 wpos = TransformObjectToWorld(vpos);
                float wpz = mo_wpos.z;
                float r = length(radius) + wpz*wpz*wpz*_RadiusWithDistance;
                
                // compress horiztonally
                // vpos.x = vpos.x * _HorizontalScale;
                // vpos.y = vpos.y * _VerticalScale * 1.0-(r-radius);
                // vpos = mul(UNITY_MATRIX_P,
                //     mul(UNITY_MATRIX_MV, float4(0,0,0,1))
                //     + float4(vpos.x, vpos.y, 0,0)
                //     + float4(1, 1,1,1));

                // recalculate wpos with updated vpos
                // wpos = TransformObjectToWorld(vpos);

                // matrix m = GetObjectToWorldMatrix();
                
                //float d = 1.0+length(wpos)*0.5;
                //float2 uv = wpos.zx;
                // vpos += _VertexScale*float3(0,0,5)*d;
                // vpos += sin(180)*sin(1800)*float3(0,0,5)*d;
                // noise *= d*d * 0.1;
                // vpos *= lerp (0.9,1.1,noise);

                // float3 owpos = TransformObjectToWorld(float3(0,0,0));
                
                // calculate position on circle
                
                // float3 object_origin = TransformObjectToWorld(float3(0,0,0));
                // float r_origin = float
                
                // float wpz = abs(mo_wpos.z-6); // offset due to player position
                // // wpz *= wpz;
                // r += wpz*_RadiusWithDistance;
                // mo_wpos.z *= _SpeedMultiplier;

                float x = mo_wpos;
                // float x = mo_wpos.x+vpos.x;
                x -= _PlayerXPos;
                // x += 0.5*PI;
                
                float a = 2*PI*(x/d);
                a -= 0.5*PI;
                // a *= (radius - r) * -1.0;

                // vpos.y += 0.5*radius*cos(a);
                
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

                float boundsStrength = 5.0*_Intensity;
                float bounce = (_SpeedMultiplier+boundsStrength*(_DistanceToNextBeat*_DistanceSinceLastBeat));
                wpos.z *= bounce;
                
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
                float normalised = IN.wpos.z/500.0;
                // float4 c = float4(lerp(_Color1, _Color2, t).xyz, _Alpha);
                float4 c = _Color1;
                //c *= tape_noise(IN.polar/0.00005);
                float noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, IN.wpos.yz);
                // c *= noise;
                
                c = lerp(c, _FarColor, (_DistanceToNextBeat*_Intensity + noise));
                c.a = 0.0;
                
                return c;
            }
            ENDHLSL
        }
    }
}