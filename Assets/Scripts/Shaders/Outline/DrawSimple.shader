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

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
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
			}

			ENDCG
		}
	}
}