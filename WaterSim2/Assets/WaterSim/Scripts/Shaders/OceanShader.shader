// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


Shader "Custom/OceanShader"
{
    Properties
    {
        //base fields
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _FlowMap("Flow (RG)", 2D) = "black" {}
        [NoScaleOffset] _DerivHeightMap("Deriv (AG) Height (B)", 2D) = "black" {}
        //texture fields
        _UJump("U jump per phase", Range(-0.25, 0.25)) = 0.25
        _VJump("V jump per phase", Range(-0.25, 0.25)) = 0.25
        _Tiling("Tiling", Float) = 1
        _Speed("Speed", Float) = 1
        _FlowStrength("Flow Strength", Range(0, 1)) = 1
        _FlowOffset("Flow Offset", Float) = 0
        _HeightScale("Height Scale, Constant", Float) = 0.25
        _HeightScaleModulated("Height Scale, Modulated", Float) = 0.75

        //sinewave noise
        _NoiseTex("Noise Texture", 2D) = "white" {}

        //standard fields
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        //fog fields
        _WaterFogDensity("Water Fog Density", Range(0, 2)) = 0.1
        _RefractionStrength("Refraction Strength", Range(0, 1)) = 0.25
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        GrabPass {"_WaterBackground"}

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert alpha addshadow finalcolor:ResetAlpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _FlowMap, _DerivHeightMap;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        //Water parameters
        sampler2D _NoiseTex;
        float _WaterScale;
        float _WaterSpeed;
        float _WaterDistance;
        float _WaterNoiseStrength;
        float _WaterNoiseWalk;


        //wavetype
        int _WaveType;
        float _WaterTime;

        //texture fields
        float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;
        float _HeightScale, _HeightScaleModulated;



        float a;
        //Fog fields
        sampler2D _CameraDepthTexture, _WaterBackground;
        float4 _CameraDepthTexture_TexelSize;
        float _WaterFogDensity;
        float _RefractionStrength;

        //Wave fields
        float4 _WaveA, _WaveB, _WaveC;

        struct Input
        {

            float2 uv_MainTex;
            float4 screenPos;
            float3 worldNormal;
            float3 worldPos;
        };

        //Functions

        //textureFunctions
        float3 UnpackDerivativeHeight(float4 textureData) {
            float3 dh = textureData.agb;
            dh.xy = dh.xy * 2 - 1;
            return dh;
        }

        float3 FlowUVW( float2 uv, float2 flowVector, float2 jump, float flowOffset, float tiling, float time, bool flowB ) {
            float phaseOffset = flowB ? 0.5 : 0;
            float progress = frac(time + phaseOffset);
            float3 uvw;
            uvw.xy = uv - flowVector * (progress + flowOffset);
            uvw.xy *= tiling;
            uvw.xy += phaseOffset;
            uvw.xy += (time - progress) * jump;
            uvw.z = 1 - abs(1 - 2 * progress);
            return uvw;
        }

        //water fog functions

        float2 AlignWithGrabTexel(float2 uv) {
            #if UNITY_UV_STARTS_AT_TOP
            if (_CameraDepthTexture_TexelSize.y < 0) {
                uv.y = 1 - uv.y;
            }
            #endif

            return (floor(uv * _CameraDepthTexture_TexelSize.zw) + 0.5) * abs(_CameraDepthTexture_TexelSize.xy);
        }

        float3 ColorBelowWater(float4 screenPos, float3 tangentSpaceNormal) {
            float2 uvOffset = tangentSpaceNormal.xy * _RefractionStrength;
            uvOffset.y *= _CameraDepthTexture_TexelSize.z * abs(_CameraDepthTexture_TexelSize.y);
            float2 uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w);

            float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
            float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
            float depthDifference = backgroundDepth - surfaceDepth;

            uvOffset *= saturate(depthDifference);
            uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w);
            backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
            depthDifference = backgroundDepth - surfaceDepth;

            float3 backgroundColor = tex2D(_WaterBackground, uv).rgb;
            float fogFactor = exp2(-_WaterFogDensity * depthDifference);
            return lerp(_Color, backgroundColor, fogFactor);
        }

        void ResetAlpha(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
            color.a = 1;
        }

        //sinewave functions
        float3 getWavePos(float3 pos)
        {
            pos.y = 0.0;

            float waveType = pos.z;

            pos.y += sin((_WaterTime * _WaterSpeed + waveType) / _WaterDistance) * _WaterScale;

            //Add noise
            //pos.y += tex2Dlod(_NoiseTex, float4(pos.x, pos.z + sin(_WaterTime * 0.1), 0.0, 0.0) * _WaterNoiseWalk).a * _WaterNoiseStrength;

            return pos;
        }

        //gerstnerwave functions
        float3 GerstnerWave(float4 wave, float3 p, inout float3 tangent, inout float3 binormal) {
            float steepness = wave.z;
            float wavelength = wave.w;
            float k = 2 * UNITY_PI / wavelength;
            float c = sqrt(9.8 / k);
            float2 d = normalize(wave.xy);
            float f = k * (dot(d, p.xz) - c * _WaterTime);
            float a = steepness / k;

            //original formula
            //p.x += d.x * (a * cos(f));
            //p.y = a * sin(f);
            //p.z += d.y * (a * cos(f));

            tangent += float3(
                -d.x * d.x * (steepness * sin(f)),
                d.x * (steepness * cos(f)),
                -d.x * d.y * (steepness * sin(f))
                );
            binormal += float3(
                -d.x * d.y * (steepness * sin(f)),
                d.y * (steepness * cos(f)),
                -d.y * d.y * (steepness * sin(f))
                );
            return float3(
                d.x * (a * cos(f)),
                a * sin(f),
                d.y * (a * cos(f))
                );
        }


        //vertex
        void vert(inout appdata_full IN)
        {
            //Get the global position of the vertice
            float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);

            //sine
            //float3 withWave = getWavePos(worldPos.xyz);


            //gerst
            float3 gridPoint = worldPos.xyz;
            float3 tangent = float3(1, 0, 0);
            float3 binormal = float3(0, 0, 1);
            float3 p = gridPoint;
            p += GerstnerWave(_WaveA, gridPoint, tangent, binormal);
            p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
            p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);
            float3 normal = normalize(cross(binormal, tangent));
            
            //Convert the position back to local
            float4 localPos = mul(unity_WorldToObject, float4(p.x, p.y, p.z, worldPos.w));
            //float4 localPos = mul(unity_WorldToObject, float4(withWave, worldPos.w));

            //Assign the modified vertice
            //gerst
            IN.vertex.xyz = localPos;
            IN.normal = normal;
            //sine
            //IN.vertex = localPos;
            
        }


        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        //fragments
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 uv = (IN.worldPos.xyz / 50);
            //texture stuff
            float3 flow = tex2D(_FlowMap, uv.xz).rgb; //add WorldPos


            //flow.xy = flow.xy * 2 - 1;
            flow.xy = flow.xy * 2 - 1;
            flow *= _FlowStrength;

            float noise = tex2D(_FlowMap, flow.xy).a; //add WorldPos
            float time = _WaterTime * _Speed + noise;
            float2 jump = float2(_UJump, _VJump);

            float3 uvwA = FlowUVW(uv.xz, flow.xy,  jump, _FlowOffset, _Tiling, time, false);//add WorldPos
            float3 uvwB = FlowUVW(uv.xz, flow.xy, jump, _FlowOffset, _Tiling, time, true);//add WorldPos

            

            float finalHeightScale = flow.z * _HeightScaleModulated + _HeightScale;

            float3 dhA = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwA.xy)) * (uvwA.z * finalHeightScale);
            float3 dhB = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwB.xy)) * (uvwB.z * finalHeightScale);
            o.Normal = normalize(float3(-(dhA.xy + dhB.xy), 1));
            
            fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
            fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;

            fixed4 c = (texA + texB) * _Color;

            //fixed4 c = texA * _Color;

            //mat properties
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Emission = ColorBelowWater(IN.screenPos, o.Normal) * (1 - c.a);
        }
        ENDCG
    }
    //FallBack "Diffuse"
}
