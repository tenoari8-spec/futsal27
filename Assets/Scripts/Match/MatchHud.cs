using UnityEngine;
using UnityEngine.UI;
using Futsal27.Core;
using Futsal27.UI;

namespace Futsal27.Match
{
    /// <summary>Interface da partida: placar de TV, relógio, aviso de gol e botões de teste.</summary>
    public class MatchHud
    {
        readonly GameObject canvasGo;
        readonly Text score, clock, goalText;

        public MatchHud(MatchRoot owner)
        {
            canvasGo = new GameObject("MatchHud", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var sc = canvasGo.GetComponent<CanvasScaler>();
            sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            sc.referenceResolution = new Vector2(1920, 1080);
            sc.matchWidthOrHeight = .5f;
            var root = (RectTransform)canvasGo.transform;

            var h = GameSession.Home;
            var a = GameSession.Away;

            // Placar
            var panel = UIKit.Img("Scoreboard", root, SpriteFactory.Round, new Color(0, 0, 0, .6f), true);
            UIKit.Anchor(panel.rectTransform, .28f, .90f, .72f, .985f);
            var chipH = UIKit.Img("ChipHome", panel.transform, SpriteFactory.White, GameSession.HomeShirt);
            UIKit.Anchor(chipH.rectTransform, .015f, .2f, .03f, .8f);
            var chipA = UIKit.Img("ChipAway", panel.transform, SpriteFactory.White, GameSession.AwayShirt);
            UIKit.Anchor(chipA.rectTransform, .97f, .2f, .985f, .8f);
            UIKit.LabelAt(panel.transform, h.shortName, 40, Color.white, TextAnchor.MiddleRight, .04f, 0, .36f, 1);
            UIKit.LabelAt(panel.transform, a.shortName, 40, Color.white, TextAnchor.MiddleLeft, .64f, 0, .96f, 1);
            score = UIKit.LabelAt(panel.transform, "0 - 0", 56, UIKit.Accent, TextAnchor.MiddleCenter, .36f, 0, .64f, 1);
            clock = UIKit.LabelAt(root, "", 28, UIKit.TextDim, TextAnchor.MiddleCenter, .40f, .85f, .60f, .90f);

            goalText = UIKit.LabelAt(root, "", 120, UIKit.Accent, TextAnchor.MiddleCenter, .1f, .38f, .9f, .72f);
            goalText.supportRichText = true;
            goalText.gameObject.SetActive(false);

            // Botões
            UIKit.Nav(root, "SAIR", .02f, .90f, .13f, .98f, false, owner.Exit);
            UIKit.Nav(root, "CÂMERA", .86f, .90f, .98f, .98f, false, owner.CycleCamera);
            UIKit.Nav(root, "REINICIAR", .02f, .04f, .17f, .13f, false, owner.ResetPositions);
            UIKit.Nav(root, "RASTEIRO", .64f, .04f, .80f, .13f, true, () => owner.TestKick(false));
            UIKit.Nav(root, "ALTO", .82f, .04f, .98f, .13f, true, () => owner.TestKick(true));
        }

        public void SetScore(int h, int a) { score.text = h + " - " + a; }

        public void SetClock(float seconds)
        {
            int m = (int)(seconds / 60f), s = (int)(seconds % 60f);
            clock.text = "1º T  " + m.ToString("00") + ":" + s.ToString("00");
        }

        public void ShowGoal(string club)
        {
            goalText.text = "GOOOL!\n<size=48><color=#FFFFFF>" + club + "</color></size>";
            goalText.gameObject.SetActive(true);
        }

        public void HideGoal() { goalText.gameObject.SetActive(false); }

        public void Destroy() { Object.Destroy(canvasGo); }
    }
}
