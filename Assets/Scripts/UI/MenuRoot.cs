using UnityEngine;
using UnityEngine.UI;
using Futsal27.Core;
using Futsal27.Match;

namespace Futsal27.UI
{
    /// <summary>Monta o Canvas, o fundo animado e todas as telas do menu.</summary>
    public class MenuRoot : MonoBehaviour
    {
        public static MenuRoot Instance { get; private set; }

        ScreenManager sm;
        GameObject canvasGo;

        void Awake() { Instance = this; }

        public void SetVisible(bool visible)
        {
            if (canvasGo != null) canvasGo.SetActive(visible);
        }

        public void Build()
        {
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo = go;
            go.transform.SetParent(transform, false);
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            var sc = go.GetComponent<CanvasScaler>();
            sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            sc.referenceResolution = new Vector2(1920, 1080);
            sc.matchWidthOrHeight = 0.5f;

            var root = (RectTransform)go.transform;
            BuildBackground(root);

            sm = gameObject.AddComponent<ScreenManager>();
            new MainMenuScreen(sm, root);
            new TeamSelectScreen(sm, root);
            new KitSelectScreen(sm, root);
            new SettingsScreen(sm, root);
            sm.Show("main");
        }

        void Update()
        {
            // Botão "voltar" do Android
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (MatchRoot.Active) MatchRoot.Instance.Exit();
            else if (sm != null) sm.GoBack();
        }

        static void BuildBackground(RectTransform root)
        {
            var bg = UIKit.Img("Background", root, SpriteFactory.VGradient(UIKit.BgTop, UIKit.BgBottom), Color.white);
            UIKit.Fill(bg.rectTransform);

            for (int i = 0; i < 4; i++)
            {
                float a = i % 2 == 0 ? .10f : .05f;
                var s = UIKit.Img("Stripe" + i, root, SpriteFactory.White,
                                  new Color(UIKit.Accent.r, UIKit.Accent.g, UIKit.Accent.b, a));
                var r = s.rectTransform;
                r.anchorMin = r.anchorMax = new Vector2(.5f, .5f);
                r.sizeDelta = new Vector2(3400, i % 2 == 0 ? 150 : 60);
                r.localRotation = Quaternion.Euler(0, 0, -24);
                r.anchoredPosition = new Vector2(i * 140 - 150, i * 190 - 300);
                var sw = s.gameObject.AddComponent<Sway>();
                sw.phase = i * 1.3f;
            }
        }
    }
}
