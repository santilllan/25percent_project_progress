using UnityEngine;
using UnityEngine.UI;

public static class UiKit
{
    static Font _font;
    static Sprite _whiteSprite;
    static Sprite _triangleSprite;
    static Sprite _circleSprite;

    public static Font Font
    {
        get
        {
            if (_font == null)
                _font = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "Liberation Sans", "DejaVu Sans" }, 36);
            return _font;
        }
    }

    public static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                Texture2D tex = new Texture2D(8, 8, TextureFormat.RGBA32, false);
                Color[] pixels = new Color[64];
                for (int i = 0; i < 64; i++) pixels[i] = Color.white;
                tex.SetPixels(pixels);
                tex.Apply();
                _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8f);
            }
            return _whiteSprite;
        }
    }

    public static Sprite TriangleSprite
    {
        get
        {
            if (_triangleSprite == null)
            {
                int res = 64;
                Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
                Color[] px = new Color[res * res];
                for (int i = 0; i < px.Length; i++) px[i] = Color.clear;
                for (int y = 0; y < res; y++)
                    for (int x = 0; x < res; x++)
                    {
                        float nx = (float)x / res;
                        float ny = (float)y / res;
                        if (ny <= 1f - Mathf.Abs(nx * 2f - 1f))
                            px[y * res + x] = Color.white;
                    }
                tex.SetPixels(px);
                tex.Apply();
                _triangleSprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0f), res);
            }
            return _triangleSprite;
        }
    }

    public static Sprite CircleSprite
    {
        get
        {
            if (_circleSprite == null)
            {
                int res = 64;
                Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
                Color[] px = new Color[res * res];
                float center = res / 2f;
                float radius = center - 1f;
                for (int y = 0; y < res; y++)
                    for (int x = 0; x < res; x++)
                    {
                        float dx = x - center + 0.5f;
                        float dy = y - center + 0.5f;
                        px[y * res + x] = (dx * dx + dy * dy <= radius * radius) ? Color.white : Color.clear;
                    }
                tex.SetPixels(px);
                tex.Apply();
                _circleSprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
            }
            return _circleSprite;
        }
    }

    public static Canvas CreateCanvas(string name, int sortingOrder, Vector2 referenceResolution)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    public static RectTransform CreateScreen(Transform parent, string name)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return rt;
    }

    public static Text CreateText(Transform parent, string text, float size, Color color,
        TextAnchor alignment = TextAnchor.MiddleCenter, float width = 900f, float height = 110f)
    {
        GameObject go = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = Vector2.zero;
        Text t = go.GetComponent<Text>();
        t.font = Font;
        t.text = text;
        t.fontSize = (int)size;
        t.color = color;
        t.alignment = alignment;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    public static Button CreateButton(Transform parent, string label, Vector2 position, Vector2 size, Color color, System.Action onClick)
    {
        GameObject go = new GameObject("Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        Image img = go.GetComponent<Image>();
        img.color = color;
        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        CreateText(go.transform, label, size.y * 0.32f, Color.white, TextAnchor.MiddleCenter, size.x - 20f, size.y);
        btn.onClick.AddListener(() => onClick());
        return btn;
    }
}