using UnityEngine;
using UnityEngine.UI;
using Futsal27.Core;
using Futsal27.Data;

namespace Futsal27.UI
{
    /// <summary>Seleção de uniformes (principal ou reserva) com pré-visualização da camisa.</summary>
    public class KitSelectScreen
    {
        class Jersey
        {
            readonly Image body, sleeveL, sleeveR, collar;
            readonly Text number;

            public Jersey(Transform parent)
            {
                var root = UIKit.Rect("Jersey", parent);
                root.anchorMin = root.anchorMax = new Vector2(.5f, .62f);
                root.sizeDelta = new Vector2(300, 320);
                root.anchoredPosition = Vector2.zero;

                sleeveL = UIKit.Img("SleeveL", root, SpriteFactory.Round, Color.white, true);
                UIKit.Anchor(sleeveL.rectTransform, 0f, .46f, .30f, .90f);
                sleeveL.rectTransform.localRotation = Quaternion.Euler(0, 0, 28);

                sleeveR = UIKit.Img("SleeveR", root, SpriteFactory.Round, Color.white, true);
                UIKit.Anchor(sleeveR.rectTransform, .70f, .46f, 1f, .90f);
                sleeveR.rectTransform.localRotation = Quaternion.Euler(0, 0, -28);

                body = UIKit.Img("Body", root, SpriteFactory.Round, Color.white, true);
                UIKit.Anchor(body.rectTransform, .17f, 0f, .83f, .93f);

                collar = UIKit.Img("Collar", root, SpriteFactory.Circle, Color.white);
                collar.rectTransform.anchorMin = collar.rectTransform.anchorMax = new Vector2(.5f, .93f);
                collar.rectTransform.sizeDelta = new Vector2(84, 84);
                collar.rectTransform.anchoredPosition = Vector2.zero;

                number = UIKit.LabelAt(root, "10", 110, Color.white, TextAnchor.MiddleCenter, .17f, .15f, .83f, .75f);
            }

            public void Set(Color shirt, Color trim)
            {
                body.color = shirt;
                sleeveL.color = shirt;
                sleeveR.color = shirt;
                collar.color = trim;
                number.color = UIKit.Contrast(shirt);
            }
        }

        class Side
        {
            public Jersey jersey;
            public Text name;
            public Button main, alt;
        }

        readonly ScreenManager sm;
        Side home, away;
        Text warning;

        public KitSelectScreen(ScreenManager sm, Transform root)
        {
            this.sm = sm;
            var screen = sm.NewScreen("kits", root, OnShow, "teams");

            var header = UIKit.LabelAt(screen,
                "PARTIDA RÁPIDA   <color=" + UIKit.Hex(UIKit.Accent) + ">Selecionar uniformes</color>", 40,
                Color.white, TextAnchor.MiddleLeft, .06f, .88f, .70f, .97f);
            header.supportRichText = true;

            home = BuildSide(screen, .06f, "CASA", () => { GameSession.HomeUsesAwayKit = false; Refresh(); },
                                                    () => { GameSession.HomeUsesAwayKit = true; Refresh(); });
            away = BuildSide(screen, .54f, "VISITANTE", () => { GameSession.AwayUsesAwayKit = false; Refresh(); },
                                                        () => { GameSession.AwayUsesAwayKit = true; Refresh(); });

            warning = UIKit.LabelAt(screen, "", 30, UIKit.Warn, TextAnchor.MiddleCenter, .25f, .155f, .75f, .20f);

            UIKit.Nav(screen, "VOLTAR", .06f, .05f, .22f, .14f, false, () => sm.Show("teams"));
            UIKit.Nav(screen, "INICIAR PARTIDA", .66f, .05f, .94f, .14f, true, StartMatch);
        }

        Side BuildSide(Transform parent, float x0, string label, System.Action pickMain, System.Action pickAlt)
        {
            var s = new Side();
            var panel = UIKit.Img("Kit_" + label, parent, SpriteFactory.Round, UIKit.Glass, true);
            UIKit.Anchor(panel.rectTransform, x0, .22f, x0 + .40f, .84f);

            UIKit.LabelAt(panel.transform, label, 28, UIKit.Accent, TextAnchor.MiddleCenter, 0, .90f, 1, .98f);
            s.jersey = new Jersey(panel.transform);
            s.name = UIKit.LabelAt(panel.transform, "", 40, Color.white, TextAnchor.MiddleCenter, .05f, .22f, .95f, .32f);

            s.main = UIKit.Btn(panel.transform, "PRINCIPAL", 30, UIKit.GlassStrong, Color.white, pickMain);
            UIKit.Anchor((RectTransform)s.main.transform, .06f, .05f, .48f, .17f);
            s.alt = UIKit.Btn(panel.transform, "RESERVA", 30, UIKit.GlassStrong, Color.white, pickAlt);
            UIKit.Anchor((RectTransform)s.alt.transform, .52f, .05f, .94f, .17f);
            return s;
        }

        void OnShow()
        {
            if (GameSession.Home == null || GameSession.Away == null) return;
            GameSession.HomeUsesAwayKit = false;
            GameSession.AwayUsesAwayKit = false;
            // Se as cores forem parecidas, o visitante já entra com o uniforme reserva.
            if (UIKit.Distance(GameSession.Home.HomeColor, GameSession.Away.HomeColor) < .35f)
                GameSession.AwayUsesAwayKit = true;
            Refresh();
        }

        void Refresh()
        {
            var h = GameSession.Home;
            var a = GameSession.Away;
            if (h == null || a == null) return;

            home.name.text = h.name;
            away.name.text = a.name;
            home.jersey.Set(GameSession.HomeShirt, h.AccentColor);
            away.jersey.Set(GameSession.AwayShirt, a.AccentColor);

            UIKit.SetSelected(home.main, !GameSession.HomeUsesAwayKit);
            UIKit.SetSelected(home.alt, GameSession.HomeUsesAwayKit);
            UIKit.SetSelected(away.main, !GameSession.AwayUsesAwayKit);
            UIKit.SetSelected(away.alt, GameSession.AwayUsesAwayKit);

            bool clash = UIKit.Distance(GameSession.HomeShirt, GameSession.AwayShirt) < .35f;
            warning.text = clash ? "Cores muito parecidas — troque um dos uniformes." : "";
        }

        void StartMatch()
        {
            Futsal27.Match.MatchRoot.Launch();
        }
    }
}
