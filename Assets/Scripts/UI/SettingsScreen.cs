using UnityEngine;
using UnityEngine.UI;
using Futsal27.Core;

namespace Futsal27.UI
{
    /// <summary>Configurações: qualidade gráfica e duração da partida.</summary>
    public class SettingsScreen
    {
        static readonly string[] QualityNames = { "BAIXA", "MÉDIA", "ALTA" };
        static readonly int[] Minutes = { 5, 10, 15, 20 };

        readonly Button[] quality = new Button[3];
        readonly Button[] minutes = new Button[4];

        public SettingsScreen(ScreenManager sm, Transform root)
        {
            var screen = sm.NewScreen("settings", root, Refresh, "main");

            var panel = UIKit.Img("Panel", screen, SpriteFactory.Round, UIKit.Glass, true);
            UIKit.Anchor(panel.rectTransform, .15f, .16f, .85f, .88f);

            UIKit.LabelAt(panel.transform, "CONFIGURAÇÕES", 44, Color.white, TextAnchor.MiddleLeft, .05f, .84f, .95f, .96f);

            UIKit.LabelAt(panel.transform, "Qualidade gráfica", 32, UIKit.TextDim, TextAnchor.MiddleLeft,
                          .05f, .72f, .95f, .82f, FontStyle.Normal);
            for (int i = 0; i < 3; i++)
            {
                int q = i;
                quality[i] = UIKit.Btn(panel.transform, QualityNames[i], 32, UIKit.GlassStrong, Color.white, () =>
                {
                    GameSettings.Quality = q;
                    GameSettings.Save();
                    QualityManager.Apply(q);
                    Refresh();
                });
                UIKit.Anchor((RectTransform)quality[i].transform, .05f + i * .31f, .56f, .05f + i * .31f + .28f, .70f);
            }
            UIKit.LabelAt(panel.transform,
                "Baixa: 30 fps, sem sombras, resolução reduzida (ideal para aparelhos de entrada).",
                24, UIKit.TextDim, TextAnchor.MiddleLeft, .05f, .46f, .95f, .54f, FontStyle.Normal);

            UIKit.LabelAt(panel.transform, "Duração de cada tempo (minutos)", 32, UIKit.TextDim, TextAnchor.MiddleLeft,
                          .05f, .34f, .95f, .44f, FontStyle.Normal);
            for (int i = 0; i < Minutes.Length; i++)
            {
                int m = Minutes[i];
                minutes[i] = UIKit.Btn(panel.transform, m.ToString(), 32, UIKit.GlassStrong, Color.white, () =>
                {
                    GameSettings.MatchMinutes = m;
                    GameSettings.Save();
                    Refresh();
                });
                UIKit.Anchor((RectTransform)minutes[i].transform, .05f + i * .235f, .18f, .05f + i * .235f + .21f, .32f);
            }

            UIKit.Nav(screen, "VOLTAR", .15f, .04f, .31f, .12f, false, () => sm.Show("main"));
        }

        void Refresh()
        {
            for (int i = 0; i < quality.Length; i++) UIKit.SetSelected(quality[i], GameSettings.Quality == i);
            for (int i = 0; i < minutes.Length; i++) UIKit.SetSelected(minutes[i], GameSettings.MatchMinutes == Minutes[i]);
        }
    }
}
