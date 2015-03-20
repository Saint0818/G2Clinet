Shader "UI/SpellSlot Cutoff"
{
   	Properties
   	{
      	_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
      	_Mask ("Cutout (A)", 2D) = "white" {}
      	_Cutoff ("Alpha cutoff", Range (0,1)) = 0.1
   	}
   	
   	SubShader
   	{
   		LOD 100
      	
      	Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
      	
      	Cull Off
      	Lighting Off
      	ZWrite Off
      	Fog { Mode Off }
      	Blend SrcAlpha OneMinusSrcAlpha
      	AlphaTest GEqual [_Cutoff]
      	
      	Pass
      	{
      		CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				sampler2D _Mask;
				float4 _MainTex_ST;
				float _Cutoff;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 c = tex2D(_MainTex, i.texcoord) * i.color;
					fixed ca = tex2D(_Mask, i.texcoord).a;
					c.a *= ca > _Cutoff ? 1 : 0;
					return c;
				}
			ENDCG
      	}
   	}
   	
   	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}