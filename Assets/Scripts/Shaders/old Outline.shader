Shader "Special FX/Old Outline" {
    Properties {
        _Colour ("Colour", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
		
		_DrawOutline ("Draw outline bool", int) = 0

		_OutlineTex ("Outline Texture", 2D) = "white" {}
        _OutlineColour ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline Width", Range(1.0, 10.0)) = 1.1
    }
    SubShader {
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

			Blend SrcAlpha OneMinusSrcAlpha
		Pass{
			Name "OUTLINE"

			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
			};

			int _DrawOutline;

			sampler2D _OutlineTex;
			float4 _OutlineColour;
			float _OutlineWidth;

			v2f vert(appdata IN)
			{
				IN.vertex.xyz *= _OutlineWidth;
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_TARGET
			{
				if (_DrawOutline == 1)
				{
					float4 colour = tex2D(_OutlineTex, IN.uv);
					return colour * _OutlineColour;
				}
				else
				{
					return (0, 0, 0, 0);
				}
			}

			ENDCG
		}
		
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

				Blend SrcAlpha OneMinusSrcAlpha
        Pass{
			Name "OBJECT"

            CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
			{
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
			{
                float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
            };

			float4 _Colour;
			sampler2D _MainTex;

            v2f vert(appdata IN)
			{
                v2f OUT;
				
				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;
				
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_TARGET
			{
				float4 colour = tex2D(_MainTex, IN.uv);

				return colour * _Colour;
            }

            ENDCG

			/*CGPROGRAM
			//#pragma surface surf Lambert
			#pragma surface surf Lambert alpha
			#pragma target 3.0

			struct Input
			{
				float2 uv_MainTex;
			};

			sampler2D _MainTex;

			fixed4 _Colour;

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 colour = tex2D(_MainTex, IN.uv_MainTex) * _Colour;

				o.Albedo = colour.rgba;

				o.Alpha = colour.a;
			}

			ENDCG*/
        }
    }
	
    FallBack "Standard"
}