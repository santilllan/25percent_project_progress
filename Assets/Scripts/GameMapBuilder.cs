using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GameMapBuilder
{
    public const string GameSceneName = "Game Scene Test";
    public const string MainMenuSceneName = "MainMenu";

    static readonly Color Skin = new Color(0.9f, 0.75f, 0.6f);
    static readonly Color SkinShade = new Color(0.78f, 0.62f, 0.48f);
    static readonly Color P1 = new Color(0.2f, 0.45f, 0.85f);
    static readonly Color P2 = new Color(0.85f, 0.2f, 0.2f);

    public static void Build()
    {
        Time.timeScale = 1f;
        HideTemplate();
        BuildScene();
        BuildHud();
    }

    static void HideTemplate()
    {
        var c = GameObject.Find("Canvas");
        if (c != null) c.SetActive(false);
    }

    static void BuildScene()
    {
        var root = new GameObject("Scene");
        string r = GameState.Region;
        switch (r)
        {
            case "Mindanao": BuildMindanao(root.transform); break;
            case "Visayas": BuildVisayas(root.transform); break;
            default: BuildLuzon(root.transform); break;
        }
        BuildChars(root.transform, r);
    }

    static void SetCam(Color sky, float sz = 6f)
    {
        var c = Camera.main;
        if (c == null) return;
        c.orthographic = true;
        c.orthographicSize = sz;
        c.clearFlags = CameraClearFlags.SolidColor;
        c.backgroundColor = sky;
        c.transform.position = new Vector3(0, 0, -10);
        c.transform.rotation = Quaternion.identity;
    }

    static GameObject Mk(Transform p, Sprite s, Vector3 pos, Vector3 sc, Color col, int o)
    {
        var g = new GameObject("S");
        g.transform.SetParent(p, false);
        g.transform.position = pos;
        g.transform.localScale = sc;
        var sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.color = col;
        sr.sortingOrder = o;
        return g;
    }

    // ---------- atmosphere ----------
    static void SkyGradient(Transform p, Color top, Color mid, Color bot)
    {
        for (int i = 0; i < 12; i++)
        {
            float t = i / 11f;
            Color c = t < 0.5f ? Color.Lerp(top, mid, t * 2f) : Color.Lerp(mid, bot, (t - 0.5f) * 2f);
            float y = 6f - i * 1.05f;
            Mk(p, UiKit.WhiteSprite, new Vector3(0, y, 0), new Vector3(28f, 1.2f, 1), c, -10);
        }
    }

    static void Shadow(Transform p, Vector3 pos, float w, int o)
    {
        Mk(p, UiKit.CircleSprite, pos, new Vector3(w, w * 0.22f, 1), new Color(0, 0, 0, 0.22f), o);
    }

    static void SunReal(Transform p, Vector3 pos, float sz, Color core, Color glow, int o)
    {
        Mk(p, UiKit.CircleSprite, pos, new Vector3(sz * 2.6f, sz * 2.6f, 1), new Color(glow.r, glow.g, glow.b, 0.12f), o - 2);
        Mk(p, UiKit.CircleSprite, pos, new Vector3(sz * 1.7f, sz * 1.7f, 1), new Color(glow.r, glow.g, glow.b, 0.25f), o - 1);
        Mk(p, UiKit.CircleSprite, pos, new Vector3(sz, sz, 1), core, o);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-sz * 0.1f, sz * 0.12f, 0), new Vector3(sz * 0.5f, sz * 0.5f, 1), new Color(1, 1, 1, 0.5f), o + 1);
    }

    static void RealCloud(Transform p, Vector3 pos, float w, Color top, Color shade, int o)
    {
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, -w * 0.07f, 0), new Vector3(w, w * 0.4f, 1), shade, o);
        Mk(p, UiKit.CircleSprite, pos, new Vector3(w * 0.94f, w * 0.36f, 1), top, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-w * 0.28f, w * 0.1f, 0), new Vector3(w * 0.4f, w * 0.34f, 1), top, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(w * 0.28f, w * 0.07f, 0), new Vector3(w * 0.46f, w * 0.31f, 1), top, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(w * 0.02f, w * 0.16f, 0), new Vector3(w * 0.34f, w * 0.28f, 1), new Color(1, 1, 1, 0.95f), o + 1);
    }

    static void Bird(Transform p, Vector3 pos, int o)
    {
        var a = Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.11f, 0.05f, 0), new Vector3(0.2f, 0.035f, 1), new Color(0.22f, 0.22f, 0.28f), o);
        a.transform.rotation = Quaternion.Euler(0, 0, 22f);
        var b = Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.11f, 0.05f, 0), new Vector3(0.2f, 0.035f, 1), new Color(0.22f, 0.22f, 0.28f), o);
        b.transform.rotation = Quaternion.Euler(0, 0, -22f);
    }

    // ---------- terrain ----------
    static void MtnReal(Transform p, Vector3 pos, Vector3 sc, Color main, Color shade, Color forest, Color snow, bool snowy, int o)
    {
        Mk(p, UiKit.TriangleSprite, pos, sc, main, o);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(sc.x * 0.16f, 0, 0), new Vector3(sc.x * 0.62f, sc.y, 1), shade, o);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, sc.y * 0.1f, 0), new Vector3(sc.x * 0.85f, sc.y * 0.2f, 1), forest, o + 1);
        if (snowy)
        {
            Mk(p, UiKit.TriangleSprite, pos + new Vector3(0, sc.y * 0.66f, 0), new Vector3(sc.x * 0.3f, sc.y * 0.34f, 1), snow, o + 1);
            Mk(p, UiKit.TriangleSprite, pos + new Vector3(sc.x * 0.02f, sc.y * 0.7f, 0), new Vector3(sc.x * 0.14f, sc.y * 0.3f, 1), new Color(shade.r, shade.g, shade.b, 0.55f), o + 1);
        }
    }

    static void HillReal(Transform p, Vector3 pos, Vector3 sc, Color main, Color dark, int o)
    {
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-sc.x * 0.2f, -sc.y * 0.08f, 0), new Vector3(sc.x * 0.7f, sc.y * 0.8f, 1), dark, o);
        Mk(p, UiKit.CircleSprite, pos, sc, main, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-sc.x * 0.1f, sc.y * 0.18f, 0), new Vector3(sc.x * 0.4f, sc.y * 0.35f, 1), new Color(Mathf.Min(1, main.r + 0.08f), Mathf.Min(1, main.g + 0.08f), main.b, 1), o + 1);
    }

    static void WaterReal(Transform p, Color deep, Color surf, Color streak, float sunX, bool sunset, int o)
    {
        // River channel runs full depth to the screen bottom so nothing floats. Half-width 1.8.
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -4.05f, 0), new Vector3(3.7f, 4.6f, 1), deep, o);
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -5.85f, 0), new Vector3(3.7f, 0.9f, 1), new Color(deep.r * 0.7f, deep.g * 0.7f, deep.b * 0.75f), o);
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -2.5f, 0), new Vector3(3.5f, 0.35f, 1), surf, o + 1);
        float sx = Mathf.Clamp(sunX, -1.1f, 1.1f);
        if (sunset)
        {
            Mk(p, UiKit.WhiteSprite, new Vector3(sx, -3.6f, 0), new Vector3(0.9f, 3.4f, 1), new Color(streak.r, streak.g, streak.b, 0.5f), o + 1);
            Mk(p, UiKit.WhiteSprite, new Vector3(sx, -3.4f, 0), new Vector3(0.45f, 3f, 1), new Color(1, 0.9f, 0.7f, 0.55f), o + 1);
        }
        Mk(p, UiKit.WhiteSprite, new Vector3(-1.1f, -2.35f, 0), new Vector3(0.7f, 0.06f, 1), new Color(1, 1, 1, 0.28f), o + 2);
        Mk(p, UiKit.WhiteSprite, new Vector3(0.4f, -2.8f, 0), new Vector3(0.55f, 0.05f, 1), new Color(1, 1, 1, 0.2f), o + 2);
        Mk(p, UiKit.WhiteSprite, new Vector3(1f, -3.3f, 0), new Vector3(0.8f, 0.05f, 1), new Color(1, 1, 1, 0.15f), o + 2);
        Mk(p, UiKit.WhiteSprite, new Vector3(-0.4f, -3.9f, 0), new Vector3(0.6f, 0.05f, 1), new Color(1, 1, 1, 0.12f), o + 2);
        Mk(p, UiKit.WhiteSprite, new Vector3(0.6f, -4.6f, 0), new Vector3(0.5f, 0.05f, 1), new Color(1, 1, 1, 0.1f), o + 2);
        Mk(p, UiKit.WhiteSprite, new Vector3(-1.78f, -4f, 0), new Vector3(0.12f, 4.3f, 1), new Color(1, 1, 1, 0.3f), o + 2);
        Mk(p, UiKit.WhiteSprite, new Vector3(1.78f, -4f, 0), new Vector3(0.12f, 4.3f, 1), new Color(1, 1, 1, 0.3f), o + 2);
    }

    static void Lily(Transform p, Vector3 pos, int o)
    {
        Mk(p, UiKit.CircleSprite, pos, new Vector3(0.28f, 0.1f, 1), new Color(0.2f, 0.5f, 0.25f), o);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.05f, 0.05f, 0), new Vector3(0.09f, 0.09f, 1), new Color(1f, 0.6f, 0.75f), o);
    }

    static void Reed(Transform p, Vector3 pos, Color stalk, int o)
    {
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.06f, 0.25f, 0), new Vector3(0.03f, 0.5f, 1), stalk, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.05f, 0.2f, 0), new Vector3(0.03f, 0.4f, 1), stalk, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.06f, 0.5f, 0), new Vector3(0.07f, 0.14f, 1), new Color(0.4f, 0.25f, 0.12f), o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.05f, 0.42f, 0), new Vector3(0.06f, 0.12f, 1), new Color(0.42f, 0.27f, 0.13f), o);
    }

    static void GroundReal(Transform p, Color grassTop, Color earth, Color soilDark, Color speck, int o)
    {
        // Full-width base fills all the way to the screen bottom so the platform never floats.
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -4.05f, 0), new Vector3(26f, 4.6f, 1), earth, o);
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -5.85f, 0), new Vector3(26f, 1f, 1), soilDark, o);
        // Dirt cliff faces where the river channel (half-width 1.8) cuts through.
        Color bank = new Color(earth.r * 0.76f, earth.g * 0.76f, earth.b * 0.76f);
        Mk(p, UiKit.WhiteSprite, new Vector3(-1.95f, -4f, 0), new Vector3(0.35f, 4.4f, 1), bank, o + 1);
        Mk(p, UiKit.WhiteSprite, new Vector3(1.95f, -4f, 0), new Vector3(0.35f, 4.4f, 1), bank, o + 1);
        // Wide grass lips across both platforms, overlapping the channel edges.
        Color grassDark = new Color(grassTop.r * 0.7f, grassTop.g * 0.8f, grassTop.b * 0.7f);
        foreach (float cx in new float[] { -7.5f, 7.5f })
        {
            Mk(p, UiKit.WhiteSprite, new Vector3(cx, -1.84f, 0), new Vector3(11.2f, 0.18f, 1), grassTop, o + 1);
            Mk(p, UiKit.WhiteSprite, new Vector3(cx, -1.96f, 0), new Vector3(11.2f, 0.07f, 1), grassDark, o + 1);
        }
        for (float x = -10f; x <= 10f; x += 0.45f)
        {
            if (Mathf.Abs(x) < 2.1f) continue;
            float y = -2.2f - (Mathf.Abs(Mathf.Sin(x * 3.7f)) * 3f);
            float s = 0.04f + Mathf.Abs(Mathf.Sin(x * 12.9f)) * 0.05f;
            Mk(p, UiKit.CircleSprite, new Vector3(x, y, 0), new Vector3(s, s * 0.7f, 1), speck, o + 1);
        }
        for (float x = -9.6f; x <= 9.6f; x += 1.7f)
        {
            if (Mathf.Abs(x) < 2.2f) continue;
            Mk(p, UiKit.CircleSprite, new Vector3(x + 0.2f, -2.6f, 0), new Vector3(0.12f, 0.08f, 1), new Color(0.55f, 0.55f, 0.56f), o + 1);
        }
        Color g1 = new Color(grassTop.r * 0.8f, grassTop.g * 1.05f, grassTop.b * 0.8f);
        for (float x = -10f; x <= 10f; x += 0.65f)
        {
            if (Mathf.Abs(x) < 2f) continue;
            GrassTuft(p, new Vector3(x, -1.73f, 0), g1, o + 2);
        }
    }

    static void GrassTuft(Transform p, Vector3 pos, Color c, int o)
    {
        Mk(p, UiKit.TriangleSprite, pos, new Vector3(0.07f, 0.2f, 1), c, o);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(0.06f, 0.01f, 0), new Vector3(0.055f, 0.15f, 1), c, o);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(-0.055f, 0.01f, 0), new Vector3(0.06f, 0.17f, 1), new Color(c.r * 0.85f, c.g, c.b * 0.85f), o);
    }

    static void FlowerReal(Transform p, Vector3 pos, Color c, int o)
    {
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.05f, 0), new Vector3(0.02f, 0.1f, 1), new Color(0.25f, 0.5f, 0.2f), o);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 0.12f, 0), new Vector3(0.1f, 0.1f, 1), c, o);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.015f, 0.13f, 0), new Vector3(0.045f, 0.045f, 1), new Color(1, 0.95f, 0.6f), o);
    }

    static void RockReal(Transform p, Vector3 pos, float sz, int o)
    {
        Shadow(p, pos + new Vector3(0, -0.02f, 0), sz * 1.2f, o);
        Mk(p, UiKit.CircleSprite, pos, new Vector3(sz, sz * 0.68f, 1), new Color(0.52f, 0.53f, 0.55f), o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-sz * 0.15f, sz * 0.14f, 0), new Vector3(sz * 0.5f, sz * 0.3f, 1), new Color(0.68f, 0.69f, 0.71f), o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(sz * 0.28f, -sz * 0.05f, 0), new Vector3(sz * 0.3f, sz * 0.2f, 1), new Color(0.42f, 0.52f, 0.35f), o + 1);
    }

    static void BushReal(Transform p, Vector3 pos, Color main, int o)
    {
        Shadow(p, pos + new Vector3(0, -0.05f, 0), 0.9f, o);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-0.22f, 0.12f, 0), new Vector3(0.45f, 0.36f, 1), new Color(main.r * 0.85f, main.g * 0.9f, main.b * 0.85f), o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.22f, 0.12f, 0), new Vector3(0.42f, 0.34f, 1), new Color(main.r * 0.9f, main.g * 0.92f, main.b * 0.9f), o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 0.28f, 0), new Vector3(0.55f, 0.42f, 1), main, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-0.08f, 0.36f, 0), new Vector3(0.25f, 0.18f, 1), new Color(Mathf.Min(1, main.r + 0.12f), Mathf.Min(1, main.g + 0.12f), main.b, 1), o + 2);
    }

    // ---------- structures ----------
    static void BridgeReal(Transform p, Color plank, Color dark, int o)
    {
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -2.6f, 0), new Vector3(4.4f, 1.5f, 1), new Color(0, 0, 0, 0.18f), o - 1);
        Mk(p, UiKit.WhiteSprite, new Vector3(-2, -2.6f, 0), new Vector3(0.24f, 1.6f, 1), dark, o);
        Mk(p, UiKit.WhiteSprite, new Vector3(2, -2.6f, 0), new Vector3(0.24f, 1.6f, 1), dark, o);
        Mk(p, UiKit.WhiteSprite, new Vector3(-2, -2.4f, 0), new Vector3(0.3f, 0.1f, 1), new Color(dark.r * 0.8f, dark.g * 0.8f, dark.b * 0.8f), o);
        Mk(p, UiKit.WhiteSprite, new Vector3(2, -2.4f, 0), new Vector3(0.3f, 0.1f, 1), new Color(dark.r * 0.8f, dark.g * 0.8f, dark.b * 0.8f), o);
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -1.85f, 0), new Vector3(4.6f, 0.2f, 1), plank, o + 1);
        for (float x = -2.1f; x <= 2.1f; x += 0.42f)
            Mk(p, UiKit.WhiteSprite, new Vector3(x, -1.85f, 0), new Vector3(0.03f, 0.2f, 1), dark, o + 1);
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -1.73f, 0), new Vector3(4.6f, 0.05f, 1), new Color(plank.r * 1.1f > 1 ? 1 : plank.r * 1.1f, plank.g * 1.05f, plank.b, 1), o + 1);
        for (float x = -2.2f; x <= 2.2f; x += 1.1f)
            Mk(p, UiKit.WhiteSprite, new Vector3(x, -1.5f, 0), new Vector3(0.07f, 0.6f, 1), dark, o + 1);
        Mk(p, UiKit.WhiteSprite, new Vector3(0, -1.28f, 0), new Vector3(4.6f, 0.07f, 1), dark, o + 1);
    }

    static void PalmReal(Transform p, Vector3 pos, Color trunk, Color leaf, Color leafDark, int o, bool coconuts)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 1f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.06f, 0.5f, 0), new Vector3(0.16f, 1.1f, 1), new Color(trunk.r * 0.85f, trunk.g * 0.85f, trunk.b * 0.85f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.04f, 1.3f, 0), new Vector3(0.15f, 1.1f, 1), trunk, o + 1);
        for (float y = 0.2f; y <= 1.6f; y += 0.35f)
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(0f, y, 0), new Vector3(0.19f, 0.05f, 1), new Color(trunk.r * 0.7f, trunk.g * 0.7f, trunk.b * 0.7f), o + 1);
        Vector3 top = pos + new Vector3(0.08f, 1.9f, 0);
        float[] angs = { 8f, 38f, 68f, 155f, 185f, 215f, 115f };
        for (int i = 0; i < angs.Length; i++)
        {
            Color c = i % 2 == 0 ? leaf : leafDark;
            var f = Mk(p, UiKit.CircleSprite, top, new Vector3(1.05f, 0.26f, 1), c, o + 2);
            f.transform.rotation = Quaternion.Euler(0, 0, angs[i]);
            f.transform.position = top + new Vector3(Mathf.Cos(angs[i] * Mathf.Deg2Rad) * 0.45f, Mathf.Sin(angs[i] * Mathf.Deg2Rad) * 0.45f, 0);
        }
        Mk(p, UiKit.CircleSprite, top + new Vector3(0, 0.1f, 0), new Vector3(0.4f, 0.3f, 1), leafDark, o + 2);
        if (coconuts)
        {
            Mk(p, UiKit.CircleSprite, top + new Vector3(-0.12f, -0.08f, 0), new Vector3(0.14f, 0.14f, 1), new Color(0.42f, 0.28f, 0.12f), o + 2);
            Mk(p, UiKit.CircleSprite, top + new Vector3(0.1f, -0.1f, 0), new Vector3(0.13f, 0.13f, 1), new Color(0.45f, 0.3f, 0.14f), o + 2);
            Mk(p, UiKit.CircleSprite, top + new Vector3(0f, -0.14f, 0), new Vector3(0.12f, 0.12f, 1), new Color(0.38f, 0.25f, 0.11f), o + 2);
        }
    }

    static void NipaHutReal(Transform p, Vector3 pos, int o)
    {
        Color wall = new Color(0.78f, 0.58f, 0.33f);
        Color wallDark = new Color(0.62f, 0.44f, 0.24f);
        Color thatch = new Color(0.5f, 0.33f, 0.15f);
        Color thatchLight = new Color(0.62f, 0.43f, 0.2f);
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 1.8f, o);
        foreach (float sx in new float[] { -0.55f, -0.2f, 0.2f, 0.55f })
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(sx, 0.25f, 0), new Vector3(0.09f, 0.55f, 1), wallDark, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.55f, 0), new Vector3(1.35f, 0.1f, 1), wallDark, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.95f, 0), new Vector3(1.25f, 0.75f, 1), wall, o + 1);
        for (float x = -0.55f; x <= 0.55f; x += 0.18f)
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(x, 0.95f, 0), new Vector3(0.025f, 0.75f, 1), new Color(wallDark.r, wallDark.g, wallDark.b, 0.7f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.33f, 1f, 0), new Vector3(0.3f, 0.4f, 1), new Color(0.25f, 0.18f, 0.1f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.33f, 1f, 0), new Vector3(0.24f, 0.32f, 1), new Color(1f, 0.85f, 0.55f, 0.9f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.33f, 1f, 0), new Vector3(0.24f, 0.04f, 1), wallDark, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.32f, 0.82f, 0), new Vector3(0.3f, 0.6f, 1), new Color(0.35f, 0.22f, 0.11f), o + 2);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(0, 1.28f, 0), new Vector3(1.75f, 0.75f, 1), thatch, o + 2);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(0, 1.45f, 0), new Vector3(1.45f, 0.6f, 1), thatchLight, o + 2);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(0, 1.62f, 0), new Vector3(1.1f, 0.45f, 1), thatch, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.95f, 0), new Vector3(0.35f, 0.08f, 1), new Color(0.35f, 0.22f, 0.1f), o + 2);
        var lad = Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.85f, 0.35f, 0), new Vector3(0.07f, 0.9f, 1), wallDark, o + 1);
        lad.transform.rotation = Quaternion.Euler(0, 0, -14f);
    }

    static void FenceReal(Transform p, Vector3 pos, float width, Color c, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), width, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.18f, 0), new Vector3(width, 0.06f, 1), c, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.34f, 0), new Vector3(width, 0.05f, 1), new Color(c.r * 1.05f > 1 ? 1 : c.r * 1.05f, c.g * 1.05f > 1 ? 1 : c.g * 1.05f, c.b, 1), o + 1);
        for (float x = -width * 0.5f; x <= width * 0.5f; x += 0.32f)
        {
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(x, 0.24f, 0), new Vector3(0.07f, 0.42f, 1), c, o + 1);
            Mk(p, UiKit.TriangleSprite, pos + new Vector3(x, 0.42f, 0), new Vector3(0.07f, 0.08f, 1), c, o + 1);
        }
    }

    static void PHFlagReal(Transform p, Vector3 pos, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 0.5f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.3f, 0), new Vector3(0.09f, 2.6f, 1), new Color(0.55f, 0.55f, 0.58f), o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 2.65f, 0), new Vector3(0.12f, 0.12f, 1), new Color(0.85f, 0.7f, 0.2f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.36f, 2.32f, 0), new Vector3(0.62f, 0.19f, 1), new Color(0.08f, 0.28f, 0.75f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.36f, 2.13f, 0), new Vector3(0.62f, 0.19f, 1), new Color(0.85f, 0.15f, 0.15f), o + 1);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(0.05f, 2.38f, 0), new Vector3(0.24f, 0.36f, 1), Color.white, o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.12f, 2.22f, 0), new Vector3(0.07f, 0.07f, 1), new Color(0.9f, 0.75f, 0.2f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.55f, 2.22f, 0), new Vector3(0.05f, 0.38f, 1), new Color(0, 0, 0, 0.12f), o + 2);
    }

    static void USFlagReal(Transform p, Vector3 pos, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 0.5f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.35f, 0), new Vector3(0.1f, 2.7f, 1), new Color(0.45f, 0.42f, 0.4f), o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 2.75f, 0), new Vector3(0.12f, 0.12f, 1), new Color(0.85f, 0.7f, 0.2f), o + 1);
        for (int i = 0; i < 7; i++)
        {
            Color c = i % 2 == 0 ? new Color(0.72f, 0.14f, 0.14f) : new Color(0.95f, 0.93f, 0.9f);
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.36f, 2.42f - i * 0.075f, 0), new Vector3(0.6f, 0.07f, 1), c, o + 1);
        }
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.16f, 2.38f, 0), new Vector3(0.24f, 0.2f, 1), new Color(0.12f, 0.14f, 0.5f), o + 2);
        for (float x = 0.07f; x <= 0.25f; x += 0.06f)
            for (float y = 2.32f; y <= 2.44f; y += 0.06f)
                Mk(p, UiKit.CircleSprite, pos + new Vector3(x, y, 0), new Vector3(0.022f, 0.022f, 1), Color.white, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.58f, 2.2f, 0), new Vector3(0.05f, 0.5f, 1), new Color(0, 0, 0, 0.14f), o + 2);
    }

    static void JPFlagReal(Transform p, Vector3 pos, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 0.5f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.3f, 0), new Vector3(0.09f, 2.6f, 1), new Color(0.45f, 0.42f, 0.4f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.34f, 2.2f, 0), new Vector3(0.56f, 0.4f, 1), new Color(0.96f, 0.94f, 0.9f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.55f, 2.2f, 0), new Vector3(0.05f, 0.4f, 1), new Color(0, 0, 0, 0.1f), o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.33f, 2.2f, 0), new Vector3(0.2f, 0.2f, 1), new Color(0.82f, 0.1f, 0.1f), o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.3f, 2.23f, 0), new Vector3(0.07f, 0.07f, 1), new Color(1, 0.5f, 0.5f, 0.7f), o + 2);
    }

    static void MonumentReal(Transform p, Vector3 pos, Color stone, Color gold, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 1.1f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.07f, 0), new Vector3(0.95f, 0.14f, 1), new Color(stone.r * 0.65f, stone.g * 0.65f, stone.b * 0.65f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.2f, 0), new Vector3(0.8f, 0.14f, 1), new Color(stone.r * 0.8f, stone.g * 0.8f, stone.b * 0.8f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.75f, 0), new Vector3(0.62f, 1.1f, 1), stone, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.2f, 0.75f, 0), new Vector3(0.1f, 1.1f, 1), new Color(1, 1, 1, 0.18f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.8f, 0), new Vector3(0.4f, 0.5f, 1), new Color(stone.r * 0.7f, stone.g * 0.7f, stone.b * 0.7f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.8f, 0), new Vector3(0.3f, 0.04f, 1), new Color(1, 1, 1, 0.25f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.38f, 0), new Vector3(0.5f, 0.12f, 1), new Color(stone.r * 0.85f, stone.g * 0.85f, stone.b * 0.85f), o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 1.66f, 0), new Vector3(0.32f, 0.32f, 1), gold, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-0.05f, 1.7f, 0), new Vector3(0.1f, 0.1f, 1), new Color(1, 1, 1, 0.7f), o + 2);
    }

    static void WoodHouseReal(Transform p, Vector3 pos, int o)
    {
        Color wall = new Color(0.72f, 0.52f, 0.3f);
        Color trim = new Color(0.92f, 0.88f, 0.8f);
        Color roof = new Color(0.42f, 0.3f, 0.16f);
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 2.2f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.12f, 0), new Vector3(1.7f, 0.24f, 1), new Color(0.5f, 0.5f, 0.52f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.65f, 0), new Vector3(1.55f, 1.05f, 1), wall, o + 1);
        for (float y = 0.3f; y <= 1f; y += 0.18f)
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, y, 0), new Vector3(1.55f, 0.025f, 1), new Color(wall.r * 0.85f, wall.g * 0.85f, wall.b * 0.85f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.78f, 0.65f, 0), new Vector3(0.08f, 1.05f, 1), trim, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.78f, 0.65f, 0), new Vector3(0.08f, 1.05f, 1), trim, o + 2);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(0, 1.15f, 0), new Vector3(1.9f, 0.65f, 1), roof, o + 2);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(-0.1f, 1.18f, 0), new Vector3(1.5f, 0.55f, 1), new Color(roof.r * 1.15f, roof.g * 1.15f, roof.b * 1.15f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.16f, 0), new Vector3(1.95f, 0.06f, 1), new Color(roof.r * 0.7f, roof.g * 0.7f, roof.b * 0.7f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.5f, 1.7f, 0), new Vector3(0.18f, 0.6f, 1), new Color(0.55f, 0.35f, 0.25f), o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.5f, 2.25f, 0), new Vector3(0.35f, 0.25f, 1), new Color(0.4f, 0.38f, 0.42f, 0.5f), o);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.55f, 2.6f, 0), new Vector3(0.5f, 0.35f, 1), new Color(0.4f, 0.38f, 0.42f, 0.35f), o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.4f, 0.68f, 0), new Vector3(0.34f, 0.42f, 1), new Color(1f, 0.82f, 0.5f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.4f, 0.68f, 0), new Vector3(0.34f, 0.04f, 1), trim, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.4f, 0.68f, 0), new Vector3(0.04f, 0.42f, 1), trim, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.4f, 0.68f, 0), new Vector3(0.3f, 0.38f, 1), new Color(0.4f, 0.55f, 0.7f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.4f, 0.68f, 0), new Vector3(0.3f, 0.04f, 1), trim, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.42f, 0), new Vector3(0.32f, 0.6f, 1), new Color(0.32f, 0.2f, 0.1f), o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.1f, 0.42f, 0), new Vector3(0.05f, 0.05f, 1), new Color(0.9f, 0.8f, 0.4f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.12f, 0), new Vector3(0.5f, 0.08f, 1), new Color(0.4f, 0.4f, 0.42f), o + 2);
    }

    static void DockReal(Transform p, Vector3 pos, int o)
    {
        Color plank = new Color(0.62f, 0.43f, 0.2f);
        Color pil = new Color(0.45f, 0.29f, 0.13f);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, -0.6f, 0), new Vector3(3.9f, 1.2f, 1), new Color(0, 0, 0, 0.2f), o - 1);
        for (float x = -1.6f; x <= 1.6f; x += 0.8f)
        {
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(x, -0.7f, 0), new Vector3(0.13f, 1.5f, 1), pil, o);
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(x, -2.2f, 0), new Vector3(0.15f, 0.25f, 1), new Color(0.25f, 0.4f, 0.25f), o);
        }
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0, 0), new Vector3(3.9f, 0.18f, 1), plank, o + 1);
        for (float x = -1.85f; x <= 1.85f; x += 0.3f)
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(x, 0, 0), new Vector3(0.025f, 0.18f, 1), new Color(pil.r, pil.g, pil.b, 0.8f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.1f, 0), new Vector3(3.9f, 0.045f, 1), new Color(plank.r * 1.12f > 1 ? 1 : plank.r * 1.12f, plank.g * 1.1f, plank.b * 1.05f, 1), o + 1);
        foreach (float x in new float[] { -1.7f, -0.5f, 0.7f, 1.7f })
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(x, 0.3f, 0), new Vector3(0.08f, 0.45f, 1), pil, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.45f, 0), new Vector3(3.9f, 0.06f, 1), pil, o + 1);
    }

    static void BarrelReal(Transform p, Vector3 pos, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 0.35f, o);
        Color c = new Color(0.55f, 0.36f, 0.18f);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.18f, 0), new Vector3(0.24f, 0.34f, 1), c, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.06f, 0.18f, 0), new Vector3(0.07f, 0.34f, 1), new Color(c.r * 1.2f > 1 ? 1 : c.r * 1.2f, c.g * 1.15f, c.b, 1), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.26f, 0), new Vector3(0.26f, 0.045f, 1), new Color(0.3f, 0.3f, 0.32f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.1f, 0), new Vector3(0.26f, 0.045f, 1), new Color(0.3f, 0.3f, 0.32f), o + 2);
    }

    static void CrateReal(Transform p, Vector3 pos, float sz, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), sz * 1.2f, o);
        Color c = new Color(0.66f, 0.48f, 0.24f);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, sz * 0.5f, 0), new Vector3(sz, sz, 1), c, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, sz * 0.5f, 0), new Vector3(sz, 0.04f, 1), new Color(c.r * 0.8f, c.g * 0.8f, c.b * 0.8f), o + 2);
        var d1 = Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, sz * 0.5f, 0), new Vector3(sz * 1.3f, 0.045f, 1), new Color(c.r * 0.8f, c.g * 0.8f, c.b * 0.8f), o + 2);
        d1.transform.rotation = Quaternion.Euler(0, 0, 45f);
        var d2 = Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, sz * 0.5f, 0), new Vector3(sz * 1.3f, 0.045f, 1), new Color(c.r * 0.8f, c.g * 0.8f, c.b * 0.8f), o + 2);
        d2.transform.rotation = Quaternion.Euler(0, 0, -45f);
    }

    static void BoatReal(Transform p, Vector3 pos, int o)
    {
        Color hull = new Color(0.48f, 0.3f, 0.14f);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, -0.35f, 0), new Vector3(0.9f, 0.5f, 1), new Color(hull.r, hull.g, hull.b, 0.25f), o - 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0, 0), new Vector3(0.95f, 0.14f, 1), hull, o);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(-0.47f, 0, 0), new Vector3(0.22f, 0.22f, 1), hull, o);
        var tip = Mk(p, UiKit.TriangleSprite, pos + new Vector3(0.47f, 0, 0), new Vector3(0.22f, 0.22f, 1), hull, o);
        tip.transform.rotation = Quaternion.Euler(0, 180f, 0);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.08f, 0), new Vector3(0.95f, 0.04f, 1), new Color(hull.r * 1.25f > 1 ? 1 : hull.r * 1.25f, hull.g * 1.2f, hull.b, 1), o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.15f, 0.12f, 0), new Vector3(0.5f, 0.04f, 1), new Color(0.6f, 0.42f, 0.2f), o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.35f, 0), new Vector3(0.04f, 0.5f, 1), new Color(0.4f, 0.27f, 0.13f), o);
        var sail = Mk(p, UiKit.TriangleSprite, pos + new Vector3(0.16f, 0.3f, 0), new Vector3(0.35f, 0.45f, 1), new Color(0.92f, 0.88f, 0.78f), o);
        sail.transform.rotation = Quaternion.Euler(0, 0, -12f);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.6f, -0.15f, 0), new Vector3(0.4f, 0.04f, 1), new Color(1, 1, 1, 0.3f), o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.5f, -0.2f, 0), new Vector3(0.3f, 0.035f, 1), new Color(1, 1, 1, 0.25f), o);
    }

    static void ToriiReal(Transform p, Vector3 pos, int o)
    {
        Color red = new Color(0.78f, 0.12f, 0.1f);
        Color redDark = new Color(0.58f, 0.08f, 0.07f);
        Color black = new Color(0.12f, 0.1f, 0.1f);
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 2.4f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.85f, 0.1f, 0), new Vector3(0.4f, 0.2f, 1), new Color(0.55f, 0.55f, 0.57f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.85f, 0.1f, 0), new Vector3(0.4f, 0.2f, 1), new Color(0.55f, 0.55f, 0.57f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.85f, 1f, 0), new Vector3(0.2f, 1.9f, 1), red, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.85f, 1f, 0), new Vector3(0.2f, 1.9f, 1), red, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.9f, 1f, 0), new Vector3(0.05f, 1.9f, 1), new Color(1, 0.4f, 0.35f, 0.5f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.85f, 0.35f, 0), new Vector3(0.2f, 0.25f, 1), black, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.85f, 0.35f, 0), new Vector3(0.2f, 0.25f, 1), black, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.85f, 0), new Vector3(1.9f, 0.14f, 1), red, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 2.08f, 0), new Vector3(2.2f, 0.2f, 1), red, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 2.22f, 0), new Vector3(2.4f, 0.08f, 1), black, o + 2);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(-1.1f, 2.08f, 0), new Vector3(0.2f, 0.14f, 1), redDark, o + 1);
        var tr = Mk(p, UiKit.TriangleSprite, pos + new Vector3(1.1f, 2.08f, 0), new Vector3(0.2f, 0.14f, 1), redDark, o + 1);
        tr.transform.rotation = Quaternion.Euler(0, 180f, 0);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.95f, 0), new Vector3(0.3f, 0.22f, 1), black, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.95f, 0), new Vector3(0.2f, 0.12f, 1), new Color(0.85f, 0.7f, 0.25f), o + 2);
    }

    static void PagodaReal(Transform p, Vector3 pos, int o)
    {
        Color wall = new Color(0.88f, 0.78f, 0.6f);
        Color wood = new Color(0.5f, 0.25f, 0.12f);
        Color roofC = new Color(0.28f, 0.22f, 0.3f);
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 1.9f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.08f, 0), new Vector3(1.6f, 0.16f, 1), new Color(0.55f, 0.55f, 0.57f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.22f, 0), new Vector3(1.4f, 0.14f, 1), new Color(0.6f, 0.6f, 0.62f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.6f, 0), new Vector3(1.1f, 0.65f, 1), wall, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.5f, 0.6f, 0), new Vector3(0.1f, 0.65f, 1), wood, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.5f, 0.6f, 0), new Vector3(0.1f, 0.65f, 1), wood, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.6f, 0), new Vector3(0.5f, 0.4f, 1), new Color(1f, 0.85f, 0.55f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.6f, 0), new Vector3(0.5f, 0.04f, 1), wood, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.6f, 0), new Vector3(0.04f, 0.4f, 1), wood, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.02f, 0), new Vector3(1.6f, 0.14f, 1), wood, o + 2);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(0, 1.08f, 0), new Vector3(1.7f, 0.4f, 1), roofC, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.35f, 0), new Vector3(0.8f, 0.35f, 1), wall, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.35f, 0), new Vector3(0.3f, 0.22f, 1), new Color(1f, 0.85f, 0.55f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.58f, 0), new Vector3(1.25f, 0.12f, 1), wood, o + 2);
        Mk(p, UiKit.TriangleSprite, pos + new Vector3(0, 1.63f, 0), new Vector3(1.3f, 0.32f, 1), roofC, o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.9f, 0), new Vector3(0.06f, 0.25f, 1), new Color(0.85f, 0.7f, 0.25f), o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 2.05f, 0), new Vector3(0.1f, 0.1f, 1), new Color(0.85f, 0.7f, 0.25f), o + 2);
    }

    static void CherryReal(Transform p, Vector3 pos, int o)
    {
        Color bark = new Color(0.38f, 0.24f, 0.14f);
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 1.3f, o);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 0.06f, 0), new Vector3(1.1f, 0.18f, 1), new Color(1f, 0.6f, 0.7f, 0.5f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.8f, 0), new Vector3(0.16f, 1.6f, 1), bark, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.03f, 0.8f, 0), new Vector3(0.05f, 1.6f, 1), new Color(bark.r * 0.7f, bark.g * 0.7f, bark.b * 0.7f), o + 2);
        var b1 = Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.3f, 1.5f, 0), new Vector3(0.09f, 0.7f, 1), bark, o + 1);
        b1.transform.rotation = Quaternion.Euler(0, 0, 28f);
        var b2 = Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.3f, 1.5f, 0), new Vector3(0.09f, 0.7f, 1), bark, o + 1);
        b2.transform.rotation = Quaternion.Euler(0, 0, -28f);
        Vector3[] puffs = {
            new Vector3(-0.45f, 1.95f, 0), new Vector3(0.45f, 1.9f, 0), new Vector3(0, 2.25f, 0),
            new Vector3(-0.25f, 1.7f, 0), new Vector3(0.25f, 1.68f, 0), new Vector3(-0.6f, 1.75f, 0),
            new Vector3(0.6f, 1.72f, 0), new Vector3(0, 1.9f, 0)
        };
        Color[] pinks = {
            new Color(1f, 0.62f, 0.72f), new Color(1f, 0.68f, 0.78f), new Color(1f, 0.74f, 0.82f),
            new Color(0.98f, 0.55f, 0.66f), new Color(1f, 0.66f, 0.75f), new Color(0.97f, 0.58f, 0.68f),
            new Color(1f, 0.7f, 0.8f), new Color(1f, 0.72f, 0.8f)
        };
        for (int i = 0; i < puffs.Length; i++)
            Mk(p, UiKit.CircleSprite, pos + puffs[i], new Vector3(0.62f, 0.48f, 1), pinks[i % pinks.Length], o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-0.1f, 2.3f, 0), new Vector3(0.4f, 0.25f, 1), new Color(1, 0.85f, 0.88f, 0.9f), o + 3);
    }

    static void PetalsReal(Transform p, Vector3 center, float spread, int count, int o)
    {
        for (int i = 0; i < count; i++)
        {
            float x = center.x + (Random.Range(-0.5f, 0.5f)) * spread;
            float y = center.y + (Random.Range(-0.5f, 0.5f)) * spread;
            float sz = Random.Range(0.035f, 0.07f);
            var g = Mk(p, UiKit.CircleSprite, new Vector3(x, y, 0), new Vector3(sz, sz * 0.7f, 1),
                new Color(1f, Random.Range(0.55f, 0.75f), Random.Range(0.65f, 0.82f), Random.Range(0.55f, 0.9f)), o);
            g.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180f));
        }
    }

    static void LanternReal(Transform p, Vector3 pos, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 0.5f, o);
        Color stone = new Color(0.55f, 0.55f, 0.57f);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.08f, 0), new Vector3(0.3f, 0.12f, 1), new Color(stone.r * 0.75f, stone.g * 0.75f, stone.b * 0.75f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.3f, 0), new Vector3(0.12f, 0.35f, 1), stone, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.52f, 0), new Vector3(0.3f, 0.08f, 1), stone, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 0.68f, 0), new Vector3(0.55f, 0.55f, 1), new Color(1f, 0.85f, 0.5f, 0.25f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.68f, 0), new Vector3(0.22f, 0.22f, 1), stone, o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 0.68f, 0), new Vector3(0.14f, 0.16f, 1), new Color(1f, 0.88f, 0.55f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.84f, 0), new Vector3(0.3f, 0.08f, 1), stone, o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 0.95f, 0), new Vector3(0.12f, 0.12f, 1), stone, o + 2);
    }

    // ---------- characters ----------
    static void CivilianReal(Transform p, Vector3 pos, Color shirt, int o)
    {
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 0.7f, o);
        Color pants = new Color(0.25f, 0.3f, 0.45f);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.1f, 0.1f, 0), new Vector3(0.13f, 0.06f, 1), new Color(0.2f, 0.15f, 0.1f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.1f, 0.1f, 0), new Vector3(0.13f, 0.06f, 1), new Color(0.2f, 0.15f, 0.1f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.09f, 0.3f, 0), new Vector3(0.13f, 0.4f, 1), pants, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.09f, 0.3f, 0), new Vector3(0.13f, 0.4f, 1), new Color(pants.r * 0.9f, pants.g * 0.9f, pants.b * 0.9f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.72f, 0), new Vector3(0.44f, 0.55f, 1), shirt, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.16f, 0.72f, 0), new Vector3(0.1f, 0.4f, 1), new Color(shirt.r * 0.85f, shirt.g * 0.85f, shirt.b * 0.85f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.2f, 0.5f, 0), new Vector3(0.1f, 0.3f, 1), Skin, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.2f, 0.5f, 0), new Vector3(0.1f, 0.3f, 1), SkinShade, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 1.15f, 0), new Vector3(0.32f, 0.34f, 1), Skin, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.32f, 0), new Vector3(0.34f, 0.14f, 1), new Color(0.15f, 0.12f, 0.1f), o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-0.06f, 1.14f, 0), new Vector3(0.035f, 0.035f, 1), new Color(0.1f, 0.1f, 0.1f), o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.06f, 1.14f, 0), new Vector3(0.035f, 0.035f, 1), new Color(0.1f, 0.1f, 0.1f), o + 2);
    }

    static void SoldierReal(Transform p, Vector3 pos, string region, int o)
    {
        Color uni, helmet;
        bool us = region == "Mindanao";
        bool jp = region == "Visayas";
        if (us) { uni = new Color(0.55f, 0.5f, 0.35f); helmet = new Color(0.42f, 0.42f, 0.28f); }
        else if (jp) { uni = new Color(0.6f, 0.55f, 0.38f); helmet = new Color(0.55f, 0.48f, 0.3f); }
        else { uni = new Color(0.35f, 0.4f, 0.25f); helmet = new Color(0.3f, 0.36f, 0.22f); }
        Shadow(p, pos + new Vector3(0, 0.02f, 0), 0.75f, o);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.11f, 0.08f, 0), new Vector3(0.15f, 0.1f, 1), new Color(0.25f, 0.18f, 0.1f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.11f, 0.08f, 0), new Vector3(0.15f, 0.1f, 1), new Color(0.25f, 0.18f, 0.1f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.1f, 0.32f, 0), new Vector3(0.15f, 0.42f, 1), new Color(uni.r * 0.85f, uni.g * 0.85f, uni.b * 0.85f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.1f, 0.32f, 0), new Vector3(0.15f, 0.42f, 1), uni, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.78f, 0), new Vector3(0.5f, 0.6f, 1), uni, o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.62f, 0), new Vector3(0.5f, 0.08f, 1), new Color(0.3f, 0.22f, 0.12f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.12f, 0.82f, 0), new Vector3(0.12f, 0.14f, 1), new Color(uni.r * 0.8f, uni.g * 0.8f, uni.b * 0.8f), o + 2);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.12f, 0.82f, 0), new Vector3(0.12f, 0.14f, 1), new Color(uni.r * 0.8f, uni.g * 0.8f, uni.b * 0.8f), o + 2);
        if (!jp)
        {
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.85f, 0), new Vector3(0.34f, 0.4f, 1), new Color(uni.r * 0.7f, uni.g * 0.7f, uni.b * 0.7f), o + 2);
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 0.85f, 0), new Vector3(0.34f, 0.05f, 1), new Color(0.25f, 0.25f, 0.2f), o + 2);
        }
        var rifle = Mk(p, UiKit.WhiteSprite, pos + new Vector3(0.3f, 0.75f, 0), new Vector3(0.06f, 0.9f, 1), new Color(0.3f, 0.2f, 0.1f), o);
        rifle.transform.rotation = Quaternion.Euler(0, 0, -18f);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.24f, 0.6f, 0), new Vector3(0.12f, 0.4f, 1), new Color(uni.r * 0.9f, uni.g * 0.9f, uni.b * 0.9f), o + 1);
        Mk(p, UiKit.WhiteSprite, pos + new Vector3(-0.24f, 0.38f, 0), new Vector3(0.1f, 0.12f, 1), Skin, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 1.22f, 0), new Vector3(0.32f, 0.34f, 1), Skin, o + 1);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(-0.06f, 1.21f, 0), new Vector3(0.035f, 0.035f, 1), new Color(0.1f, 0.1f, 0.1f), o + 2);
        Mk(p, UiKit.CircleSprite, pos + new Vector3(0.06f, 1.21f, 0), new Vector3(0.035f, 0.035f, 1), new Color(0.1f, 0.1f, 0.1f), o + 2);
        if (jp)
        {
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.42f, 0), new Vector3(0.34f, 0.14f, 1), helmet, o + 2);
            Mk(p, UiKit.TriangleSprite, pos + new Vector3(-0.14f, 1.62f, 0), new Vector3(0.12f, 0.2f, 1), helmet, o + 2);
            Mk(p, UiKit.TriangleSprite, pos + new Vector3(0.14f, 1.62f, 0), new Vector3(0.12f, 0.2f, 1), helmet, o + 2);
        }
        else if (us)
        {
            Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 1.4f, 0), new Vector3(0.44f, 0.24f, 1), helmet, o + 2);
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.32f, 0), new Vector3(0.48f, 0.06f, 1), new Color(helmet.r * 0.8f, helmet.g * 0.8f, helmet.b * 0.8f), o + 2);
        }
        else
        {
            Mk(p, UiKit.CircleSprite, pos + new Vector3(0, 1.42f, 0), new Vector3(0.42f, 0.22f, 1), helmet, o + 2);
            Mk(p, UiKit.WhiteSprite, pos + new Vector3(0, 1.5f, 0), new Vector3(0.2f, 0.06f, 1), new Color(helmet.r * 0.75f, helmet.g * 0.75f, helmet.b * 0.75f), o + 2);
        }
    }

    static void BuildChars(Transform p, string region)
    {
        CivilianReal(p, new Vector3(-3.5f, -1.85f, 0), P1, 8);
        SoldierReal(p, new Vector3(3.5f, -1.85f, 0), region, 8);
    }

    // ---------- maps ----------
    static void BuildLuzon(Transform p)
    {
        Color top = new Color(0.18f, 0.49f, 0.84f);
        Color mid = new Color(0.49f, 0.76f, 0.93f);
        Color hor = new Color(0.81f, 0.94f, 0.98f);
        SetCam(top);
        SkyGradient(p, top, mid, hor);

        SunReal(p, new Vector3(-7, 4.8f, 0), 0.85f, new Color(1f, 0.96f, 0.75f), new Color(1f, 0.95f, 0.6f), -8);
        RealCloud(p, new Vector3(-5, 4.3f, 0), 2.6f, Color.white, new Color(0.82f, 0.87f, 0.93f), -7);
        RealCloud(p, new Vector3(3.5f, 4.7f, 0), 2.1f, Color.white, new Color(0.82f, 0.87f, 0.93f), -7);
        RealCloud(p, new Vector3(-0.5f, 3.8f, 0), 1.7f, Color.white, new Color(0.84f, 0.88f, 0.94f), -7);
        RealCloud(p, new Vector3(7.2f, 4f, 0), 1.5f, Color.white, new Color(0.84f, 0.88f, 0.94f), -7);
        RealCloud(p, new Vector3(-8.8f, 3.4f, 0), 1.2f, Color.white, new Color(0.85f, 0.89f, 0.94f), -7);
        Bird(p, new Vector3(-3, 5.1f, 0), -6);
        Bird(p, new Vector3(-2.2f, 5.3f, 0), -6);
        Bird(p, new Vector3(5, 4.9f, 0), -6);
        Bird(p, new Vector3(5.8f, 5.1f, 0), -6);

        MtnReal(p, new Vector3(-8, -2.05f, 0), new Vector3(5.5f, 4.2f, 1), new Color(0.55f, 0.65f, 0.72f), new Color(0.45f, 0.55f, 0.64f), new Color(0.2f, 0.38f, 0.25f), Color.white, false, 0);
        MtnReal(p, new Vector3(-3.5f, -2.05f, 0), new Vector3(4.2f, 3.6f, 1), new Color(0.4f, 0.55f, 0.45f), new Color(0.32f, 0.46f, 0.38f), new Color(0.2f, 0.38f, 0.24f), Color.white, false, 0);
        MtnReal(p, new Vector3(0, -2.05f, 0), new Vector3(6.8f, 7.2f, 1), new Color(0.38f, 0.5f, 0.62f), new Color(0.3f, 0.42f, 0.55f), new Color(0.22f, 0.4f, 0.26f), new Color(0.94f, 0.96f, 1f), true, 1);
        MtnReal(p, new Vector3(6.5f, -2.05f, 0), new Vector3(5.2f, 4.4f, 1), new Color(0.35f, 0.53f, 0.38f), new Color(0.28f, 0.45f, 0.31f), new Color(0.2f, 0.38f, 0.24f), Color.white, false, 0);
        MtnReal(p, new Vector3(9.8f, -2.05f, 0), new Vector3(3.4f, 3.2f, 1), new Color(0.42f, 0.57f, 0.46f), new Color(0.34f, 0.49f, 0.38f), new Color(0.22f, 0.4f, 0.26f), Color.white, false, 0);

        HillReal(p, new Vector3(-4.2f, -1.1f, 0), new Vector3(3.6f, 2.8f, 1), new Color(0.32f, 0.56f, 0.28f), new Color(0.25f, 0.47f, 0.22f), 2);
        HillReal(p, new Vector3(-1.4f, -1.05f, 0), new Vector3(2.4f, 1.9f, 1), new Color(0.35f, 0.58f, 0.3f), new Color(0.27f, 0.49f, 0.24f), 2);
        HillReal(p, new Vector3(4.6f, -1.05f, 0), new Vector3(3f, 2.2f, 1), new Color(0.33f, 0.56f, 0.29f), new Color(0.26f, 0.47f, 0.23f), 2);
        for (float i = 0; i < 3; i++)
            Mk(p, UiKit.WhiteSprite, new Vector3(-5.2f + i * 0.5f, -0.5f + i * 0.28f, 0), new Vector3(1.6f - i * 0.3f, 0.1f, 1), new Color(0.45f, 0.68f, 0.35f), 2);

        WaterReal(p, new Color(0.12f, 0.4f, 0.55f), new Color(0.3f, 0.62f, 0.78f), Color.white, 0f, false, 3);
        Lily(p, new Vector3(-1.2f, -2.7f, 0), 4);
        Lily(p, new Vector3(1.3f, -2.9f, 0), 4);
        GroundReal(p, new Color(0.3f, 0.6f, 0.22f), new Color(0.55f, 0.36f, 0.2f), new Color(0.42f, 0.26f, 0.14f), new Color(0.45f, 0.28f, 0.15f), 4);
        BridgeReal(p, new Color(0.6f, 0.4f, 0.2f), new Color(0.45f, 0.28f, 0.13f), 5);

        Reed(p, new Vector3(-2.2f, -1.9f, 0), new Color(0.3f, 0.55f, 0.25f), 6);
        Reed(p, new Vector3(2.25f, -1.9f, 0), new Color(0.3f, 0.55f, 0.25f), 6);
        RockReal(p, new Vector3(-1.7f, -2f, 0), 0.22f, 5);
        RockReal(p, new Vector3(1.8f, -2.05f, 0), 0.18f, 5);
        BushReal(p, new Vector3(-2.9f, -1.85f, 0), new Color(0.25f, 0.52f, 0.22f), 6);
        BushReal(p, new Vector3(2.9f, -1.85f, 0), new Color(0.25f, 0.52f, 0.22f), 6);

        NipaHutReal(p, new Vector3(-7.2f, -1.85f, 0), 6);
        FenceReal(p, new Vector3(-8.8f, -1.85f, 0), 0.9f, new Color(0.55f, 0.4f, 0.2f), 6);
        PalmReal(p, new Vector3(-9.6f, -1.85f, 0), new Color(0.5f, 0.32f, 0.16f), new Color(0.18f, 0.55f, 0.2f), new Color(0.13f, 0.44f, 0.16f), 6, true);
        PalmReal(p, new Vector3(-5.3f, -1.85f, 0), new Color(0.48f, 0.3f, 0.15f), new Color(0.2f, 0.53f, 0.2f), new Color(0.14f, 0.43f, 0.16f), 6, true);
        BushReal(p, new Vector3(-6.2f, -1.85f, 0), new Color(0.28f, 0.55f, 0.24f), 6);
        PHFlagReal(p, new Vector3(7.3f, -1.85f, 0), 6);
        MonumentReal(p, new Vector3(9f, -1.85f, 0), new Color(0.6f, 0.6f, 0.62f), new Color(0.85f, 0.7f, 0.18f), 6);
        BushReal(p, new Vector3(8.1f, -1.85f, 0), new Color(0.28f, 0.55f, 0.24f), 6);

        Color[] fl = { new Color(1f, 0.82f, 0.2f), new Color(1f, 0.42f, 0.5f), new Color(0.65f, 0.35f, 0.9f), Color.white };
        for (float x = -9f; x <= -2.4f; x += 0.85f) FlowerReal(p, new Vector3(x, -1.8f, 0), fl[Mathf.Abs((int)(x * 10)) % fl.Length], 7);
        for (float x = 2.4f; x <= 9.5f; x += 0.85f) FlowerReal(p, new Vector3(x, -1.8f, 0), fl[Mathf.Abs((int)(x * 10)) % fl.Length], 7);
    }

    static void BuildMindanao(Transform p)
    {
        Color top = new Color(0.24f, 0.22f, 0.4f);
        Color mid = new Color(0.83f, 0.45f, 0.35f);
        Color hor = new Color(1f, 0.78f, 0.5f);
        SetCam(top);
        SkyGradient(p, top, mid, hor);

        SunReal(p, new Vector3(-0.5f, 2.1f, 0), 1.15f, new Color(1f, 0.82f, 0.45f), new Color(1f, 0.6f, 0.3f), -8);
        RealCloud(p, new Vector3(-6, 4.2f, 0), 2.8f, new Color(0.55f, 0.42f, 0.52f), new Color(0.95f, 0.6f, 0.42f), -7);
        RealCloud(p, new Vector3(2.5f, 4.6f, 0), 2.2f, new Color(0.55f, 0.42f, 0.52f), new Color(0.95f, 0.62f, 0.45f), -7);
        RealCloud(p, new Vector3(6.5f, 3.6f, 0), 1.8f, new Color(0.6f, 0.45f, 0.55f), new Color(0.95f, 0.65f, 0.45f), -7);
        RealCloud(p, new Vector3(-2f, 3.3f, 0), 1.5f, new Color(0.6f, 0.46f, 0.55f), new Color(0.96f, 0.62f, 0.45f), -7);
        Bird(p, new Vector3(-4, 5f, 0), -6);
        Bird(p, new Vector3(4.5f, 4.7f, 0), -6);

        MtnReal(p, new Vector3(-5.5f, -2.05f, 0), new Vector3(5f, 3.8f, 1), new Color(0.38f, 0.28f, 0.38f), new Color(0.3f, 0.22f, 0.32f), new Color(0.25f, 0.3f, 0.28f), Color.white, false, 0);
        MtnReal(p, new Vector3(7f, -2.05f, 0), new Vector3(4.6f, 3.4f, 1), new Color(0.4f, 0.3f, 0.38f), new Color(0.32f, 0.23f, 0.31f), new Color(0.27f, 0.32f, 0.28f), Color.white, false, 0);
        MtnReal(p, new Vector3(2.5f, -2.05f, 0), new Vector3(3.5f, 2.8f, 1), new Color(0.36f, 0.27f, 0.36f), new Color(0.29f, 0.21f, 0.3f), new Color(0.24f, 0.29f, 0.27f), Color.white, false, 0);
        Mk(p, UiKit.WhiteSprite, new Vector3(0, 0.2f, 0), new Vector3(24f, 1.2f, 1), new Color(1f, 0.6f, 0.35f, 0.18f), 1);
        HillReal(p, new Vector3(-3.2f, -1f, 0), new Vector3(3f, 2.4f, 1), new Color(0.4f, 0.42f, 0.3f), new Color(0.32f, 0.34f, 0.24f), 2);
        HillReal(p, new Vector3(5.2f, -1.05f, 0), new Vector3(2.6f, 2f, 1), new Color(0.42f, 0.43f, 0.31f), new Color(0.33f, 0.35f, 0.25f), 2);

        WaterReal(p, new Color(0.2f, 0.24f, 0.45f), new Color(0.75f, 0.45f, 0.4f), new Color(1f, 0.62f, 0.3f), -0.5f, true, 3);
        GroundReal(p, new Color(0.55f, 0.5f, 0.32f), new Color(0.78f, 0.66f, 0.45f), new Color(0.6f, 0.48f, 0.32f), new Color(0.62f, 0.5f, 0.33f), 4);

        DockReal(p, new Vector3(-2.6f, -1.85f, 0), 5);
        Mk(p, UiKit.WhiteSprite, new Vector3(-0.4f, -1.85f, 0), new Vector3(1.4f, 0.16f, 1), new Color(0.62f, 0.43f, 0.2f), 5);
        CrateReal(p, new Vector3(-4.4f, -1.85f, 0), 0.34f, 6);
        BarrelReal(p, new Vector3(-3.9f, -1.85f, 0), 6);
        BarrelReal(p, new Vector3(-3.55f, -1.85f, 0), 6);
        BoatReal(p, new Vector3(-0.9f, -2.5f, 0), 4);

        WoodHouseReal(p, new Vector3(5.6f, -1.85f, 0), 6);
        FenceReal(p, new Vector3(4f, -1.85f, 0), 1.3f, new Color(0.5f, 0.35f, 0.18f), 6);
        USFlagReal(p, new Vector3(3.2f, -1.85f, 0), 6);
        PalmReal(p, new Vector3(-8.2f, -1.85f, 0), new Color(0.4f, 0.24f, 0.12f), new Color(0.22f, 0.4f, 0.2f), new Color(0.16f, 0.32f, 0.16f), 6, true);
        PalmReal(p, new Vector3(8.6f, -1.85f, 0), new Color(0.4f, 0.24f, 0.12f), new Color(0.23f, 0.4f, 0.2f), new Color(0.17f, 0.32f, 0.16f), 6, true);
        PalmReal(p, new Vector3(7.1f, -1.85f, 0), new Color(0.42f, 0.25f, 0.13f), new Color(0.22f, 0.39f, 0.2f), new Color(0.16f, 0.31f, 0.16f), 6, false);
        MonumentReal(p, new Vector3(7.9f, -1.85f, 0), new Color(0.58f, 0.56f, 0.55f), new Color(0.9f, 0.75f, 0.25f), 6);
        BushReal(p, new Vector3(-6f, -1.85f, 0), new Color(0.3f, 0.42f, 0.22f), 6);
        BushReal(p, new Vector3(2.6f, -1.85f, 0), new Color(0.3f, 0.42f, 0.22f), 6);
    }

    static void BuildVisayas(Transform p)
    {
        Color top = new Color(0.35f, 0.27f, 0.47f);
        Color mid = new Color(0.88f, 0.5f, 0.55f);
        Color hor = new Color(1f, 0.79f, 0.62f);
        SetCam(top);
        SkyGradient(p, top, mid, hor);

        SunReal(p, new Vector3(0.5f, 2.2f, 0), 1.05f, new Color(1f, 0.8f, 0.5f), new Color(1f, 0.6f, 0.4f), -8);
        RealCloud(p, new Vector3(-5, 4.3f, 0), 2.5f, new Color(0.98f, 0.8f, 0.72f), new Color(0.85f, 0.5f, 0.55f), -7);
        RealCloud(p, new Vector3(3, 4.6f, 0), 2f, new Color(0.98f, 0.82f, 0.75f), new Color(0.86f, 0.52f, 0.56f), -7);
        RealCloud(p, new Vector3(7, 3.7f, 0), 1.7f, new Color(0.97f, 0.8f, 0.72f), new Color(0.85f, 0.52f, 0.55f), -7);
        RealCloud(p, new Vector3(-1, 3.4f, 0), 1.5f, new Color(0.97f, 0.8f, 0.72f), new Color(0.86f, 0.53f, 0.56f), -7);
        Bird(p, new Vector3(-3, 5.1f, 0), -6);
        Bird(p, new Vector3(5, 4.9f, 0), -6);

        MtnReal(p, new Vector3(0, -2.05f, 0), new Vector3(7f, 5.8f, 1), new Color(0.5f, 0.4f, 0.55f), new Color(0.4f, 0.32f, 0.47f), new Color(0.3f, 0.38f, 0.34f), Color.white, false, 0);
        MtnReal(p, new Vector3(-6.5f, -2.05f, 0), new Vector3(4.5f, 3.8f, 1), new Color(0.42f, 0.42f, 0.42f), new Color(0.34f, 0.34f, 0.35f), new Color(0.28f, 0.4f, 0.3f), Color.white, false, 0);
        MtnReal(p, new Vector3(7.5f, -2.05f, 0), new Vector3(4f, 3.3f, 1), new Color(0.44f, 0.42f, 0.43f), new Color(0.36f, 0.34f, 0.36f), new Color(0.29f, 0.4f, 0.3f), Color.white, false, 0);
        Mk(p, UiKit.WhiteSprite, new Vector3(0, 0.3f, 0), new Vector3(24f, 1.1f, 1), new Color(1f, 0.62f, 0.5f, 0.16f), 1);
        HillReal(p, new Vector3(-4f, -1f, 0), new Vector3(3f, 2.4f, 1), new Color(0.38f, 0.5f, 0.34f), new Color(0.3f, 0.42f, 0.27f), 2);
        HillReal(p, new Vector3(4.5f, -1.05f, 0), new Vector3(2.5f, 1.9f, 1), new Color(0.4f, 0.52f, 0.35f), new Color(0.31f, 0.43f, 0.28f), 2);

        WaterReal(p, new Color(0.25f, 0.3f, 0.52f), new Color(0.85f, 0.55f, 0.55f), new Color(1f, 0.65f, 0.45f), 0.5f, true, 3);
        GroundReal(p, new Color(0.32f, 0.55f, 0.26f), new Color(0.55f, 0.36f, 0.2f), new Color(0.42f, 0.26f, 0.14f), new Color(0.45f, 0.28f, 0.15f), 4);
        BridgeReal(p, new Color(0.6f, 0.4f, 0.2f), new Color(0.45f, 0.28f, 0.13f), 5);

        PagodaReal(p, new Vector3(-6.5f, -1.85f, 0), 6);
        ToriiReal(p, new Vector3(6.5f, -1.85f, 0), 6);
        JPFlagReal(p, new Vector3(4.4f, -1.85f, 0), 6);
        CherryReal(p, new Vector3(-8.8f, -1.85f, 0), 6);
        CherryReal(p, new Vector3(-4.3f, -1.85f, 0), 6);
        CherryReal(p, new Vector3(8.8f, -1.85f, 0), 6);
        CherryReal(p, new Vector3(2.9f, -1.85f, 0), 6);
        LanternReal(p, new Vector3(5.4f, -1.85f, 0), 6);
        LanternReal(p, new Vector3(-5.2f, -1.85f, 0), 6);
        for (float x = 4.8f; x <= 8.2f; x += 0.55f)
            Mk(p, UiKit.CircleSprite, new Vector3(x, -1.78f, 0), new Vector3(0.3f, 0.12f, 1), new Color(0.6f, 0.6f, 0.62f), 6);
        PetalsReal(p, new Vector3(0, 0.8f, 0), 11f, 45, 7);
        MonumentReal(p, new Vector3(8f, -1.85f, 0), new Color(0.55f, 0.55f, 0.57f), new Color(0.88f, 0.72f, 0.2f), 6);
        RockReal(p, new Vector3(-1.8f, -2f, 0), 0.2f, 5);
        RockReal(p, new Vector3(1.7f, -2.05f, 0), 0.17f, 5);
        BushReal(p, new Vector3(-2.8f, -1.85f, 0), new Color(0.28f, 0.48f, 0.26f), 6);
        Color[] fl = { new Color(1f, 0.55f, 0.7f), new Color(1f, 0.65f, 0.78f), Color.white };
        for (float x = -9f; x <= -2.4f; x += 0.9f) FlowerReal(p, new Vector3(x, -1.8f, 0), fl[Mathf.Abs((int)(x * 10)) % fl.Length], 7);
        for (float x = 2.4f; x <= 4.2f; x += 0.9f) FlowerReal(p, new Vector3(x, -1.8f, 0), fl[Mathf.Abs((int)(x * 10)) % fl.Length], 7);
    }

    static void BuildHud()
    {
        var canvas = UiKit.CreateCanvas("GameHUD", 100, new Vector2(1920, 1080));
        var hud = UiKit.CreateScreen(canvas.transform, "HUD");

        var ban = new GameObject("Ban", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        var banRt = ban.GetComponent<RectTransform>();
        banRt.SetParent(hud, false);
        banRt.anchorMin = new Vector2(0, 1);
        banRt.anchorMax = new Vector2(0, 1);
        banRt.pivot = new Vector2(0, 1);
        banRt.anchoredPosition = new Vector2(20, -15);
        banRt.sizeDelta = new Vector2(700, 65);
        ban.GetComponent<Image>().color = new Color(0, 0, 0, 0.55f);

        var t = UiKit.CreateText(hud, GameState.Region + " - Level " + GameState.Level, 42, Color.white, TextAnchor.MiddleLeft, 660, 55);
        var tRt = t.rectTransform;
        tRt.anchorMin = new Vector2(0, 1);
        tRt.anchorMax = new Vector2(0, 1);
        tRt.pivot = new Vector2(0, 1);
        tRt.anchoredPosition = new Vector2(40, -20);

        var tl = UiKit.CreateText(hud, "Player 1's Turn", 38, new Color(0.2f, 0.5f, 1f), TextAnchor.MiddleCenter, 480, 50);
        var tlRt = tl.rectTransform;
        tlRt.anchorMin = new Vector2(0.5f, 0);
        tlRt.anchorMax = new Vector2(0.5f, 0);
        tlRt.pivot = new Vector2(0.5f, 0);
        tlRt.anchoredPosition = new Vector2(0, 20);

        var tc = new GameObject("TC");
        var cycle = tc.AddComponent<TurnCycle>();
        cycle.SetLabel(tl);

        var eb = UiKit.CreateButton(hud, "End Turn", new Vector2(0, 80), new Vector2(280, 85), new Color(0.9f, 0.6f, 0.12f), () => cycle.EndTurn());
        var ebRt = eb.GetComponent<RectTransform>();
        ebRt.anchorMin = new Vector2(0.5f, 0);
        ebRt.anchorMax = new Vector2(0.5f, 0);
        ebRt.pivot = new Vector2(0.5f, 0);
        ebRt.anchoredPosition = new Vector2(0, 80);

        UiKit.CreateButton(hud, "Main Menu", new Vector2(-810, 455), new Vector2(260, 70), new Color(0.33f, 0.34f, 0.4f), () => SceneManager.LoadScene(MainMenuSceneName));
    }
}
