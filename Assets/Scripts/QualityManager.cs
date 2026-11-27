using UnityEngine;

namespace Futsal27.Core
{
    /// <summary>Aplica os perfis gráficos baixo / médio / alto.</summary>
    public static class QualityManager
    {
        public static void Apply(int level)
        {
            switch (level)
            {
                case 0: Set(30, ShadowQuality.Disable, 0, 0.6f); break;
                case 1: Set(30, ShadowQuality.HardOnly, 0, 0.8f); break;
                default: Set(60, ShadowQuality.All, 2, 1f); break;
            }
        }

        static void Set(int fps, ShadowQuality shadows, int aa, float scale)
        {
            Application.targetFrameRate = fps;
            QualitySettings.vSyncCount = 0;
            QualitySettings.shadows = shadows;
            QualitySettings.antiAliasing = aa;

            if (Application.platform == RuntimePlatform.Android)
            {
                int sw = Display.main.systemWidth, sh = Display.main.systemHeight;
                int w = Mathf.Max(sw, sh), h = Mathf.Min(sw, sh);
                Screen.SetResolution(Mathf.RoundToInt(w * scale), Mathf.RoundToInt(h * scale), true);
            }
        }
    }
}
