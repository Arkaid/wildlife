Shader "jintori/Play Area Shader"
{
	Properties
	{
		_BaseImage ("Base Image", 2D) = "white" {}
		_Shadow ("Shadow", 2D) = "white" {}
		_Mask("Shadow Mask", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _BaseImage;
			float4 _BaseImage_ST;
			sampler2D _Shadow;
			float4 _Shadow_ST;
			sampler2D _Mask;
			float4 _Mask_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _BaseImage);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 base = tex2D(_BaseImage, i.uv);
				fixed4 shadow = tex2D(_Shadow, i.uv);
				fixed4 mask = tex2D(_Mask, i.uv);

				return base * (1 - mask.a) + shadow * mask.a;
			}
			ENDCG
		}
	}
}
