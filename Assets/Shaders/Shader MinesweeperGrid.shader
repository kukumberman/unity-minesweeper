Shader "Unlit/MinesweeperGrid"
{
    Properties
    {
        [HideInInspector]
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

            float4 _UVBottomLeft;
            float4 _UVBottomRight;
            float4 _UVTopRight;
            float4 _UVTopLeft;

            float2 scaleUv(float2 uv, float2 scale, float2 pivot)
            {
                uv -= pivot;
                uv *= scale;
                uv += pivot;
                return uv;
            }

            fixed4 SampleFontTexture(float2 uv)
            {
                float width = abs(_UVBottomRight.x - _UVBottomLeft.x);
                float height = abs(_UVTopLeft.y - _UVBottomLeft.y);
                float aspect = width / height;

                float2 a = lerp(_UVBottomLeft.xy, _UVBottomRight.xy, uv.x);
                float2 b = lerp(_UVTopLeft.xy, _UVTopRight.xy, uv.x);
                float2 myUv = lerp(a, b, uv.y);
                
                float2 scaledUv = myUv;
                float2 pivot = lerp(_UVBottomLeft, _UVTopRight, 0.5);
                float baseScale = _FontScale;
                float2 scale = float2(baseScale / aspect, baseScale);
                scaledUv = scaleUv(scaledUv, scale, pivot);
                
                fixed4 originalColor = tex2D(_FontTexture, scaledUv);
                fixed4 trimmedColor = originalColor;

                if (scaledUv.x < _UVBottomLeft.x || scaledUv.x > _UVBottomRight.x || scaledUv.y < _UVTopLeft.y || scaledUv.y > _UVBottomLeft.y)
                {
                    return 0;
                }

                return trimmedColor;
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
                fixed4 numberColor = _Colors[normalizedIndex];

                float2 gridMin = step(gridPos, uvFloor);
                float2 gridMax = step(uvFloor, gridPos);
                float value = gridMin.x * gridMin.y * gridMax.x * gridMax.y;

                bool isRevealed = true;
                bool isBomb = false;
                bool isFlag = false;

                fixed4 cellColor;

                if (isRevealed)
                {
                    float2 fontUv = uvFrac;
                    fixed4 fontColor = SampleFontTexture(fontUv);
                    float fontMask = fontColor.a;
                    cellColor = lerp(colorCellOpened, numberColor, fontMask);
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
                        float2 flagUv = uvFrac;
                        flagUv = scaleUv(flagUv, 2, 0.5);
                        fixed4 flagColor = tex2D(_TextureFlag, flagUv);
                        cellColor = lerp(cellColor, flagColor, flagColor.a);
                    }
                }

                return cellColor;
            }
            ENDCG
        }
    }
}
