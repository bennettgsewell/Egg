//Modified by Bennett Sewell
//2021-09-23
//Took the sprite shader from Unity and modified it to force Gameboy colors.
Shader "Sprites/Gamboi"
{
	Properties
	{
		[PerRendererData]
		_MainTex("Sprite Texture", 2D) = "white" {}

		//[MaterialToggle]
		//PixelSnap("Pixel snap", Float) = 0

		[HideInInspector]
		_RendererColor("RendererColor", Color) = (1,1,1,1)

		[HideInInspector]
		_Flip("Flip", Vector) = (1,1,1,1)

		[PerRendererData]
		_AlphaTex("External Alpha", 2D) = "white" {}

		[PerRendererData]
		_EnableExternalAlpha("Enable External Alpha", Float) = 0

			//_GameboiColor1("Gameboy Color 1", Color) = (0,0,0,1)
			//_GameboiColor2("Gameboy Color 2", Color) = (0.33,0.33,0.33,1)
			//_GameboiColor3("Gameboy Color 3", Color) = (0.66,0.66,0.66,1)
			//_GameboiColor4("Gameboy Color 4", Color) = (1,1,1,1)
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex SpriteVert
				#pragma fragment SpriteFrag
				#pragma target 2.0
				#pragma multi_compile_instancing
			//#pragma multi_compile_local _ PIXELSNAP_ON
			//#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

			#ifndef UNITY_SPRITES_INCLUDED
			#define UNITY_SPRITES_INCLUDED

			#include "UnityCG.cginc"

			#ifdef UNITY_INSTANCING_ENABLED

			UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
			// SpriteRenderer.Color while Non-Batched/Instanced.
			UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
			// this could be smaller but that's how bit each entry is regardless of type
			UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
		UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

		#define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
		#define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

	#endif // instancing

	CBUFFER_START(UnityPerDrawSprite)
	#ifndef UNITY_INSTANCING_ENABLED
		fixed4 _RendererColor;
		fixed2 _Flip;
	#endif
		float _EnableExternalAlpha;
	CBUFFER_END

		// Material Color.
		fixed4 _GameboiColor1 = (0,0,0,0);
		fixed4 _GameboiColor2 = (0.33,0.33,0.33,0.4);
		fixed4 _GameboiColor3 = (0.66,0.66,0.66,0.8);
		fixed4 _GameboiColor4 = (1,1,1,1);

		struct appdata_t
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float4 vertex   : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
		{
			return float4(pos.xy * flip, pos.z, 1.0);
		}

		v2f SpriteVert(appdata_t IN)
		{
			v2f OUT;

			UNITY_SETUP_INSTANCE_ID(IN);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

			OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
			OUT.vertex = UnityObjectToClipPos(OUT.vertex);
			OUT.texcoord = IN.texcoord;

			OUT.color = IN.color * _RendererColor;

			//#ifdef PIXELSNAP_ON
			//OUT.vertex = UnityPixelSnap(OUT.vertex);
			//#endif

			return OUT;
		}

		sampler2D _MainTex;
		sampler2D _AlphaTex;

		float4 SampleSpriteTexture(float2 uv)
		{
			float4 color = tex2D(_MainTex, uv);

			//#if ETC1_EXTERNAL_ALPHA
			//    fixed4 alpha = tex2D(_AlphaTex, uv);
			//    color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
			//#endif

			//The brightness of the pixel itself.
			float brightness = (color.r + color.g + color.b) / 3;

			if (brightness < 0.2)
				color.rgb = _GameboiColor1.rgb;
			else if (brightness < 0.4)
				color.rgb = _GameboiColor2.rgb;
			else if (brightness < 0.8)
				color.rgb = _GameboiColor3.rgb;
			else
				color.rgb = _GameboiColor4.rgb;
			
			return color;
		}

		fixed4 SpriteFrag(v2f IN) : SV_Target
		{
			fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
			c.rgb *= c.a;
			return c;
		}

		#endif // UNITY_SPRITES_INCLUDED
ENDCG
}
		}
}