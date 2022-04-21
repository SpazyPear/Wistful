Shader "Unlit/PingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "ZeroRGBA" {}
        _Power("Power", Float) = 0.5
        _LineThickness("Line Thickness", Float) = 2
        _Speed ("Speed", Float) = 30
        _Dimmer ("Dimmer", Float) = 2
        _IsAdditive ("Uses Texture", Integer) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _LineThickness;
            float _Power;
            float _Speed;
            float _Dimmer;
            int _IsAdditive;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float allowedDistance = ((tan(_Time.x * _Speed) + 1) / 2);
                float4 col = tex2D(_MainTex, i.uv);
                if (_IsAdditive == 0)
                    col.a = 0;
                col = col + (clamp((1 - pow(distance(i.uv.y, allowedDistance), _Power) * _LineThickness), 0, 1) * float4(1, 1, 1, 1) / 2);
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
