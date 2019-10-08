Shader "Special FX/DrawSimple"
{
	SubShader
	{
		ZWrite Off
		ZTest Always
		Lighting Off

		Pass
		{
			CGPROGRAM

			#pragma vertex VShader
			#pragma fragment FShader

			#include "UnityCG.cginc"

			struct VertexToFragment
			{
				float4 pos:SV_POSITION;
			};

			VertexToFragment VShader(VertexToFragment i)
			{
				VertexToFragment o;
				o.pos = UnityObjectToClipPos(i.pos);
				return o;
			}

			//return white
			half4 FShader() :COLOR0
			{
				return half4(1,1,1,1);
			}

			/*struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata IN)
			{
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);

				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				return fixed4(1, 1, 1, 1);
			}*/

			ENDCG
		}
	}
}