// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// This code is related to an answer I provided in the Unity forums at:
// http://forum.unity3d.com/threads/circular-fade-in-out-shader.344816/

Shader "jintori/ScreenTransitionImageEffect"
{
	Properties
	{
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_MaskValue ("Mask Value", Range(0,1)) = 0.5
		_MaskColor ("Mask Color", Color) = (0,0,0,1)
		[Toggle(INVERT_MASK)] _INVERT_MASK ("Mask Invert", Float) = 0
	}
	SubShader
	{
		Tags{ "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" "CanUseSpriteAtlas" = "true" "PreviewType" = "Plane" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest[unity_GUIZTestMode]
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag		
			#include "UnityCG.cginc"

			#pragma shader_feature INVERT_MASK


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv     : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv     : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

			#if UNITY_UV_STARTS_AT_TOP
				o.uv.y = 1 - o.uv.y;
			#endif

				return o;
			}
			
			sampler2D _MaskTex;
			float _MaskValue;
			float4 _MaskColor;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = _MaskColor;
				float4 mask = tex2D(_MaskTex, i.uv);

				// Scale 0..255 to 0..254 range.
				float alpha = mask.r * (1 - 1/255.0);

				// If the mask value is greater than the alpha value,
				// we want to draw the mask.
				float weight = step(_MaskValue, alpha);
			#if INVERT_MASK
				weight = 1 - weight;
			#endif

				return float4(col.rgb, weight);
			}
			ENDCG
		}
	}
}
