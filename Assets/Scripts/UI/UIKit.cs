using System;
using UnityEngine;
using UnityEngine.UI;

namespace Futsal27.UI
{
    /// <summary>Helpers para montar interface uGUI por código.</summary>
    public static class UIKit
    {
        public static readonly Color Accent = new Color32(0, 230, 118, 255);
        public static readonly Color Dark = new Color32(6, 32, 22, 255);
        public static readonly Color BgTop = new Color32(11, 59, 46, 255);
        public static readonly Color BgBottom = new Color32(3, 16, 12, 255);
        public static readonly Color Glass = new Color(1, 1, 1, 0.08f);
        public static readonly Color GlassStrong = new Color(1, 1, 1, 0.16f);
        public static readonly Color TextDim = new Color(1, 1, 1, 0.6f);
        public static readonly Color Warn = new Color32(255, 199, 51, 255);

        static Font _font;
        public static Font Font
        {
            get
            {
                if (_font == null)
                {
                    _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    if (_font == null) _font = Font.CreateDynamicFontFromOSFont(new[] { "Roboto", "Arial" }, 32);
                }
                return _font;
            }
        }

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var r = go.GetComponent<RectTransform>();
            r.SetParent(parent, false);
            return r;
        }

        public static RectTransform Anchor(RectTransform r, float x0, float y0, float x1, float y1)
        {
            r.anchorMin = new Vector2(x0, y0);
            r.anchorMax = new Vector2(x1, y1);
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
            return r;
        }

        public static RectTransform Fill(RectTransform r) { return Anchor(r, 0, 0, 1, 1); }

        public static Image Img(string name, Transform parent, Sprite s, Color c, bool sliced = false, bool raycast = false)
        {
            var r = Rect(name, parent);
            var i = r.gameObject.AddComponent<Image>();
            i.sprite = s;
            i.color = c;
            i.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
            i.raycastTarget = raycast;
            return i;
        }

        public static Text Label(Transform parent, string text, int size, Color c,
                                 TextAnchor a = TextAnchor.MiddleCenter, FontStyle st = FontStyle.Bold)
        {
            var r = Rect("Text", parent);
            Fill(r);
            var t = r.gameObject.AddComponent<Text>();
            t.font = Font;
            t.text = text;
            t.fontSize = size;
            t.color = c;
            t.alignment = a;
            t.fontStyle = st;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }

        public static Text LabelAt(Transform parent, string text, int size, Color c, TextAnchor a,
                                   float x0, float y0, float x1, float y1, FontStyle st = FontStyle.Bold)
        {
            var t = Label(parent, text, size, c, a, st);
            Anchor(t.rectTransform, x0, y0, x1, y1);
            return t;
        }

        public static Button Btn(Transform parent, string label, int size, Color bg, Color fg, Action click)
        {
            var img = Img("Btn_" + label, parent, SpriteFactory.Round, bg, true, true);
            var b = img.gameObject.AddComponent<Button>();
            b.targetGraphic = img;
            var cb = b.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(.92f, .92f, .92f);
            cb.pressedColor = new Color(.72f, .72f, .72f);
            cb.disabledColor = new Color(1, 1, 1, .4f);
            b.colors = cb;
            Label(img.transform, label, size, fg);
            if (click != null) b.onClick.AddListener(() => click());
            return b;
        }

        public static Button Nav(Transform parent, string label, float x0, float y0, float x1, float y1,
                                 bool primary, Action click)
        {
            var b = Btn(parent, label, 34, primary ? Accent : GlassStrong, primary ? Dark : Color.white, click);
            Anchor((RectTransform)b.transform, x0, y0, x1, y1);
            return b;
        }

        public static void SetSelected(Button b, bool selected)
        {
            ((Image)b.targetGraphic).color = selected ? Accent : GlassStrong;
            var t = b.GetComponentInChildren<Text>();
            if (t != null) t.color = selected ? Dark : Color.white;
        }

        public static Color Contrast(Color c)
        {
            float l = .299f * c.r + .587f * c.g + .114f * c.b;
            return l > .6f ? (Color)new Color32(10, 25, 20, 255) : Color.white;
        }

        public static float Distance(Color a, Color b)
        {
            float dr = a.r - b.r, dg = a.g - b.g, db = a.b - b.b;
            return Mathf.Sqrt(dr * dr + dg * dg + db * db);
        }

        public static string Hex(Color c) { return "#" + ColorUtility.ToHtmlStringRGB(c); }
    }
}
