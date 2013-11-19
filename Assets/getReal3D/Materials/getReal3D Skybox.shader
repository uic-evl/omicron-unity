Shader "getReal3D/Skybox" {
    Properties {
    	_Tint ("Tint", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" }
		Cull Back ZWrite Off Fog { Mode Off }
CGINCLUDE
	#include "UnityCG.cginc"

	fixed4 _Tint;
	
	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = v.texcoord;
		return o;
	}
	fixed4 skybox_frag(v2f i, sampler2D smp)
	{
		fixed4 tex = tex2D(smp, i.texcoord);
		fixed4 col;
		col.rgb = tex.rgb + _Tint.rgb - unity_ColorSpaceGrey;
		col.a = tex.a * _Tint.a;
		return col;
	}
ENDCG

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		sampler2D _MainTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_MainTex); }

		ENDCG 
	}

    } 

    FallBack "Unlit/Texture"
}
