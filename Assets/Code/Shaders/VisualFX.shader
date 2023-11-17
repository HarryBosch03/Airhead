Shader "Unlit/VisualFX"
{
	Properties
	{
		_Brightness("Brightness", float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			Varyings vert (Attributes input)
			{
				Varyings output;
				output.vertex = TransformObjectToHClip(input.vertex.xyz);
				output.color = input.color;
				return output;
			}

			float _Brightness;
			
			half4 frag (Varyings input) : SV_Target
			{
				return half4(input.color.rgb * _Brightness, input.color.a);
			}
			ENDHLSL
		}
	}
}