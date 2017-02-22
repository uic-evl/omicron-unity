Shader "Custom/CAVE2ScreenMask" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		[KeywordEnum(Less,Greater,LEqual,GEqual,Equal,NotEqual,Always)] _ZTest("Z Test", Float) = 2
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Lighting Off
		Cull Off
		ZTest [_ZTest]
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	Fallback "Legacy Shaders/VertexLit"
}
