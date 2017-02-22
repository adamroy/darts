Shader "Unlit/Glint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (0, 0, 0, 1)
		_AnimationT("Animation Progress", Range(-1, 1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _AnimationT;

			// Tranforms position from object to homogenous space
			inline float4 UnityObjectToClipPos(in float3 pos)
			{
				#if defined(UNITY_SINGLE_PASS_STEREO) || defined(UNITY_USE_CONCATENATED_MATRICES)
					// More efficient than computing M*VP matrix product
					return mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4(pos, 1.0)));
				#else
					return mul(UNITY_MATRIX_MVP, float4(pos, 1.0));
				#endif
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float4 pos = UnityObjectToClipPos(v.vertex.xyz);
				o.screenPos = ComputeScreenPos(pos);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 screenUV = i.screenPos / i.screenPos.w;
				// screenUV.y = 1 - screenUV.y;
				fixed4 col = _Color;
				col += tex2D(_MainTex, screenUV + float2(_AnimationT, 0));
				return col;
			}

			ENDCG
		}
	}
}
