using UnityEngine;

namespace Futsal27.Match
{
    /// <summary>
    /// Bola de futsal com física própria: quique baixo (~0,55), desaceleração de rolamento,
    /// efeito Magnus (curva por rotação) e colisão contínua para chutes rápidos.
    /// </summary>
    public class BallController : MonoBehaviour
    {
        const float RollDecel = 1.0f;   // m/s² de perda ao rolar no piso
        const float MagnusCoeff = 0.003f;

        Rigidbody rb;
        public Rigidbody Body { get { return rb; } }

        public static BallController Create(Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Ball";
            go.transform.SetParent(parent, false);
            go.transform.localScale = Vector3.one * CourtSpec.BallRadius * 2f;

            var col = go.GetComponent<SphereCollider>();
            col.sharedMaterial = new PhysicMaterial("Ball") { bounciness = .55f, dynamicFriction = .5f, staticFriction = .5f };

            var body = go.AddComponent<Rigidbody>();
            body.mass = CourtSpec.BallMass;
            body.drag = .05f;
            body.angularDrag = .4f;
            body.maxAngularVelocity = 80f;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.interpolation = RigidbodyInterpolation.Interpolate;

            var rend = go.GetComponent<Renderer>();
            rend.material.mainTexture = MakeTexture();
            rend.material.color = Color.white;

            var bc = go.AddComponent<BallController>();
            bc.rb = body;
            return bc;
        }

        // Textura quadriculada: deixa a rotação da bola visível
        static Texture2D MakeTexture()
        {
            var t = new Texture2D(128, 64, TextureFormat.RGBA32, true);
            var px = new Color[128 * 64];
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 128; x++)
                {
                    bool dark = ((x / 16) + (y / 16)) % 2 == 0;
                    px[y * 128 + x] = dark ? new Color(.12f, .12f, .14f) : new Color(.96f, .96f, .96f);
                }
            t.SetPixels(px);
            t.Apply();
            return t;
        }

        void FixedUpdate()
        {
            var v = rb.velocity;

            // Resistência de rolamento: só quando a bola está no piso
            if (transform.position.y <= CourtSpec.BallRadius + .03f)
            {
                var h = new Vector3(v.x, 0, v.z);
                float s = h.magnitude;
                if (s > .01f)
                {
                    float ns = Mathf.Max(0f, s - RollDecel * Time.fixedDeltaTime);
                    rb.velocity = new Vector3(h.x / s * ns, v.y, h.z / s * ns);
                }
            }

            // Efeito Magnus: a rotação curva a trajetória no ar
            rb.AddForce(MagnusCoeff * Vector3.Cross(rb.angularVelocity, rb.velocity));
        }

        public void ResetTo(Vector3 pos)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = pos;
            transform.position = pos;
        }

        /// <summary>Chute de teste: define velocidade e rotação (chutes reais por impulso entram na Etapa 4).</summary>
        public void Kick(Vector3 dir, float speed, float lift, bool curve)
        {
            dir.y = 0;
            dir.Normalize();
            var vel = dir * speed + Vector3.up * lift;
            rb.velocity = vel;

            if (!curve)
            {
                rb.angularVelocity = Vector3.Cross(vel, Vector3.up) / CourtSpec.BallRadius; // rolando sem deslizar
            }
            else
            {
                var side = Vector3.Cross(dir, Vector3.up);
                rb.angularVelocity = side * (speed / CourtSpec.BallRadius) * -.4f + Vector3.up * Random.Range(-20f, 20f);
            }
        }
    }
}
