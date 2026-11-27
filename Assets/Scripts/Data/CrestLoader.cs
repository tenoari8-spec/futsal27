using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Futsal27.Data
{
    /// <summary>Carrega escudos PNG inseridos pelo usuário na pasta "Escudos".</summary>
    public static class CrestLoader
    {
        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        public static Sprite Get(ClubData c)
        {
            if (c == null || string.IsNullOrEmpty(c.crestFile)) return null;
            if (cache.TryGetValue(c.id, out var cached)) return cached;

            Sprite s = null;
            var path = Path.Combine(ClubRepository.CrestDir, c.crestFile);
            if (File.Exists(path))
            {
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (tex.LoadImage(File.ReadAllBytes(path)))
                    s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
            }
            cache[c.id] = s;
            return s;
        }
    }
}
