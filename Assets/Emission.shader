Shader "Unlit/Emission" {
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D_float _CameraDepthTexture;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float linearDepth : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                o.linearDepth = o.vertex.w * _ProjectionParams.w;// -(UnityObjectToViewPos(v.vertex).z * _ProjectionParams.w);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = float4(0,0,0,1);
                float camDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
                camDepth = Linear01Depth(camDepth);

                if(i.linearDepth < camDepth + _ProjectionParams.w * 0.01) col = float4(1, 0, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
