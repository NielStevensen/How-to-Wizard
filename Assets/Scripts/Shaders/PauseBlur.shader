Shader "Special FX/Pause Blur"{
	Properties
	{
		[HideInInspector]_MainTex("Texture", 2D) = "white" {}
		_BlurSize("Blur Size", Range(0,0.5)) = 0.25
	}

	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always

		//Vertical Blur
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float _BlurSize;

			struct appdata 
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f 
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v) 
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET
			{
				float4 col = 0;

				for (float y = -5; y < 6; y++)
				{
					float2 uv = i.uv + float2(0, y * _BlurSize);

					col += tex2D(_MainTex, uv);
				}

				return col / 9;
			}

			ENDCG
		}

		//Horizontal Blur
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float _BlurSize;

			struct appdata 
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET
			{
				float invAspect = _ScreenParams.y / _ScreenParams.x;
				float4 col = 0;

				for (float x = -5; x < 6; x++)
				{
					float2 uv = i.uv + float2(0, x * _BlurSize * invAspect);

					col += tex2D(_MainTex, uv);
				}

				return col / 9;
			}

			ENDCG
		}
	}
}