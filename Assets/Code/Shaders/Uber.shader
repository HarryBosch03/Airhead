Shader "Unlit/Uber"
{
	Properties
	{
		_Color("Color", Color) = (0.8, 0.8, 0.8, 1)
		_MainTex("Base Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

			struct Attributes
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct Varyings
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normalWS : NORMAL_WS;
				float3 positionOS : POSITION_OS;
				float3 scale : SCALE;
			};

			Varyings vert (Attributes input)
			{
				Varyings output;
				output.vertex = TransformObjectToHClip(input.vertex.xyz);
				output.uv = input.uv;
				output.normalWS = TransformObjectToWorldNormal(input.normal);
				output.positionOS = input.vertex;
				output.scale = float3
				(
					length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
					length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)),
					length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))
				);
				return output;
			}

			float4 _Color;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			float4 _MainTex_ST;
			
			half4 frag (Varyings input) : SV_Target
			{
				input.normalWS = normalize(input.normalWS);

				float3 weights = abs(input.normalWS);
				float3 uv = input.positionOS * input.scale;
				float4 tex =
					SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv.zy * _MainTex_ST.xy + _MainTex_ST.zw) * weights.x + 
					SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv.xz * _MainTex_ST.xy + _MainTex_ST.zw) * weights.y + 
					SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv.xy * _MainTex_ST.xy + _MainTex_ST.zw) * weights.z;
				
				float4 albedo = _Color * tex;

				float3 color = albedo.rgb;
				float alpha = albedo.a;

				float ndl = dot(input.normalWS, _MainLightPosition);
				color *= lerp(0.5, 1.0, saturate(ndl));
				
				return float4(color, alpha);
			}
			ENDHLSL
		}
	}
}