// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


// src: http://xroft666.blogspot.jp/2015/07/glow-highlighting-in-unity.html
// archived: https://web.archive.org/web/20161206224834/http://xroft666.blogspot.jp/2015/07/glow-highlighting-in-unity.html#!https://xroft666.blogspot.com/2015/07/glow-highlighting-in-unity.html

Shader "Custom/Highlight" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "black" {}
		_OccludeMap ("Occlusion Map", 2D) = "black" {}
	}
	
	SubShader {

		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
			
		// 0) OVERLAY GLOW		
		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
				sampler2D _OccludeMap;
				
				fixed4 frag(v2f_img IN) : COLOR 
				{
					fixed4 mCol = tex2D (_MainTex, IN.uv);

					/* 
					// screws up VR!
					// invert for OPENGL
					#if UNITY_UV_STARTS_AT_TOP
        				IN.uv.y = 1.0 - IN.uv.y;
					#endif
					*/

					fixed4 overCol = tex2D(_OccludeMap, IN.uv);
					return mCol + overCol;
				}
			ENDCG
		}
		
		// 1) OVERLAY SOLID
		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
				sampler2D _OccludeMap;
				
				// fixed4 _Color;
			
				fixed4 frag(v2f_img IN) : COLOR 
				{
					fixed4 mCol = tex2D (_MainTex, IN.uv);

					// invert for OPENGL
					#if UNITY_UV_STARTS_AT_TOP
        				IN.uv.y = 1.0 - IN.uv.y;
					#endif

					fixed4 oCol = tex2D (_OccludeMap, IN.uv);

					fixed4 solid = step (1.0 - oCol.a, oCol);
					return mCol + solid * fixed4(oCol.rgb, 1.0);
				}
			ENDCG
		}
	
		
		// 2) OCCLUSION	
		Pass {
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
				sampler2D _OccludeMap;

				fixed4 frag(v2f_img IN) : COLOR 
				{
					// difference between blurred and non-blurred happens here
					return tex2D(_MainTex, IN.uv) - tex2D(_OccludeMap, IN.uv);
				}
			ENDCG
		}

		// 3) Draw to render texture
		Pass {
        
           	Tags {"RenderType"="Opaque"}
        	ZWrite On
        	ZTest LEqual
        	Fog { Mode Off }
        
			CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
			
			fixed4 _Color;

            float4 vert(float4 v:POSITION) : POSITION {
                return UnityObjectToClipPos (v);
            }

           	fixed4 frag() : COLOR {
				return _Color;
            }

            ENDCG
        }	

        // 4) Depth sort and draw to render texture
        Pass {        	
           	Tags {"Queue"="Transparent"}
            Cull Back
            Lighting Off
            ZWrite Off
            ZTest LEqual
            ColorMask RGBA
            Blend OneMinusDstColor One

        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _CameraDepthTexture;
			fixed4 _Color;

            struct v2f {
                float4 vertex : POSITION;
                float4 projPos : TEXCOORD1;
            };
     
            v2f vert( float4 v : POSITION ) {        
                v2f o;
                o.vertex = UnityObjectToClipPos( v );
                o.projPos = ComputeScreenPos(o.vertex);             
                return o;
            }

            fixed4 frag( v2f i ) : COLOR {          
                float depthVal = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
                float zPos = i.projPos.z;
                
				return _Color * step( zPos, depthVal );
            }

            ENDCG
        }
	} 
}
