Shader "Special FX/Null Projection"
{
    Properties
    {
        _Colour ("Colour", Color) = (0.25, 0.375, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
		_BorderColour ("Border Colour", Color) = (0.125, 0.1875, 1, 1)
		_BorderSize ("Border Size", Range(1.0, 7.5)) = 3.75
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        CGPROGRAM
        //#pragma surface surf Lambert
        #pragma surface surf Lambert alpha
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpTex;
			
			float3 viewDir;
        };

		sampler2D _MainTex;
		sampler2D _BumpMap;
		
		fixed4 _Colour;
		
		fixed4 _BorderColour;
		fixed _BorderSize;

        half _Glossiness;
        half _Metallic;
        
        void surf (Input IN, inout SurfaceOutput o)
        {
			fixed4 colour = tex2D(_MainTex, IN.uv_MainTex) * _Colour;
			
			o.Albedo = colour.rgba;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpTex));
			
			half border = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _BorderColour.rgb * pow(border, _BorderSize);
			
            o.Alpha = colour.a;
        }
		
        ENDCG
    }
	
    FallBack "Diffuse"
}
