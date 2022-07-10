Shader "Perception/SemanticSegmentation"
{
    Properties
    {
        [PerObjectData] LabelingId("Labeling Id", Vector) = (0,0,0,1)
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

            #pragma vertex semanticSegmentationVertexStage
            #pragma fragment semanticSegmentationFragmentStage

            #include "UnityCG.cginc"

            float4 LabelingId;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct in_vert
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vertexToFragment
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            vertexToFragment semanticSegmentationVertexStage (in_vert vertWorldSpace)
            {
                vertexToFragment vertScreenSpace;
                vertScreenSpace.vertex = UnityObjectToClipPos(vertWorldSpace.vertex);
                vertScreenSpace.uv = TRANSFORM_TEX(vertWorldSpace.uv, _MainTex);
                return vertScreenSpace;
            }

            fixed4 semanticSegmentationFragmentStage (vertexToFragment vertScreenSpace) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, vertScreenSpace.uv);
                if (col.a == 0)
                    return fixed4(0, 0, 0, 0);

                return LabelingId;
            }

            ENDCG
        }
    }
}
