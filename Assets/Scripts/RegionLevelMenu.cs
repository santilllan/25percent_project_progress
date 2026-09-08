using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class RegionLevelMenu
{
    static Canvas overlay;
    static RectTransform currentScreen;
    static string selectedRegion;
    static bool loading;

    static readonly Color Gold = new Color(0.95f, 0.75f, 0.3f);
    static readonly Color Body = new Color(0.13f, 0.15f, 0.22f);
    static readonly Color Dim = new Color(0.65f, 0.68f, 0.75f);
    static readonly Color LuzonColor = new Color(0.95f, 0.62f, 0.18f);
    static readonly Color MindanaoColor = new Color(0.3f, 0.58f, 0.95f);
    static readonly Color VisayasColor = new Color(0.92f, 0.38f, 0.5f);
    static readonly Color EasyColor = new Color(0.3f, 0.7f, 0.35f);
    static readonly Color NormalColor = new Color(0.3f, 0.55f, 0.9f);
    static readonly Color HardColor = new Color(0.88f, 0.3f, 0.28f);
    static readonly Color BackColor = new Color(0.35f, 0.36f, 0.42f);

    public static void Open()
    {
        loading = false;
        if (overlay == null)
        {
            overlay = UiKit.CreateCanvas("MenuOverlay", 100, new Vector2(1920f, 1080f));
            var bg = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
            var bgRt = bg.rectTransform;
            bgRt.SetParent(overlay.transform, false);
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;
            bg.color = new Color(0.03f, 0.04f, 0.08f, 0.96f);
            var bar = new GameObject("TopBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
            var barRt = bar.rectTransform;
            barRt.SetParent(overlay.transform, false);
            barRt.anchorMin = new Vector2(0, 1);
            barRt.anchorMax = new Vector2(1, 1);
            barRt.pivot = new Vector2(0.5f, 1);
            barRt.anchoredPosition = new Vector2(0, 0);
            barRt.sizeDelta = new Vector2(0, 8);
            bar.color = Gold;
        }
        ShowRegionScreen();
    }

    static Button CreateCard(Transform parent, Vector2 pos, Vector2 size, Color accent, string title, string subtitle, System.Action onClick)
    {
        var shadow = new GameObject("CardShadow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        var shRt = shadow.rectTransform;
        shRt.SetParent(parent, false);
        shRt.anchorMin = new Vector2(0.5f, 0.5f);
        shRt.anchorMax = new Vector2(0.5f, 0.5f);
        shRt.pivot = new Vector2(0.5f, 0.5f);
        shRt.anchoredPosition = pos + new Vector2(0, -10);
        shRt.sizeDelta = size;
        shadow.color = new Color(0, 0, 0, 0.55f);

        var btn = UiKit.CreateButton(parent, "", pos, size, Body, onClick);

        var strip = new GameObject("Accent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        var stRt = strip.rectTransform;
        stRt.SetParent(btn.transform, false);
        stRt.anchorMin = new Vector2(0, 1);
        stRt.anchorMax = new Vector2(1, 1);
        stRt.pivot = new Vector2(0.5f, 1);
        stRt.anchoredPosition = Vector2.zero;
        stRt.sizeDelta = new Vector2(0, 12);
        strip.color = accent;
        strip.raycastTarget = false;

        var dot = new GameObject("Dot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        var dotRt = dot.rectTransform;
        dotRt.SetParent(btn.transform, false);
        dotRt.anchorMin = new Vector2(0.5f, 0.5f);
        dotRt.anchorMax = new Vector2(0.5f, 0.5f);
        dotRt.pivot = new Vector2(0.5f, 0.5f);
        dotRt.anchoredPosition = new Vector2(0, 62);
        dotRt.sizeDelta = new Vector2(54, 54);
        dot.color = accent;
        dot.raycastTarget = false;

        var t = UiKit.CreateText(btn.transform, title, 40, Color.white, TextAnchor.MiddleCenter, size.x - 40f, 80f);
        t.rectTransform.anchoredPosition = new Vector2(0, -8);
        t.raycastTarget = false;

        var s = UiKit.CreateText(btn.transform, subtitle, 23, Dim, TextAnchor.MiddleCenter, size.x - 40f, 70f);
        s.rectTransform.anchoredPosition = new Vector2(0, -78);
        s.raycastTarget = false;

        return btn;
    }

    static void Header(RectTransform screen, string over, string title, string sub)
    {
        var o = UiKit.CreateText(screen, over, 28, Gold, TextAnchor.MiddleCenter, 1200f, 50f);
        o.rectTransform.anchoredPosition = new Vector2(0, 430f);
        var t = UiKit.CreateText(screen, title, 76, Color.white, TextAnchor.MiddleCenter, 1400f, 110f);
        t.rectTransform.anchoredPosition = new Vector2(0, 350f);
        var u = new GameObject("Underline", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        var uRt = u.rectTransform;
        uRt.SetParent(screen, false);
        uRt.anchorMin = new Vector2(0.5f, 0.5f);
        uRt.anchorMax = new Vector2(0.5f, 0.5f);
        uRt.pivot = new Vector2(0.5f, 0.5f);
        uRt.anchoredPosition = new Vector2(0, 290f);
        uRt.sizeDelta = new Vector2(220, 5);
        u.color = Gold;
        var s = UiKit.CreateText(screen, sub, 26, Dim, TextAnchor.MiddleCenter, 1400f, 50f);
        s.rectTransform.anchoredPosition = new Vector2(0, 250f);
    }

    static void ShowRegionScreen()
    {
        if (loading) return;
        ClearScreen();
        currentScreen = UiKit.CreateScreen(overlay.transform, "RegionScreen");
        Header(currentScreen, "PHILIPPINE CAMPAIGN", "Choose a Region", "Three fronts. Three eras. Pick your battlefield.");
        CreateCard(currentScreen, new Vector2(-560f, -20f), new Vector2(500f, 300f), LuzonColor,
            "1. LUZON", "Nipa huts and volcano\nBright noon", () => ChooseRegion("Luzon"));
        CreateCard(currentScreen, new Vector2(0f, -20f), new Vector2(500f, 300f), MindanaoColor,
            "2. MINDANAO", "American era\nSunset docks", () => ChooseRegion("Mindanao"));
        CreateCard(currentScreen, new Vector2(560f, -20f), new Vector2(500f, 300f), VisayasColor,
            "3. VISAYAS", "Japanese era\nSakura dusk", () => ChooseRegion("Visayas"));
        var hint = UiKit.CreateText(currentScreen, "Each region holds 3 levels", 22, Dim, TextAnchor.MiddleCenter, 900f, 40f);
        hint.rectTransform.anchoredPosition = new Vector2(0f, -240f);
        UiKit.CreateButton(currentScreen, "Back", new Vector2(0f, -380f), new Vector2(260f, 85f), BackColor, Close);
    }

    static void ShowLevelScreen()
    {
        if (loading) return;
        ClearScreen();
        currentScreen = UiKit.CreateScreen(overlay.transform, "LevelScreen");
        Header(currentScreen, selectedRegion.ToUpper(), "Select Level", "Pick a mission to deploy.");
        CreateCard(currentScreen, new Vector2(-560f, -20f), new Vector2(500f, 300f), EasyColor,
            "LEVEL 1", "Easy\nSkirmish", () => StartLevel(1));
        CreateCard(currentScreen, new Vector2(0f, -20f), new Vector2(500f, 300f), NormalColor,
            "LEVEL 2", "Normal\nAssault", () => StartLevel(2));
        CreateCard(currentScreen, new Vector2(560f, -20f), new Vector2(500f, 300f), HardColor,
            "LEVEL 3", "Hard\nSiege", () => StartLevel(3));
        var hint = UiKit.CreateText(currentScreen, "Harder levels, same battlefield", 22, Dim, TextAnchor.MiddleCenter, 900f, 40f);
        hint.rectTransform.anchoredPosition = new Vector2(0f, -240f);
        UiKit.CreateButton(currentScreen, "Back", new Vector2(0f, -380f), new Vector2(260f, 85f), BackColor, ShowRegionScreen);
    }

    static void ChooseRegion(string region)
    {
        if (loading) return;
        selectedRegion = region;
        ShowLevelScreen();
    }

    static void StartLevel(int levelNumber)
    {
        if (loading) return;
        loading = true;
        GameState.Region = selectedRegion;
        GameState.Level = levelNumber;
        SceneManager.LoadScene(GameMapBuilder.GameSceneName);
    }

    static void ClearScreen()
    {
        if (currentScreen != null)
            UnityEngine.Object.Destroy(currentScreen.gameObject);
        currentScreen = null;
    }

    static void Close()
    {
        if (overlay != null)
            UnityEngine.Object.Destroy(overlay.gameObject);
        overlay = null;
        currentScreen = null;
    }
}
