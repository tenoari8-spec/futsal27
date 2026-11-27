using UnityEngine;

namespace Futsal27.UI
{
    /// <summary>Balanço lento usado nas faixas diagonais do fundo.</summary>
    public class Sway : MonoBehaviour
    {
        public Vector2 amp = new Vector2(40, 14);
        public float speed = 0.25f;
        public float phase;
        RectTransform r;
        Vector2 start;

        void Start() { r = (RectTransform)transform; start = r.anchoredPosition; }

        void Update()
        {
            float t = Time.unscaledTime * speed + phase;
            r.anchoredPosition = start + new Vector2(Mathf.Sin(t) * amp.x, Mathf.Cos(t * .8f) * amp.y);
        }
    }

    /// <summary>Animação de entrada (deslizar + fade) sempre que a tela é exibida.</summary>
    public class SlideIn : MonoBehaviour
    {
        public float delay;
        public float duration = 0.4f;
        public Vector2 offset = new Vector2(-120, 0);
        RectTransform r;
        CanvasGroup g;
        Vector2 target;
        bool init;
        float t;

        void Init()
        {
            if (init) return;
            init = true;
            r = (RectTransform)transform;
            g = gameObject.GetComponent<CanvasGroup>();
            if (g == null) g = gameObject.AddComponent<CanvasGroup>();
            target = r.anchoredPosition;
        }

        void OnEnable() { Init(); t = -delay; Apply(0); }

        void Update()
        {
            if (t >= duration) return;
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            k = 1f - (1f - k) * (1f - k) * (1f - k);
            Apply(k);
        }

        void Apply(float k)
        {
            r.anchoredPosition = target + offset * (1f - k);
            g.alpha = k;
        }

        public static SlideIn Add(Component c, float delay, Vector2 offset)
        {
            var s = c.gameObject.AddComponent<SlideIn>();
            s.delay = delay;
            s.offset = offset;
            return s;
        }
    }
}
