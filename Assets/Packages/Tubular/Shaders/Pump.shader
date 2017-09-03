Shader "Tubular/Pump"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Radius ("Radius", Range(0.1, 1)) = 1
		_Offset ("Offset", Float) = 20.0
		_Speed ("Speed", Float) = 1.0

		_Expansion ("Expansion", Float) = 1.0
		_Torsion ("Torsion", Float) = 2.0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			half _Radius, _Offset, _Speed;
			half _Expansion, _Torsion;

			// Quaternion multiplication
			// http://mathworld.wolfram.com/Quaternion.html
			float4 qmul(float4 q1, float4 q2) {
				return float4(
					q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
					q1.w * q2.w - dot(q1.xyz, q2.xyz)
				);
			}

			// Vector rotation with a quaternion
			// http://mathworld.wolfram.com/Quaternion.html
			float3 rotate_vector(float3 v, float4 r) {
				float4 r_c = r * float4(-1, -1, -1, 1);
				return qmul(r, qmul(float4(v, 0), r_c)).xyz;
			}

			float3 rotate_vector_at(float3 v, float3 center, float4 r) {
				float3 dir = v - center;
				return center + rotate_vector(dir, r);
			}

			// A given angle of rotation about a given axis
			float4 rotate_angle_axis(float angle, float3 axis) {
				float sn = sin(angle * 0.5);
				float cs = cos(angle * 0.5);
				return float4(axis * sn, cs);
			}

			v2f vert (appdata v)
			{
				v2f o;

				float2 uv = v.uv;
				float3 center = v.vertex.xyz;

				float wave = (sin(_Time.y * _Speed + uv.y * _Offset) + 1.0) * 0.5 * _Expansion;
				v.vertex.xyz += v.normal * (_Radius + wave);

				float torsion = sin(_Time.y * _Speed + uv.y * _Offset) * _Torsion;
				float4 q = rotate_angle_axis(torsion, v.tangent.xyz);
				v.vertex.xyz = rotate_vector_at(v.vertex.xyz, center, q);

				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
