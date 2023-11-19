Shader "Unlit/Shell"
{
    Properties
    {
        _ColorA("Color A", Color) = (1, 1, 1, 1)
        _ColorB("Color B", Color) = (0.8, 0.8, 0.8, 1.0)
        _MainTex ("Texture", 2D) = "white" {}
        _TexScale("Texture Scale", float) = 1.0
        _Noise ("Noise Map", 2D) = "white" {}
        _NoiseScale ("Noise Map Scale", float) = 1.0
        _Extrude("Extrude", float) = 0.1
        _Thickness("Thickness", Range(0.0, 1.0)) = 1.0
        _Shading("Shading", Range(0.0, 1.0)) = 0.5
        
        _Wind ("Wind Map", 2D) = "white" {}
        _WindStrength("Wind Strength", Vector) = (1, 0.5, 0.0, 0.0)
        _WindFrequency("Wind Frequency", float) = 1.0
        _WindVariation("Wind Variation", float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #define UNITY_INSTANCING_ENABLED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 positionOS : VAR_POSITION_OS;
                float3 positionWS : VAR_POSITION_WS;
                float3 scale : VAR_SCALE;
                float3 normal : VAR_NORMAL;
                float3 tangent : VAR_TANGENT;
                float percent : VAR_PERCENT;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            int _ShellCount = 1;
            float _Extrude;

            float2 _WindStrength;
            float _WindFrequency;
            float _WindVariation;
            
            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);
            float _NoiseScale;
            
            TEXTURE2D(_Wind);
            SAMPLER(sampler_Wind);

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(output);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                output.positionWS = TransformObjectToWorld(input.vertex.xyz);
                output.percent = (float)UNITY_GET_INSTANCE_ID(input) / _ShellCount;

                output.normal = TransformObjectToWorldNormal(input.normal);
                output.positionWS += output.normal * _Extrude * output.percent;
                output.positionOS = TransformWorldToObject(output.positionWS);

                half3x3 m = (half3x3)UNITY_MATRIX_M;
                output.scale = half3(
                    length(half3(m[0][0], m[1][0], m[2][0])),
                    length(half3(m[0][1], m[1][1], m[2][1])),
                    length(half3(m[0][2], m[1][2], m[2][2]))
                );
                
                output.vertex = TransformWorldToHClip(output.positionWS);
                output.tangent = TransformObjectToWorldNormal(input.tangent);
                
                return output;
            }

            float4 _ColorA, _ColorB;

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _TexScale;
            float _Thickness;
            float _Shading;
            
            half4 SampleTriplanar(TEXTURE2D_PARAM(_tex, sampler_tex), Varyings input, float scale)
            {
                float3 weights = abs(input.normal);
                float3 pos = input.positionOS * input.scale / scale;
                
                return
                    SAMPLE_TEXTURE2D(_tex, sampler_tex, pos.zy) * weights.x +
                    SAMPLE_TEXTURE2D(_tex, sampler_tex, pos.xz) * weights.y +
                    SAMPLE_TEXTURE2D(_tex, sampler_tex, pos.xy) * weights.z;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float2 wind = SAMPLE_TEXTURE2D_LOD(_Wind, sampler_Wind, float2(_Time[0] * _WindFrequency, 0.0), 0) * _WindStrength;
                input.positionOS.xz += wind / input.scale.xz * input.percent;

                float noise = SampleTriplanar(_Noise, sampler_Noise, input, _TexScale * _NoiseScale).r;
                clip(noise - input.percent + _Thickness - 1.0);
                
                
                input.normal = normalize(input.normal);
                input.tangent = normalize(input.tangent);

                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.positionCS = input.vertex;
                inputData.normalWS = input.normal;

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = lerp(_ColorA, _ColorB, SampleTriplanar(_MainTex, sampler_MainTex, input, _TexScale)).rgb;

                half4 color = UniversalFragmentBlinnPhong(inputData, surfaceData);
                color.rgb *= lerp(_Shading, 1.0, 1.0 - pow(1.0 - input.percent, 2.0));
                
                return color;
            }
            ENDHLSL
        }
    }
}