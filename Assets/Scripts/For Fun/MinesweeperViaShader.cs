using UnityEngine;

public sealed class MinesweeperViaShader : MonoBehaviour
{
    private static readonly string[] HtmlColors = new string[]
    {
        "",
        "#0000FF", // 1
        "#008000", // 2
        "#FF0000", // 3
        "#000080", // 4
        "#800000", // 5
        "#008080", // 6
        "#000000", // 7
        "#808080", // 8
    };

    [SerializeField]
    private Font _font;

    [SerializeField]
    private Material _material;

    [SerializeField]
    private Camera _camera;

    private Color[] _colorArray;

    private void OnEnable()
    {
        Font.textureRebuilt += Font_textureRebuilt;
    }

    private void OnDisable()
    {
        Font.textureRebuilt -= Font_textureRebuilt;
    }

    private void OnDestroy()
    {
        _material.SetVector("_MousePosition", Vector2.zero);
    }

    private void Start()
    {
        ConvertColors();

        ApplyShaderProps();

        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer) && spriteRenderer.enabled)
        {
            var fontTexture = _font.material.mainTexture as Texture2D;
            var sprite = Sprite.Create(
                fontTexture,
                new Rect(0, 0, fontTexture.width, fontTexture.height),
                new Vector2(0.5f, 0.5f)
            );
            spriteRenderer.sprite = sprite;
        }
    }

    private void Update()
    {
        _material.SetVector("_MousePosition", GetMousePositionUvCoord());
    }

    private void Font_textureRebuilt(Font font)
    {
        if (font == _font)
        {
            ApplyShaderProps();
        }
    }

    private Vector2 GetMousePositionUvCoord()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            return hit.textureCoord;
        }
        else
        {
            return Vector2.one * 5;
        }
    }

    private void ConvertColors()
    {
        _colorArray = new Color[HtmlColors.Length];

        for (int i = 1; i < _colorArray.Length; i++)
        {
            if (!ColorUtility.TryParseHtmlString(HtmlColors[i], out _colorArray[i]))
            {
                Debug.LogWarning(
                    string.Format("Ivalid html color <b>{0}</b> at index {1}", HtmlColors[i], i)
                );
            }
        }
    }

    private void ApplyShaderProps()
    {
        _font.RequestCharactersInTexture("0123456789", 150, FontStyle.Normal);

        _material.SetTexture("_FontTexture", _font.material.mainTexture);

        var charInfo = _font.characterInfo[4];

        _material.SetVector("_UVBottomLeft", charInfo.uvBottomLeft);
        _material.SetVector("_UVBottomRight", charInfo.uvBottomRight);
        _material.SetVector("_UVTopRight", charInfo.uvTopRight);
        _material.SetVector("_UVTopLeft", charInfo.uvTopLeft);

        _material.SetColorArray("_Colors", _colorArray);
    }
}
