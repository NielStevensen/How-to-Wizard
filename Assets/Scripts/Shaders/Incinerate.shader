Shader "Special FX/Incinerate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Colour("Colour", Color) = (1, 1, 1, 1)
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		[HDR] _Emission ("Emission", Color) = (0, 0, 0)

		[Header(Disintegrate)]
		_DisintegrateTex("Disintegration Texture", 2D) = "black" {}
		_DisintegrateAmount("Disintegration Amount", Range(0, 1)) = 0.5

		[Header(Glow)]
		[HDR] _GlowColour ("Glow Colour", Color) = (1, 1, 1, 1)
		_GlowRange ("Range", Range(0, 0.3)) = 0.1
		_GlowFallOff("Fall Off", Range(0.001, 0.3)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

		//Base variables
        sampler2D _MainTex;
		fixed4 _Colour;
		half _Smoothness;
		half _Metallic;
		half3 _Emission;

		//Disintegration variables
		sampler2D _DisintegrateTex;
		float _DisintegrateAmount;

		//Glow variables
		float3 _GlowColour;
		float _GlowRange;
		float _GlowFallOff;

		//Input structure
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DisintegrateTex;
        };

		//Render the pixel based on disintegration and glow
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			//Determine whether or not the pixel is rendered
			float disintegrate = tex2D(_DisintegrateTex, IN.uv_DisintegrateTex).r;
			disintegrate *= 0.999;
			float isVisible = disintegrate - _DisintegrateAmount;
			clip(isVisible);
			
			//Determine the degree of the glow
			float isGlowing = smoothstep(_GlowRange + _GlowFallOff, _GlowRange, isVisible);
			float3 glow = isGlowing *_GlowColour * min(_DisintegrateAmount * 2.5, 1);

			//Basic render stuff
			fixed4 colour = tex2D(_MainTex, IN.uv_MainTex);
			colour *= _Colour;

			o.Albedo = colour;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
			o.Emission = glow;
        }

        ENDCG
    }

    FallBack "Standard"
}
