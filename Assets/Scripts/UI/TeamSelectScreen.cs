using UnityEngine;
using UnityEngine.UI;
using Futsal27.Core;
using Futsal27.Data;

namespace Futsal27.UI
{
    /// <summary>Partida Rápida: seleção de times (casa x visitante).</summary>
    public class TeamSelectScreen
    {
        class Card
        {
            public CrestView crest;
            public Text name, city;
            public Text[] stats = new Text[3];
            public Image[] stars = new Image[5];
        }

        readonly ScreenManager sm;
        Card home, away;
        Text duration;
        int homeIdx = 0, awayIdx = 1;

        public TeamSelectScreen(ScreenManager sm, Transform root)
        {
            this.sm = sm;
            var screen = sm.NewScreen("teams", root, Refresh, "main");

            var header = UIKit.LabelAt(screen,
                "PARTIDA RÁPIDA   <color=" + UIKit.Hex(UIKit.Accent) + ">Selecionar times</color>", 40,
                Color.white, TextAnchor.MiddleLeft, .06f, .88f, .70f, .97f);
            header.supportRichText = true;

            home = BuildCard(screen, .06f, "CASA", true);
            away = BuildCard(screen, .54f, "VISITANTE", false);

            UIKit.LabelAt(screen, "VS", 70, Color.white, TextAnchor.MiddleCenter, .44f, .46f, .56f, .58f);
            duration = UIKit.LabelAt(screen, "", 26, UIKit.TextDim, TextAnchor.MiddleCenter, .42f, .40f, .58f, .46f,
                                     FontStyle.Normal);

            UIKit.Nav(screen, "VOLTAR", .06f, .05f, .22f, .14f, false, () => sm.Show("main"));
            UIKit.Nav(screen, "SORTEAR", .40f, .05f, .60f, .14f, false, Randomize);
            UIKit.Nav(screen, "AVANÇAR", .78f, .05f, .94f, .14f, true, Next);
        }

        Card BuildCard(Transform parent, float x0, string side, bool isHome)
        {
            var c = new Card();
            var panel = UIKit.Img("Card_" + side, parent, SpriteFactory.Round, UIKit.Glass, true);
            UIKit.Anchor(panel.rectTransform, x0, .20f, x0 + .40f, .84f);

            UIKit.LabelAt(panel.transform, side, 28, UIKit.Accent, TextAnchor.MiddleCenter, 0, .90f, 1, .98f);

            c.crest = new CrestView(panel.transform, 64);
            c.crest.Root.anchorMin = c.crest.Root.anchorMax = new Vector2(.5f, .66f);
            c.crest.Root.sizeDelta = new Vector2(260, 260);
            c.crest.Root.anchoredPosition = Vector2.zero;

            var prev = UIKit.Btn(panel.transform, "<", 52, UIKit.GlassStrong, Color.white, () => Step(isHome, -1));
            UIKit.Anchor((RectTransform)prev.transform, .03f, .56f, .14f, .70f);
            var next = UIKit.Btn(panel.transform, ">", 52, UIKit.GlassStrong, Color.white, () => Step(isHome, 1));
            UIKit.Anchor((RectTransform)next.transform, .86f, .56f, .97f, .70f);

            c.name = UIKit.LabelAt(panel.transform, "", 44, Color.white, TextAnchor.MiddleCenter, .05f, .31f, .95f, .43f);
            c.city = UIKit.LabelAt(panel.transform, "", 26, UIKit.TextDim, TextAnchor.MiddleCenter, .05f, .25f, .95f, .31f,
                                   FontStyle.Normal);

            for (int k = 0; k < 5; k++)
            {
                var star = UIKit.Img("Star" + k, panel.transform, SpriteFactory.Circle, UIKit.Accent);
                var r = star.rectTransform;
                r.anchorMin = r.anchorMax = new Vector2(.5f, .20f);
                r.sizeDelta = new Vector2(30, 30);
                r.anchoredPosition = new Vector2((k - 2) * 44, 0);
                c.stars[k] = star;
            }

            string[] names = { "ATQ", "MEI", "DEF" };
            for (int k = 0; k < 3; k++)
            {
                var box = UIKit.Img("Stat" + k, panel.transform, SpriteFactory.Round, UIKit.Glass, true);
                UIKit.Anchor(box.rectTransform, .07f + k * .31f, .03f, .07f + k * .31f + .26f, .14f);
                var t = UIKit.Label(box.transform, names[k], 30, Color.white);
                t.supportRichText = true;
                c.stats[k] = t;
            }
            return c;
        }

        void Step(bool isHome, int dir)
        {
            int n = ClubRepository.Clubs.Count;
            int cur = isHome ? homeIdx : awayIdx;
            int other = isHome ? awayIdx : homeIdx;
            do { cur = (cur + dir + n) % n; } while (cur == other);
            if (isHome) homeIdx = cur; else awayIdx = cur;
            Refresh();
        }

        void Randomize()
        {
            int n = ClubRepository.Clubs.Count;
            homeIdx = Random.Range(0, n);
            do { awayIdx = Random.Range(0, n); } while (awayIdx == homeIdx);
            Refresh();
        }

        void Next()
        {
            GameSession.Home = ClubRepository.Clubs[homeIdx];
            GameSession.Away = ClubRepository.Clubs[awayIdx];
            sm.Show("kits");
        }

        void Refresh()
        {
            int n = ClubRepository.Clubs.Count;
            homeIdx = Mathf.Clamp(homeIdx, 0, n - 1);
            if (awayIdx == homeIdx || awayIdx >= n) awayIdx = (homeIdx + 1) % n;

            Fill(home, ClubRepository.Clubs[homeIdx]);
            Fill(away, ClubRepository.Clubs[awayIdx]);
            duration.text = "2 x " + GameSettings.MatchMinutes + " min";
        }

        static void Fill(Card c, ClubData d)
        {
            c.crest.Set(d);
            c.name.text = d.name;
            c.city.text = d.city;
            string acc = UIKit.Hex(UIKit.Accent);
            c.stats[0].text = "ATQ <color=" + acc + ">" + d.attack + "</color>";
            c.stats[1].text = "MEI <color=" + acc + ">" + d.midfield + "</color>";
            c.stats[2].text = "DEF <color=" + acc + ">" + d.defense + "</color>";
            int stars = Mathf.Clamp(Mathf.RoundToInt(d.Overall / 20f), 1, 5);
            for (int k = 0; k < 5; k++)
                c.stars[k].color = k < stars ? UIKit.Accent : new Color(1, 1, 1, .2f);
        }
    }
}
