Shader "Unlit/MinesweeperGrid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _GridSize ("Grid Size", Vector) = (1, 1, 0, 0)

        _MousePosition ("Mouse Position", Vector) = (0, 0, 0, 0)

        _FontScale ("Font Scale", Range(1, 3)) = 2

        [NoScaleOffset]
        _Texture0 ("Texture 0", 2D) = "white"

        [NoScaleOffset]
        _Texture1 ("Texture 1", 2D) = "white"

        [NoScaleOffset]
        _TextureBomb ("Texture Bomb", 2D) = "white"

        [NoScaleOffset]
        _TextureFlag ("Texture Flag", 2D) = "white"
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha
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

            fixed4 _GridSize;
            fixed4 _MousePosition;
            float _FontScale;

            sampler2D _FontTexture;
            sampler2D _Texture0;
            sampler2D _Texture1;
            sampler2D _TextureBomb;
            sampler2D _TextureFlag;

            float4 _Colors[9];

            // bottomLeft
            // bottomRight
            // topRight
            // topLeft
            float4 _FontUvs[4 * 10];

            float2 scaleUv(float2 uv, float2 scale, float2 pivot)
            {
                uv -= pivot;
                uv *= scale;
                uv += pivot;
                return uv;
            }

            void unpackFontUv(int index, out float2 bottomLeft, out float2 bottomRight, out float2 topRight, out float2 topLeft)
            {
                int j = index * 4;
                bottomLeft = _FontUvs[j + 0];
                bottomRight = _FontUvs[j + 1];
                topRight = _FontUvs[j + 2];
                topLeft = _FontUvs[j + 3];
            }

            fixed4 SampleFontTexture(int index, float2 uv)
            {
                float2 bottomLeft;
                float2 bottomRight;
                float2 topRight;
                float2 topLeft;

                unpackFontUv(index, bottomLeft, bottomRight, topRight, topLeft);

                float width = abs(bottomRight.x - bottomLeft.x);
                float height = abs(topLeft.y - bottomLeft.y);
                float aspect = width / height;

                float2 a = lerp(bottomLeft.xy, bottomRight.xy, uv.x);
                float2 b = lerp(topLeft.xy, topRight.xy, uv.x);
                float2 myUv = lerp(a, b, uv.y);
                
                float2 scaledUv = myUv;
                float2 pivot = lerp(bottomLeft, topRight, 0.5);
                float baseScale = _FontScale;
                float2 scale = float2(baseScale / aspect, baseScale);
                scaledUv = scaleUv(scaledUv, scale, pivot);
                
                if (scaledUv.x < bottomLeft.x || scaledUv.x > bottomRight.x || scaledUv.y < topLeft.y || scaledUv.y > bottomLeft.y)
                {
                    return 0;
                }

                return tex2D(_FontTexture, scaledUv);
            }

            fixed4 SampleIcon(sampler2D tex, float2 uv)
            {
                float scale = 1.5;
                uv = scaleUv(uv, scale, 0.5);
                return tex2D(tex, uv);
            }

            int4 SampleAsInt4(float2 uv)
            {
                // https://forum.unity.com/threads/cant-pass-an-integer-to-a-shader.950419/#post-6196543
                return (int4)(tex2D(_MainTex, uv) * 255.0 + 0.5);
            }

            bool IsRevealed(float2 uv)
            {
                return SampleAsInt4(uv).x == 0;
            }

            bool IsBomb(float2 uv)
            {
                return SampleAsInt4(uv).g == 200;
            }

            bool IsFlag(float2 uv)
            {
                return SampleAsInt4(uv).g == 100;
            }

            int GetBombNeighborCount(float2 uv)
            {
                return SampleAsInt4(uv).b;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv *= _GridSize.xy;
                float2 uvFrac = frac(uv);
                float2 uvFloor = floor(uv);

                fixed4 colorCellClosed = tex2D(_Texture0, uvFrac);
                fixed4 colorCellOpened = tex2D(_Texture1, uvFrac);

                float chessboard = (uvFloor.x + uvFloor.y) % 2;

                float2 gridPos = _MousePosition * _GridSize;
                gridPos = floor(gridPos);

                int cellIndex = (int)(uvFloor.y * _GridSize.x + uvFloor.x);
                int normalizedIndex = cellIndex % 9u;
                int bombCount = GetBombNeighborCount(i.uv);
                fixed4 numberColor = _Colors[bombCount];

                float2 gridMin = step(gridPos, uvFloor);
                float2 gridMax = step(uvFloor, gridPos);
                float value = gridMin.x * gridMin.y * gridMax.x * gridMax.y;

                bool isRevealed = IsRevealed(i.uv);
                bool isBomb = IsBomb(i.uv);
                bool isFlag = IsFlag(i.uv);

                fixed4 cellColor;

                if (isRevealed)
                {
                    if (isBomb)
                    {
                        fixed4 bombColor = SampleIcon(_TextureBomb, uvFrac);
                        cellColor = lerp(colorCellOpened, bombColor, bombColor.a);
                    }
                    else if (bombCount > 0)
                    {
                        fixed4 fontColor = SampleFontTexture(bombCount, uvFrac);
                        cellColor = lerp(colorCellOpened, numberColor, fontColor.a);
                    }
                }
                else
                {
                    float tint = 0.85;
                    fixed4 defaultColor = colorCellClosed;
                    defaultColor.rgb *= tint;

                    fixed4 hoverColor = colorCellClosed;

                    cellColor = lerp(defaultColor, hoverColor, value);

                    if (isFlag)
                    {
                        fixed4 flagColor = SampleIcon(_TextureFlag, uvFrac);
                        cellColor = lerp(cellColor, flagColor, flagColor.a);
                    }
                }

                return cellColor;
            }
            ENDCG
        }
    }
}
