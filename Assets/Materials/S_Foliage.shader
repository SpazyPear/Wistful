Shader "Custom/S_Foliage" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Speed("Speed", Float) = 50
		_YScaling("Y Scaling Factor", Float) = 1
		_Strength("Strength", Float) = 2
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
		//Notice the "vertex:vert" at the end of the next line
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma glsl
		#pragma target 3.0

		sampler2D _MainTex;
		float _Speed;
		float _YScaling;
		float _Strength;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;


		void vert(inout appdata_full v, out Input o) {



			if (tex2Dlod(_MainTex, float4(v.texcoord.xy, 0,0)).g > 0.4) {
				v.vertex.x += sin(_Time.x * _Speed + v.vertex.y * _Strength) * .1 * (v.vertex.y * _YScaling);
			}
			UNITY_INITIALIZE_OUTPUT(Input, o);


		}

		void surf(Input IN, inout SurfaceOutputStandard o) {

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;

			o.Alpha = c.a;
		}
		ENDCG
	}

		FallBack "Diffuse"
}
