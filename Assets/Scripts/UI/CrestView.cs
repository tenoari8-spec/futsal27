using UnityEngine;
using UnityEngine.UI;
using Futsal27.Data;

namespace Futsal27.UI
{
    /// <summary>Escudo do clube: usa o PNG do usuário ou um escudo genérico com a sigla.</summary>
    public class CrestView
    {
        public readonly RectTransform Root;
        readonly Image bg, logo;
        readonly Text initials;

        public CrestView(Transform parent, int fontSize)
        {
            Root = UIKit.Rect("Crest", parent);
            bg = UIKit.Img("Bg", Root, SpriteFactory.Circle, Color.white);
            UIKit.Fill(bg.rectTransform);
            logo = UIKit.Img("Logo", Root, null, Color.white);
            UIKit.Anchor(logo.rectTransform, .08f, .08f, .92f, .92f);
            logo.preserveAspect = true;
            initials = UIKit.Label(Root, "", fontSize, Color.white);
        }

        public void Set(ClubData c)
        {
            var s = CrestLoader.Get(c);
            if (s != null)
            {
                logo.gameObject.SetActive(true);
                logo.sprite = s;
                bg.color = new Color(1, 1, 1, .12f);
                initials.text = "";
            }
            else
            {
                logo.gameObject.SetActive(false);
                bg.color = c.HomeColor;
                initials.text = c.shortName;
                initials.color = UIKit.Contrast(c.HomeColor);
            }
        }
    }
}
