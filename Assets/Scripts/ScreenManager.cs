using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Futsal27.UI;

namespace Futsal27.Core
{
    /// <summary>Controla as telas do menu com transições suaves (fade).</summary>
    public class ScreenManager : MonoBehaviour
    {
        class Entry { public CanvasGroup group; public Action onShow; public string back; }

        readonly Dictionary<string, Entry> screens = new Dictionary<string, Entry>();
        string current;
        Coroutine routine;

        public RectTransform NewScreen(string key, Transform parent, Action onShow = null, string back = null)
        {
            var r = UIKit.Rect("Screen_" + key, parent);
            UIKit.Fill(r);
            var g = r.gameObject.AddComponent<CanvasGroup>();
            g.alpha = 0; g.interactable = false; g.blocksRaycasts = false;
            r.gameObject.SetActive(false);
            screens[key] = new Entry { group = g, onShow = onShow, back = back };
            return r;
        }

        public void Show(string key)
        {
            if (!screens.ContainsKey(key) || key == current) return;
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(Switch(key));
        }

        public void GoBack()
        {
            if (current != null && screens[current].back != null) Show(screens[current].back);
        }

        IEnumerator Switch(string key)
        {
            string prev = current;
            current = key;

            foreach (var kv in screens)
            {
                if (kv.Key == prev || kv.Key == key) continue;
                kv.Value.group.interactable = false;
                kv.Value.group.blocksRaycasts = false;
                kv.Value.group.gameObject.SetActive(false);
            }

            if (prev != null)
            {
                var p = screens[prev].group;
                p.interactable = false; p.blocksRaycasts = false;
                yield return Fade(p, p.alpha, 0f, 0.15f);
                p.gameObject.SetActive(false);
            }

            var e = screens[key];
            e.onShow?.Invoke();
            e.group.gameObject.SetActive(true);
            e.group.interactable = true;
            e.group.blocksRaycasts = true;
            yield return Fade(e.group, 0f, 1f, 0.22f);
        }

        static IEnumerator Fade(CanvasGroup g, float a, float b, float d)
        {
            float t = 0;
            while (t < d)
            {
                t += Time.unscaledDeltaTime;
                g.alpha = Mathf.Lerp(a, b, t / d);
                yield return null;
            }
            g.alpha = b;
        }
    }
}
