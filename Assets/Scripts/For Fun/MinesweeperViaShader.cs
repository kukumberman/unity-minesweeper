using System.IO;
using UnityEngine;
using Kukumberman.Minesweeper.Core;

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

    [SerializeField]
    private Texture2D _runtimeTexture;

    [Header("Gameplay")]
    [SerializeField]
    private MinesweeperService _service;

    [SerializeField]
    private MinesweeperGameSettings _settings;

    [SerializeField]
    private string _seed;

    private Color[] _colorArray;

    private Vector2 _mousePositionUvCoord;

    private Color32[] _pixels;

    private Vector4[] _fontUvs = new Vector4[4 * 10];

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
        _material.SetTexture("_MainTex", null);
    }

    private void Start()
    {
        ConvertColors();

        _service.StartGame(_settings, _seed.GetHashCode());

        if (_runtimeTexture == null)
        {
            _runtimeTexture = CreateTexture();
        }

        _pixels = _runtimeTexture.GetPixels32();

        _material.SetTexture("_MainTex", _runtimeTexture);

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
        _mousePositionUvCoord = GetMousePositionUvCoord();

        _material.SetVector("_MousePosition", _mousePositionUvCoord);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            HandleLeftClick();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            HandleRightClick();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            HandleRestart();
        }
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

        for (int i = 0, j = 0; i < _font.characterInfo.Length; i++)
        {
            _fontUvs[j++] = _font.characterInfo[i].uvBottomLeft;
            _fontUvs[j++] = _font.characterInfo[i].uvBottomRight;
            _fontUvs[j++] = _font.characterInfo[i].uvTopRight;
            _fontUvs[j++] = _font.characterInfo[i].uvTopLeft;
        }

        _material.SetVectorArray("_FontUvs", _fontUvs);

        _material.SetColorArray("_Colors", _colorArray);

        _material.SetVector("_GridSize", new Vector2(_service.Game.Width, _service.Game.Height));
    }

    private Texture2D CreateTexture()
    {
        var texture = new Texture2D(
            _service.Game.Width,
            _service.Game.Height,
            TextureFormat.RGBA32,
            false
        );

        texture.filterMode = FilterMode.Point;

        var colors32 = new Color32[texture.width * texture.height];

        FillColors(colors32, _service);

        texture.SetPixels32(colors32);
        texture.Apply();

#if UNITY_EDITOR
        var path = Path.Combine(Application.dataPath, "minesweeper.png");
        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
#endif
        return texture;
    }

    private bool MousePositionToIndex(out int index)
    {
        int width = _service.Game.Width;
        int height = _service.Game.Height;

        int x = (int)(_mousePositionUvCoord.x * width);
        int y = (int)((1 - _mousePositionUvCoord.y) * height);

        index = y * width + x;

        return index >= 0 && index < _service.Game.CellsRef.Length;
    }

    private void HandleLeftClick()
    {
        if (MousePositionToIndex(out var index))
        {
            _service.RevealCell(index);
            SyncState();
        }
    }

    private void HandleRightClick()
    {
        if (MousePositionToIndex(out var index))
        {
            _service.FlagCell(index);
            SyncState();
        }
    }

    private void HandleRestart()
    {
        _service.Restart();
        SyncState();
    }

    private void SyncState()
    {
        FillColors(_pixels, _service);
        _runtimeTexture.SetPixels32(_pixels);
        _runtimeTexture.Apply();
    }

    private static Color32 CellToColorDebug(MinesweeperCell cell)
    {
        if (cell.Index == 0)
        {
            return new Color32(255, 0, 0, 255);
        }

        if (cell.Index == 1)
        {
            return new Color32(0, 255, 0, 255);
        }

        return new Color32(0, 0, 0, 255);
    }

    private static Color32 CellToColor(MinesweeperCell cell)
    {
        return new Color32
        {
            r = (byte)(cell.IsRevealed ? 0 : 255),
            g = (byte)(cell.IsFlag ? 100 : (cell.IsBomb ? 200 : 0)),
            b = (byte)cell.BombNeighborCount,
            a = 255
        };
    }

    private static void FillColors(Color32[] colors, MinesweeperService service)
    {
        var cells = service.Game.CellsRef;
        var grid = service.Game.Grid;

        var height = service.Game.Height;

        for (int i = 0; i < cells.Length; i++)
        {
            grid.ConvertTo2D(i, out var x, out var y);
            y = height - 1 - y;
            var colorIndex = grid.ConvertTo1D(x, y);
            colors[colorIndex] = CellToColor(cells[i]);
        }
    }

    public static Color Int32ToColor(uint hexValue)
    {
        byte r = (byte)((hexValue >> 24) & 0xFF);
        byte g = (byte)((hexValue >> 16) & 0xFF);
        byte b = (byte)((hexValue >> 8) & 0xFF);
        byte a = (byte)((hexValue) & 0xFF);
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static uint ColorToInt32(Color color)
    {
        uint hexValue = 0;
        hexValue |= (uint)(color.r * 0xFF) << 24;
        hexValue |= ((uint)(color.g * 0xFF)) << 16;
        hexValue |= ((uint)(color.b * 0xFF)) << 8;
        hexValue |= (uint)(color.a * 0xFF);
        return hexValue;
    }
}
