Shader "nz.Rishaan/CapsuleCull" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Radius("Radius", Float) = 0.5
		_P1("P1", Vector) = (0,0,0,0)
		_P2("P2", Vector) = (0,0,0,0)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
		Tags{"RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
	#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
	#pragma target 3.0

			sampler2D _MainTex;

		struct Input {
			float3 worldPos;
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		float3 _P1;
		float3 _P2;

		float _Radius;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float squaredDistance(float3 a)
		{
			return a.x * a.x + a.y * a.y + a.z * a.z;
		}

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o) {
			if (dot(IN.worldPos - _P1, _P2 - _P1) >= 0 && dot(IN.worldPos - _P2, _P1 - _P2) >= 0 && squaredDistance(cross(IN.worldPos - _P1, _P2 - _P1)) <= _Radius * squaredDistance(_P2 - _P1)) {
				
				discard;
				// Albedo comes from a texture tinted by color
				//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				//o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				//o.Metallic = _Metallic;
				//o.Smoothness = _Glossiness;
				//o.Alpha = _Transparency;
			}
			else {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Albedo *= IN.color.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
		}
		ENDCG
		}
			FallBack "Diffuse"
}