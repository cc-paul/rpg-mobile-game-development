Shader "Todonero/Effect"
{
    Properties
    {
		//_BaseColor ("Base Color", Color) = (1,1,1,1)
		[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		[HDR] _EmissionColor ("Emission Color", Color) = (0,0,0)
		_RotateSpeed ("Rotate Speed", float) = 1.0
		_UScrollSpeed("U_ScrollSpeed", float) = 1.0
		_VScrollSpeed("V_ScrollSpeed", float) = 1.0
		[NoScaleOffset]_DissolveTex ("DissolveTex (RGB)", 2D) = "white" {}
		_DissolveScrollSpeed("DissolveScrollSpeed", float) = 1.0
		_Threshold("Threshold", Range(0,1))= 0.0
		[MaterialToggle] _VertexAlpha ("VertexAlpha", int) = 0
		_VertexAlphaRange("VertexAlphaRange", float) = 1.0
	}

	SubShader
	{
		//Tags {"Queue"="Transparent" "RenderType"="Opaque"}
		//Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#include "UnityCG.cginc"

			#define PI 3.141592

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _EmissionColor;
			float4 _MainTex_ST;
			float _RotateSpeed;
			float _UScrollSpeed;
			float _VScrollSpeed;
			sampler2D _DissolveTex;
			float _DissolveScrollSpeed;
			half _Threshold;
			int _VertexAlpha;
			float _VertexAlphaRange;

			v2f vert (appdata v)
			{
				v2f o;

				// VertAlpha
				if(_VertexAlpha == 1 && v.vertex.y > _VertexAlphaRange)
				{
					v.color.a = 0;
				}

				o.vertex    = UnityObjectToClipPos(v.vertex);
				o.uv        = v.uv;
				o.color     = v.color;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// -- Rotation -- 
				// Timeを入力として現在の回転角度を作る
				half angle              = frac(_Time.x) * PI * 2;
				// 回転行列を作る
				half angleCos           = cos(angle * _RotateSpeed);
				half angleSin           = sin(angle * _RotateSpeed);
				half2x2 rotateMatrix    = half2x2(angleCos, -angleSin, angleSin, angleCos);
				// 中心を起点にUVを回転させる
				i.uv                    = mul(i.uv - 0.5, rotateMatrix) + 0.5;

				half4 sub;
				half2 uv = i.uv;
				half t = _Time.x - floor(_Time.x);

				// -- Scroll -- 
				fixed4 col = tex2D(_MainTex, uv);

				uv.y =  i.uv.y + t * _DissolveScrollSpeed;
				sub = tex2D(_DissolveTex, uv);
				col.rgb = lerp (col.rgb, col.rgb, sub.a);

				i.uv.x += _UScrollSpeed * _Time;
				i.uv.y += _VScrollSpeed * _Time;

				// --Dilsove--
				half g = sub.r * 0.2 + sub.g * 0.7 + sub.b * 0.1;
				if( g < _Threshold )
				{
					discard;
				}

				// -- color --
				col = i.color * tex2D(_MainTex, i.uv) * _EmissionColor;
				return col;
			}
			ENDCG
		}
	}
}
