Shader "Renge/RadialSegmentedHealthBarBuiltIn" {
	Properties {
		_MainTex ("DONT_USE", 2D) = "white" {}

		_Color("Color", Color) = (1,0,0,1)

		_SegmentCount("SegmentCount", Float) = 5
		_RemoveSegments("RemoveSegments", Float) = 1

		_SegmentSpacing("Spacing", Float) = 0.04
		_Radius("Radius", Float) = 0.35
		_LineWidth("LineWidth", Float) = 0.1
		_Rotation("Rotation", Float) = 0
		
	}
	SubShader {
		Tags {
			"Queue"="Transparent" "RenderType"="Transparent"
		}
		LOD 100
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct VertexIn
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct VertexOut
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Color;
			
			float _SegmentCount;
			float _RemoveSegments;

			float _SegmentSpacing;
			float _Radius;
			float _LineWidth;
			float _Rotation;

			//rotate uvs using radians
			//source: https://forum.unity.com/threads/rotation-of-texture-uvs-directly-from-a-shader.150482/
			float2 rotateuv(float2 uv, float2 center, float rotation)
			{
				uv -= center;
				float s = sin(rotation);
				float c = cos(rotation);
				float2x2 rotMat = float2x2(c, -s, s, c);
				rotMat *= .5;
				rotMat += .5;
				rotMat = rotMat * 2 - 1;
				float2 res = mul(uv, rotMat);
				res += center;
				return res;
			}

			//source: https://stackoverflow.com/a/3451607/3987342
			float remap(float value, float2 i, float2 o)
			{
				return o.x + (value - i.x) * (o.y - o.x) / (i.y - i.x);
			}

			float mod(float a, float b)
			{
				return a % b;
			}

			//inverse lerp function
			//source: shader graph
			float ilerp(float a, float b, float t)
			{
				return (t - a) / (b - a);
			}

			float2 polarCoordinates(float2 uv, float2 center, float radialScale, float lengthScale)
			{
				float2 delta = uv - center;
				float radius = length(delta) * 2 * radialScale;
				float angle = atan2(delta.x, delta.y) * 1.0/6.28 * lengthScale;
				return float2(radius, angle);
			}

			VertexOut vert(VertexIn v)
			{
				VertexOut o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(VertexOut i) : SV_Target
			{
				float pi = 3.14159;
				float2 halfuv = float2(.5, .5);
				float2 transuv = i.uv - halfuv;
				float transuvlen = length(transuv);
				float divpisegc = pi / _SegmentCount;

				///////////////////////////////////////////////////////////////////////////////////////////LINES
				float2 rotateduv01 = rotateuv(transuv, float2(0, 0), radians(_Rotation));
				float calc000 = pi + atan2(rotateduv01.y, rotateduv01.x) + divpisegc;
				float lines = mul(transuvlen, abs(sin(mod(calc000, 2 * divpisegc) - divpisegc)));

				//////////////////////////////////////////////////INNER_SPACING
				float innerspacing = mul(1 - clamp((lines - _SegmentSpacing) / fwidth(lines - _SegmentSpacing), 0, 1),
										 mul(clamp(remap(_SegmentSpacing, float2(0, 0.001), float2(0, 1)), 0, 1),
											 round(clamp(remap(_SegmentCount, float2(1, 2), float2(0, 0.51)), 0, 1))));

				////////////////////////////////////////////////////////////////////INNER_CIRCLE
				float preCirc = (transuvlen - _Radius);
				float circle = 1 - clamp((abs(preCirc) - _LineWidth) / fwidth(preCirc), 0, 1);

				float segcirc = circle - innerspacing;

				float calc001 = remap(mul(_RemoveSegments, divpisegc), float2(0, pi),
									  float2(divpisegc, divpisegc + 2 * pi)) - calc000;
				float removedsegments = lerp(clamp(calc001 / fwidth(calc001), 0, 1),
											 smoothstep(0, 0.001, calc001 / fwidth(calc001)),
											 clamp(ilerp(.0001, .0002, _RemoveSegments), 0, 1));

				float remsegcirc = clamp(segcirc - removedsegments, 0, 1);

				////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////INNER_CIRCLE_FINAL
				float4 finalHealthBar = float4(_Color.rgb, 1) * remsegcirc * _Color.a;

				/////////FINAL
				return finalHealthBar;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}