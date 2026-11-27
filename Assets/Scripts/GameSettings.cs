using UnityEngine;

namespace Futsal27.Core
{
    /// <summary>Configurações do jogador salvas localmente (PlayerPrefs).</summary>
    public static class GameSettings
    {
        public static int Quality = 1;        // 0 = baixa, 1 = média, 2 = alta
        public static int MatchMinutes = 20;  // minutos por tempo

        public static void Load()
        {
            Quality = Mathf.Clamp(PlayerPrefs.GetInt("quality", 1), 0, 2);
            MatchMinutes = PlayerPrefs.GetInt("minutes", 20);
        }

        public static void Save()
        {
            PlayerPrefs.SetInt("quality", Quality);
            PlayerPrefs.SetInt("minutes", MatchMinutes);
            PlayerPrefs.Save();
        }
    }
}
