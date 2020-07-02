Shader "Unlit/Pounding"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _TintColor ("Tint Color", Color) = (1, 0, 0, 1)
        _Transparency ("Transparency", Range(0.0, 1.0)) = 0.5
        _Distance ("Distance", Float) = 1
        _Speed ("Speed", Float) = 1
        _Amplitude ("Amplitude", Range(0.0, 1.0)) = 0.25
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _TintColor;
            float _Transparency;
            float _Distance;
            float _Speed;
            float _Amplitude;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex = v.vertex * _Distance * _Amplitude * abs(sin(_Time.x * _Speed));
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // multiply color
                //fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;
                fixed4 col = _TintColor;
                // col.a = _Transparency;

                return col;
            }
            ENDCG
        }
    }
}
