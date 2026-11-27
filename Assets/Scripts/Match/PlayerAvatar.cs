using UnityEngine;

namespace Futsal27.Match
{
    /// <summary>
    /// Jogador provisório (primitivas). O esqueleto e as animações reais entram nas Etapas 3 e 9;
    /// aqui já existem corpo físico, camisa, posição de origem e movimento ocioso.
    /// </summary>
    public class PlayerAvatar : MonoBehaviour
    {
        public bool IsHome;
        public int Number;
        public bool Keeper;

        Vector3 spawn;
        Quaternion spawnRot;
        Transform torso;
        float baseY, phase;

        static readonly Color[] Skins =
        {
            new Color(.87f, .68f, .55f), new Color(.72f, .52f, .38f), new Color(.45f, .30f, .22f)
        };

        public static PlayerAvatar Create(Transform parent, string name, Color shirt, Color shorts, Vector3 pos,
                                          float yaw, int number, bool keeper, bool home)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = pos;
            root.transform.rotation = Quaternion.Euler(0, yaw, 0);

            var p = root.AddComponent<PlayerAvatar>();
            p.spawn = pos;
            p.spawnRot = root.transform.rotation;
            p.Number = number;
            p.Keeper = keeper;
            p.IsHome = home;
            p.phase = Random.value * 6.28f;

            Part(root.transform, PrimitiveType.Capsule, "Legs", new Vector3(0, .4f, 0), new Vector3(.34f, .4f, .34f), shorts);
            p.torso = Part(root.transform, PrimitiveType.Capsule, "Torso", new Vector3(0, 1.12f, 0),
                           new Vector3(.44f, .36f, .3f), shirt);
            p.baseY = 1.12f;
            Part(root.transform, PrimitiveType.Sphere, "Head", new Vector3(0, 1.62f, 0), Vector3.one * .24f,
                 Skins[Random.Range(0, Skins.Length)]);

            var cap = root.AddComponent<CapsuleCollider>();
            cap.radius = .28f;
            cap.height = 1.75f;
            cap.center = new Vector3(0, .875f, 0);

            var rb = root.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            return p;
        }

        static Transform Part(Transform parent, PrimitiveType type, string name, Vector3 lp, Vector3 ls, Color c)
        {
            var g = GameObject.CreatePrimitive(type);
            g.name = name;
            DestroyImmediate(g.GetComponent<Collider>());
            g.transform.SetParent(parent, false);
            g.transform.localPosition = lp;
            g.transform.localScale = ls;
            g.GetComponent<Renderer>().material.color = c;
            return g.transform;
        }

        void Update()
        {
            var lp = torso.localPosition;
            lp.y = baseY + Mathf.Sin(Time.time * 2f + phase) * .012f;
            torso.localPosition = lp;
        }

        public void ResetPose()
        {
            transform.position = spawn;
            transform.rotation = spawnRot;
        }
    }
}
