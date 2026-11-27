using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Futsal27.Core;
using Futsal27.Data;

namespace Futsal27.UI
{
    /// <summary>Menu principal: lista de modos à esquerda, painel de destaque à direita.</summary>
    public class MainMenuScreen
    {
        class Entry
        {
            public string title, desc, target;
            public int stage;
            public Entry(string t, string d, string target, int stage)
            { title = t; desc = d; this.target = target; this.stage = stage; }
        }

        readonly ScreenManager sm;
        readonly List<Entry> entries = new List<Entry>();
        readonly List<Text> labels = new List<Text>();
        readonly List<Image> underlines = new List<Image>();
        Text heroTitle, heroStatus, heroDesc, playLabel;
        Button play;
        int selected = -1;

        public MainMenuScreen(ScreenManager sm, Transform root)
        {
            this.sm = sm;

            entries.Add(new Entry("Partida Rápida", "Escolha dois clubes, defina os uniformes e entre em quadra.", "teams", 0));
            entries.Add(new Entry("Campeonato Brasileiro", "Temporada completa com rodadas, tabela e classificação.", "", 8));
            entries.Add(new Entry("Copa do Brasil", "Mata-mata com jogos de ida e volta e disputa de pênaltis.", "", 8));
            entries.Add(new Entry("Liga Personalizada", "Monte sua própria liga com os clubes que quiser.", "", 8));
            entries.Add(new Entry("Torneio Internacional", "Competição entre clubes e seleções de vários países.", "", 8));
            entries.Add(new Entry("Carreira de Treinador", "Comande um clube: elenco, tática e contratações.", "", 8));
            entries.Add(new Entry("Carreira de Jogador", "Crie um atleta e construa sua trajetória no futsal.", "", 8));
            entries.Add(new Entry("Treinamento", "Pratique passes, chutes e jogadas ensaiadas sem adversário.", "", 3));
            entries.Add(new Entry("Editor de Clubes",
                "Já dá para editar agora pelo arquivo JSON:\n" + ClubRepository.FilePath +
                "\n\nEscudos em PNG na pasta:\n" + ClubRepository.CrestDir +
                "\n\nO editor visual chega na Etapa 7.", "", 7));
            entries.Add(new Entry("Configurações", "Qualidade gráfica (baixa, média, alta) e duração da partida.", "settings", 0));

            var screen = sm.NewScreen("main", root, () => Select(selected < 0 ? 0 : selected));

            // Título
            var title = UIKit.LabelAt(screen, "FUTSAL <color=" + UIKit.Hex(UIKit.Accent) + ">27</color>", 88,
                                      Color.white, TextAnchor.MiddleLeft, .05f, .88f, .40f, .98f);
            title.supportRichText = true;
            UIKit.LabelAt(screen, "Simulador de Futsal Brasileiro", 26, UIKit.TextDim, TextAnchor.MiddleLeft,
                          .05f, .845f, .40f, .885f, FontStyle.Normal);

            // Lista de modos
            float top = .82f, pitch = .075f, h = .065f;
            for (int i = 0; i < entries.Count; i++)
            {
                int idx = i;
                float y1 = top - i * pitch, y0 = y1 - h;

                var item = UIKit.Img("Item" + i, screen, SpriteFactory.Round, new Color(1, 1, 1, .04f), true, true);
                UIKit.Anchor(item.rectTransform, .05f, y0, .34f, y1);
                var btn = item.gameObject.AddComponent<Button>();
                btn.targetGraphic = item;
                btn.transition = Selectable.Transition.None;
                btn.onClick.AddListener(() => Select(idx));

                var lb = UIKit.LabelAt(item.transform, entries[i].title, 34, UIKit.TextDim, TextAnchor.MiddleLeft,
                                       .06f, 0, 1, 1);
                labels.Add(lb);

                var ul = UIKit.Img("Underline", item.transform, SpriteFactory.White, UIKit.Accent);
                UIKit.Anchor(ul.rectTransform, .04f, 0, .96f, 0);
                ul.rectTransform.offsetMax = new Vector2(0, 4);
                underlines.Add(ul);

                SlideIn.Add(item, i * 0.045f, new Vector2(-140, 0));
            }

            // Painel de destaque
            var hero = UIKit.Img("Hero", screen, SpriteFactory.Round, UIKit.Glass, true);
            UIKit.Anchor(hero.rectTransform, .42f, .10f, .95f, .90f);
            heroTitle = UIKit.LabelAt(hero.transform, "", 64, Color.white, TextAnchor.MiddleLeft, .06f, .72f, .94f, .92f);
            heroStatus = UIKit.LabelAt(hero.transform, "", 28, UIKit.Accent, TextAnchor.MiddleLeft, .06f, .64f, .94f, .72f);
            heroDesc = UIKit.LabelAt(hero.transform, "", 32, UIKit.TextDim, TextAnchor.UpperLeft, .06f, .22f, .94f, .62f,
                                     FontStyle.Normal);
            play = UIKit.Btn(hero.transform, "JOGAR", 38, UIKit.Accent, UIKit.Dark, OnPlay);
            UIKit.Anchor((RectTransform)play.transform, .06f, .06f, .50f, .18f);
            playLabel = play.GetComponentInChildren<Text>();
            SlideIn.Add(hero, 0.15f, new Vector2(140, 0));
        }

        void OnPlay()
        {
            if (selected < 0) return;
            var t = entries[selected].target;
            if (!string.IsNullOrEmpty(t)) sm.Show(t);
        }

        void Select(int i)
        {
            selected = i;
            for (int k = 0; k < entries.Count; k++)
            {
                bool on = k == i;
                labels[k].color = on ? Color.white : UIKit.TextDim;
                underlines[k].enabled = on;
            }

            var e = entries[i];
            bool avail = !string.IsNullOrEmpty(e.target);
            heroTitle.text = e.title;
            heroStatus.text = avail ? "Disponível" : "Em breve · Etapa " + e.stage;
            heroStatus.color = avail ? UIKit.Accent : UIKit.Warn;
            heroDesc.text = e.desc;
            play.interactable = avail;
            playLabel.text = !avail ? "EM BREVE" : (e.target == "settings" ? "ABRIR" : "JOGAR");
        }
    }
}
