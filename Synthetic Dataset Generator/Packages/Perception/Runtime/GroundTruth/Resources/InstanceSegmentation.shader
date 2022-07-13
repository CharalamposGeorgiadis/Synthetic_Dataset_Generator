Shader "Perception/InstanceSegmentation"
{
    Properties
    {
        [PerObjectData] _SegmentationId("Segmentation ID", vector) = (0,0,0,1)
        _MainTex ("Main Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite on
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            //Tags { "LightMode" = "SRP" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Packing.hlsl"

            float4 _SegmentationId;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a == 0)
                    return fixed4(0, 0, 0, 0);

                return _SegmentationId;
            }
            ENDCG
        }
    }
}
