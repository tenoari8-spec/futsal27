using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Futsal27.Core;
using Futsal27.Data;
using Futsal27.UI;

namespace Futsal27.Match
{
    /// <summary>
    /// Orquestra a partida 3D: monta quadra, bola, 10 jogadores, câmera e HUD.
    /// Etapa 2: ainda sem controle dos jogadores; há botões de chute para testar a física.
    /// </summary>
    public class MatchRoot : MonoBehaviour
    {
        public static MatchRoot Instance { get; private set; }
        public static bool Active { get { return Instance != null; } }

        // Losango 1-2-1 (time da casa ataca para +X). O visitante é espelhado.
        static readonly Vector2[] Formation =
        {
            new Vector2(-18f, 0f), new Vector2(-10f, 0f), new Vector2(-5f, 5f), new Vector2(-5f, -5f), new Vector2(-2f, 0f)
        };
        static readonly int[] Numbers = { 1, 4, 7, 10, 9 };
        static readonly Vector3 Kickoff = new Vector3(0, CourtSpec.BallRadius + .01f, 0);

        GameObject world;
        BallController ball;
        MatchCamera cam;
        MatchHud hud;
        readonly List<PlayerAvatar> players = new List<PlayerAvatar>();
        int homeScore, awayScore;
        float clock;
        bool paused;

        // Estado da câmera do menu (restaurado ao sair)
        Vector3 camPos; Quaternion camRot; float camFov; float camNear, camFar;
        CameraClearFlags camFlags; Color camBg;

        public static void Launch()
        {
            if (Instance != null) return;
            new GameObject("MatchRoot").AddComponent<MatchRoot>();
        }

        void Awake()
        {
            Instance = this;
            Begin();
        }

        void Begin()
        {
            if (GameSession.Home == null || GameSession.Away == null)
            {
                GameSession.Home = ClubRepository.Clubs[0];
                GameSession.Away = ClubRepository.Clubs[1];
            }
            if (MenuRoot.Instance != null) MenuRoot.Instance.SetVisible(false);

            var camera = Camera.main;
            if (camera == null)
            {
                var cg = new GameObject("Main Camera", typeof(Camera));
                cg.tag = "MainCamera";
                camera = cg.GetComponent<Camera>();
            }
            camPos = camera.transform.position; camRot = camera.transform.rotation; camFov = camera.fieldOfView;
            camNear = camera.nearClipPlane; camFar = camera.farClipPlane;
            camFlags = camera.clearFlags; camBg = camera.backgroundColor;

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(8, 12, 20, 255);
            camera.orthographic = false;
            camera.nearClipPlane = .5f;
            camera.farClipPlane = 150f;

            var light = FindObjectOfType<Light>();
            if (light == null || light.type != LightType.Directional)
            {
                light = new GameObject("Sun").AddComponent<Light>();
                light.type = LightType.Directional;
            }
            light.transform.rotation = Quaternion.Euler(55, -35, 0);
            light.intensity = 1.15f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = .7f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.5f, .52f, .58f);

            world = new GameObject("MatchWorld");
            CourtBuilder.Build(world.transform, OnGoal);
            ball = BallController.Create(world.transform);
            ball.ResetTo(Kickoff);
            SpawnPlayers();

            cam = camera.gameObject.AddComponent<MatchCamera>();
            cam.target = ball.transform;
            cam.SnapNow();

            hud = new MatchHud(this);
            hud.SetScore(0, 0);
            hud.SetClock(0);
        }

        void SpawnPlayers()
        {
            var home = GameSession.Home;
            var away = GameSession.Away;
            var gkHome = new Color32(255, 212, 0, 255);
            var gkAway = new Color32(255, 106, 0, 255);

            for (int i = 0; i < Formation.Length; i++)
            {
                var f = Formation[i];
                bool gk = i == 0;
                players.Add(PlayerAvatar.Create(world.transform, "Home" + i,
                    gk ? (Color)gkHome : GameSession.HomeShirt, home.AccentColor,
                    new Vector3(f.x, 0, f.y), 90f, Numbers[i], gk, true));
                players.Add(PlayerAvatar.Create(world.transform, "Away" + i,
                    gk ? (Color)gkAway : GameSession.AwayShirt, away.AccentColor,
                    new Vector3(-f.x, 0, f.y), -90f, Numbers[i], gk, false));
            }
        }

        void Update()
        {
            if (paused || hud == null) return;
            clock = Mathf.Min(clock + Time.deltaTime, GameSettings.MatchMinutes * 60f);
            hud.SetClock(clock);
        }

        void OnGoal(int side)
        {
            if (paused) return;
            paused = true;
            var scorer = side > 0 ? GameSession.Home : GameSession.Away;
            if (side > 0) homeScore++; else awayScore++;
            hud.SetScore(homeScore, awayScore);
            hud.ShowGoal(scorer.name);
            StartCoroutine(AfterGoal());
        }

        IEnumerator AfterGoal()
        {
            yield return new WaitForSeconds(2.5f);
            hud.HideGoal();
            ResetPositions();
            paused = false;
        }

        public void ResetPositions()
        {
            ball.ResetTo(Kickoff);
            foreach (var p in players) p.ResetPose();
        }

        public void CycleCamera() { if (cam != null) cam.Cycle(); }

        /// <summary>Chute de teste em direção ao gol mais distante da bola.</summary>
        public void TestKick(bool high)
        {
            if (paused) return;
            var p = ball.transform.position;
            float goalX = p.x > 0 ? -CourtSpec.HalfLength : CourtSpec.HalfLength;
            var dir = new Vector3(goalX - p.x, 0, Random.Range(-1.2f, 1.2f) - p.z);
            if (dir.sqrMagnitude < 1f) dir = Vector3.right;
            ball.Kick(dir, high ? 20f : 15f, high ? 3.5f : 0f, high);
        }

        public void Exit()
        {
            if (cam != null) Destroy(cam);
            if (hud != null) hud.Destroy();
            if (world != null) Destroy(world);

            var camera = Camera.main;
            if (camera != null)
            {
                camera.transform.position = camPos;
                camera.transform.rotation = camRot;
                camera.fieldOfView = camFov;
                camera.nearClipPlane = camNear;
                camera.farClipPlane = camFar;
                camera.clearFlags = camFlags;
                camera.backgroundColor = camBg;
            }
            if (MenuRoot.Instance != null) MenuRoot.Instance.SetVisible(true);
            Instance = null;
            Destroy(gameObject);
        }
    }
}
