using UnityEngine;
using UnityEngine.EventSystems;
using Futsal27.Data;
using Futsal27.UI;

namespace Futsal27.Core
{
    /// <summary>Ponto de entrada: único objeto da cena. Monta todo o menu por código.</summary>
    public class Bootstrap : MonoBehaviour
    {
        void Awake()
        {
            GameSettings.Load();
            QualityManager.Apply(GameSettings.Quality);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            var cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = UIKit.BgBottom;
            }

            if (FindObjectOfType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            ClubRepository.Load();
            gameObject.AddComponent<MenuRoot>().Build();
        }
    }
}
