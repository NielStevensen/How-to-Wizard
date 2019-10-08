Shader "Special FX/Post Outline"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "black"{}
		_SceneTex("Scene Texture",2D) = "black"{}
	}
	
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvs : TEXCOORD0;
			};

			sampler2D _MainTex;
			float2 _MainTex_TexelSize;

			v2f vert(appdata_base IN)
			{
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uvs = OUT.pos.xy / 2 + 0.5;

				return OUT;
			}

			half frag(v2f IN) : COLOR
			{
				int NumberOfIterations = 20;
				float TX_x = _MainTex_TexelSize.x;
				float ColorIntensityInRadius;

				for (int i = 0; i < NumberOfIterations; i++)
				{
					ColorIntensityInRadius += tex2D(_MainTex, IN.uvs.xy + float2((i - NumberOfIterations / 2) * TX_x, 0)).r / NumberOfIterations;
				}

				return ColorIntensityInRadius;
			}

			ENDCG
		}  

		GrabPass{}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvs : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _SceneTex;

			sampler2D _GrabTexture;
			float2 _GrabTexture_TexelSize;

			v2f vert(appdata_base IN)
			{
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uvs = OUT.pos.xy / 2 + 0.5;

				return OUT;
			}

			half4 frag(v2f IN) : COLOR
			{
				int NumberOfIterations = 20;
				float TX_y = _GrabTexture_TexelSize.y;
				half ColorIntensityInRadius = 0;

				if (tex2D(_MainTex, IN.uvs).r > 0)
				{
					return tex2D(_SceneTex, float2(IN.uvs.x, IN.uvs.y));
				}

				for (int i = 0; i < NumberOfIterations; i++)
				{
					ColorIntensityInRadius += tex2D(_GrabTexture, float2(IN.uvs.x, 1 - IN.uvs.y) + float2(0, (i - NumberOfIterations / 2) * TX_y)).r / NumberOfIterations;
				}

				//half4 scene = tex2D(_SceneTex, float2(IN.uvs.x, IN.uvs.y));
				//scene = ColorIntensityInRadius * scene;
				//scene = (1 – ColorIntensityInRadius) * scene;
				//return ColorIntensityInRadius * half4(1, 0, 0, 1) + scene;

				half4 outcolor = ColorIntensityInRadius * half4(0, 1, 1, 1) * 2 + (1 - ColorIntensityInRadius) * tex2D(_SceneTex,  float2(IN.uvs.x, IN.uvs.y));
				return outcolor;
			}

			ENDCG
		}
	}
}