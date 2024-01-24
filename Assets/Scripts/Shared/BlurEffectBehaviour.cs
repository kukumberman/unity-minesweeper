using UnityEngine;

public sealed class BlurEffectBehaviour : MonoBehaviour
{
    [SerializeField]
    private Material _material;

    [SerializeField]
    [Range(1, 10)]
    private int _iterations = 1;

    [SerializeField]
    [Range(0, 4)]
    private int _downsample = 0;

    [SerializeField]
    private Texture2D _texture;

    private Texture2D _result;

    public Texture2D Result => _result;

    private void OnEnable()
    {
        _result = ApplyBlur(_texture, _iterations, _downsample);

        if (
            gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer)
            && spriteRenderer.enabled
        )
        {
            var sprite = Sprite.Create(
                _result,
                new Rect(0, 0, _result.width, _result.height),
                Vector2.one * 0.5f
            );
            spriteRenderer.sprite = sprite;
        }
    }

    private void OnDisable()
    {
        DestroyObjectSafely(_result);
    }

    private Texture2D ApplyBlur(Texture2D texture, int iterations, int downsample)
    {
        var width = texture.width >> downsample;
        var height = texture.height >> downsample;

        var rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(texture, rt);

        for (int i = 0; i < iterations; i++)
        {
            var rt2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(rt, rt2, _material);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        var result = ConvertRenderTexture(rt);

        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    private static Texture2D ConvertRenderTexture(RenderTexture renderTexture)
    {
        var texture = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGBA32,
            false
        );

        var previousActive = RenderTexture.active;

        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = previousActive;

        return texture;
    }

    private static void DestroyObjectSafely(Object obj)
    {
        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else
        {
            DestroyImmediate(obj);
        }
    }
}
