// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/WorldRuntime"
{
	Properties
	{
		_MainTex("texture", 2D) = "white" {}
		_BackTex ("Background texture", 2D) = "white" {}
		_ForeTex("Foreground texture", 2D) = "white" {}
		col1("color 1", color) = (0.0,0.5,1.0,1.0)
		col2("color 2", color) = (1.0,0.5,0.0,1.0)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _BackTex, _MainTex, _ForeTex;
			float2 pos, scrSize, size2;
			fixed4 col1, col2;
			float boss;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = (boss*col2 + (1.0 - boss)*col1);
				fixed4 fore = tex2D(_ForeTex, frac((i.uv+pos/size2*0.5)*scrSize / float2(132.0,154.0))).a*c;// +pos / size2 / float2(74,88)*50000.0));
				return tex2D(_BackTex, frac((i.uv-0.5) / float2(315.0, 362.0) * scrSize + pos / float2(315.0, 362.0) * 10.0+0.5)).b*c*0.2 + 0.5*fore.a*fore;
			}
			ENDCG
		}
	}
}