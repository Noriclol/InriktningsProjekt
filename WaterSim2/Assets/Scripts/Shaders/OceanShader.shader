
Shader "Custom/OceanShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
            _NoiseTex("Noise Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        sampler2D _NoiseTex;

        //Water parameters
        float _WaterScale;
        float _WaterSpeed;
        float _WaterDistance;
        float _WaterTime;
        float _WaterNoiseStrength;
        float _WaterNoiseWalk;

        struct Input
        {
            float2 uv_MainTex;
        };

        float3 getWavePos(float3 pos)
        {
            pos.y = 0.0;

            float waveType = pos.z;

            pos.y += sin((_WaterTime * _WaterSpeed + waveType) / _WaterDistance) * _WaterScale;

            //Add noise
            pos.y += tex2Dlod(_NoiseTex, float4(pos.x, pos.z + sin(_WaterTime * 0.1), 0.0, 0.0) * _WaterNoiseWalk).a * _WaterNoiseStrength;

            return pos;
        }

        void vert(inout appdata_full IN)
        {
            //Get the global position of the vertice
            float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);

            //Manipulate the position
            float3 withWave = getWavePos(worldPos.xyz);

            //Convert the position back to local
            float4 localPos = mul(unity_WorldToObject, float4(withWave, worldPos.w));

            //Assign the modified vertice
            IN.vertex = localPos;
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
