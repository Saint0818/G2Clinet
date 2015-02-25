// CANDELA-SSRR SCREEN SPACE RAYTRACED REFLECTIONS
// Copyright 2014 Livenda

Shader "Hidden/CustomDepthSSRR" {


SubShader {
//ZTest Off Cull Off ZWrite Off Fog { Mode Off }
    Tags {"Queue" = "Transparent"  "RenderType"="Transparent"}
    Pass {
   // Tags {"Queue" = "Transparent"  "RenderType"="Transparent"}
    // ColorMask 1
        Fog { Mode Off }
        //Blend  Off
        //BlendOp Max 
        
        //Blend SrcAlpha OneMinusSrcAlpha
         //   ZWrite Off
         //   Cull Off
         //   ZTest Equal  
         //   ZWrite Off
        //ZWrite Off
        //ZTest Always Cull Off ZWrite On Fog { Mode Off }
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

float _ExcludeFromSSRR;

struct v2f {
    float4 pos : SV_POSITION;
    float2 mypos : TEXCOORD1;
};

v2f vert (appdata_base v) {
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.mypos = o.pos.zw;
    return o;
}

half4 frag(v2f i) : COLOR {
	
	float castref =_ExcludeFromSSRR;//_CastSSRR;
	
	float4 d = float4(i.mypos.x/i.mypos.y, 1-castref, 0, 0);
	
    return  d;//i.mypos.x/i.mypos.y;
}
ENDCG
    }
}
}