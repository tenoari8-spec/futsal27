using UnityEngine;

namespace Futsal27.Match
{
    /// <summary>Câmera esportiva que acompanha a bola, com 3 ângulos (lateral, aérea, baixa).</summary>
    public class MatchCamera : MonoBehaviour
    {
        struct Preset { public Vector3 offset; public float fov, follow; }

        static readonly Preset[] Presets =
        {
            new Preset { offset = new Vector3(0, 12f, -17f), fov = 42f, follow = .8f },
            new Preset { offset = new Vector3(0, 24f, -6f),  fov = 50f, follow = .8f },
            new Preset { offset = new Vector3(0, 5.5f, -13f), fov = 48f, follow = .9f },
        };

        public Transform target;
        int idx;
        Camera cam;
        Vector3 focus;

        void Awake() { cam = GetComponent<Camera>(); }

        public void Cycle() { idx = (idx + 1) % Presets.Length; }
        public void SnapNow() { Step(1f, 1f); }

        void LateUpdate()
        {
            Step(1f - Mathf.Exp(-4f * Time.deltaTime), 1f - Mathf.Exp(-3f * Time.deltaTime));
        }

        void Step(float kPos, float kRot)
        {
            if (target == null) return;
            var p = Presets[idx];
            float fx = Mathf.Clamp(target.position.x * p.follow, -15f, 15f);
            focus = Vector3.Lerp(focus, new Vector3(fx, 0, 0), kPos);

            transform.position = Vector3.Lerp(transform.position, focus + p.offset, kPos);
            var look = Quaternion.LookRotation(focus - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, kRot);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, p.fov, kRot);
        }
    }
}
