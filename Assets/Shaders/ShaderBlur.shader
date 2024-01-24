Shader "Unlit/ShaderBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Header(Blur)]
        _Kernel("Kernel", int) = 3
        _GaussianSpread("Gaussian Spread", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            int _Kernel;
            float _GaussianSpread;

            float3 boxBlur(sampler2D tex, float2 texelSize, float2 uv, uint kernel)
            {
                int upper = (kernel - 1) / 2;
                int lower = -upper;

                float3 result = 0;

                for (int x = lower; x <= upper; x++)
                {
                    for (int y = lower; y <= upper; y++)
                    {
                        float2 offset = float2(texelSize.x * float(x), texelSize.y * float(y));
                        result += tex2D(tex, uv + offset).xyz;
                    }
                }

                result /= float(kernel * kernel);

                return result;
            }

            float gaussianFormula(int x, int y, float spread)
            {
                static const float TWO_PI = 6.28319;
                static const float E = 2.71828;
                float sigmaSqu = spread * spread;
                float result = (1.0 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(float(x * x) + float(y * y)) / (2.0 * sigmaSqu));
                return result;
            }

            float3 gaussianBlur(sampler2D tex, float2 texelSize, float2 uv, uint kernel, float spread)
            {
                int upper = (kernel - 1) / 2;
                int lower = -upper;

                float kernelSum = 0.0;

                float3 result = 0;

                for (int x = lower; x <= upper; x++)
                {
                    for (int y = lower; y <= upper; y++)
                    {
                        float gaussian = gaussianFormula(x, y, spread);
                        kernelSum += gaussian;
                        float2 offset = float2(texelSize.x * float(x), texelSize.y * float(y));
                        result += gaussian * tex2D(tex, uv + offset).xyz;
                    }
                }

                result /= kernelSum;

                return result;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //col.rgb = boxBlur(_MainTex, _MainTex_TexelSize.xy, i.uv, _Kernel);
                col.rgb = gaussianBlur(_MainTex, _MainTex_TexelSize.xy, i.uv, _Kernel, _GaussianSpread);
                return col;
            }
            ENDCG
        }
    }
}
