using UnityEngine;

namespace Futsal27.UI
{
    /// <summary>Gera sprites proceduralmente (sem depender de arquivos de imagem).</summary>
    public static class SpriteFactory
    {
        static Sprite _white, _circle, _round;

        public static Sprite White => _white ? _white : (_white = MakeSolid());
        public static Sprite Circle => _circle ? _circle : (_circle = MakeCircle(128));
        public static Sprite Round => _round ? _round : (_round = MakeRound(64, 24));

        static Sprite MakeSolid()
        {
            var t = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            var px = new Color[16];
            for (int i = 0; i < 16; i++) px[i] = Color.white;
            t.SetPixels(px);
            t.Apply();
            return Sprite.Create(t, new Rect(0, 0, 4, 4), new Vector2(.5f, .5f));
        }

        static Sprite MakeCircle(int size)
        {
            var t = NewTex(size, size);
            float r = size / 2f;
            var px = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + .5f, y + .5f), new Vector2(r, r));
                    px[y * size + x] = new Color(1, 1, 1, Mathf.Clamp01(r - d));
                }
            t.SetPixels(px);
            t.Apply();
            return Sprite.Create(t, new Rect(0, 0, size, size), new Vector2(.5f, .5f));
        }

        static Sprite MakeRound(int size, int radius)
        {
            var t = NewTex(size, size);
            var px = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float cx = Mathf.Clamp(x + .5f, radius, size - radius);
                    float cy = Mathf.Clamp(y + .5f, radius, size - radius);
                    float d = Vector2.Distance(new Vector2(x + .5f, y + .5f), new Vector2(cx, cy));
                    px[y * size + x] = new Color(1, 1, 1, Mathf.Clamp01(radius - d + .5f));
                }
            t.SetPixels(px);
            t.Apply();
            return Sprite.Create(t, new Rect(0, 0, size, size), new Vector2(.5f, .5f), 100f, 0,
                                 SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        }

        public static Sprite VGradient(Color top, Color bottom)
        {
            const int h = 128;
            var t = NewTex(4, h);
            var px = new Color[4 * h];
            for (int y = 0; y < h; y++)
            {
                var c = Color.Lerp(bottom, top, y / (h - 1f));
                for (int x = 0; x < 4; x++) px[y * 4 + x] = c;
            }
            t.SetPixels(px);
            t.Apply();
            return Sprite.Create(t, new Rect(0, 0, 4, h), new Vector2(.5f, .5f));
        }

        static Texture2D NewTex(int w, int h)
        {
            return new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
        }
    }
}
