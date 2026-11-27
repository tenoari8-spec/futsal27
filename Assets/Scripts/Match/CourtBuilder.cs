using System.Collections.Generic;
using UnityEngine;

namespace Futsal27.Match
{
    /// <summary>Monta a quadra coberta: piso, linhas, gols, paredes invisíveis, placas e arquibancadas.</summary>
    public static class CourtBuilder
    {
        static Material ghost, lineMat;

        static Material Ghost()
        {
            if (ghost == null) { ghost = new Material(Shader.Find("Sprites/Default")); ghost.color = new Color(1, 1, 1, .22f); }
            return ghost;
        }

        static Material LineMat()
        {
            if (lineMat == null) { lineMat = new Material(Shader.Find("Sprites/Default")); lineMat.color = Color.white; }
            return lineMat;
        }

        static PhysicMaterial Phys(string n, float bounce, float fric)
        {
            return new PhysicMaterial(n) { bounciness = bounce, dynamicFriction = fric, staticFriction = fric };
        }

        static GameObject Box(Transform p, string name, Vector3 c, Vector3 s, Color color,
                              PhysicMaterial m, bool collider, bool visible = true)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name;
            g.transform.SetParent(p, false);
            g.transform.localPosition = c;
            g.transform.localScale = s;

            var bc = g.GetComponent<BoxCollider>();
            if (!collider) Object.DestroyImmediate(bc);
            else if (m != null) bc.sharedMaterial = m;

            var r = g.GetComponent<MeshRenderer>();
            if (!visible) r.enabled = false;
            else r.material.color = color;
            return g;
        }

        static void Arc(Transform p, string name, List<Vector3> pts)
        {
            var g = new GameObject(name);
            g.transform.SetParent(p, false);
            g.transform.rotation = Quaternion.Euler(90, 0, 0);
            var lr = g.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = pts.Count;
            lr.SetPositions(pts.ToArray());
            lr.widthMultiplier = .08f;
            lr.alignment = LineAlignment.TransformZ;
            lr.sharedMaterial = LineMat();
            lr.startColor = lr.endColor = Color.white;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
        }

        public static void Build(Transform parent, System.Action<int> onGoal)
        {
            var floorMat = Phys("Floor", .55f, .6f);
            var wallMat = Phys("Wall", .35f, .2f);
            var netMat = Phys("Net", .05f, .8f);
            var white = Color.white;
            const float L = CourtSpec.HalfLength, W = CourtSpec.HalfWidth;

            // Piso
            Box(parent, "Floor", new Vector3(0, -.25f, 0), new Vector3(2 * CourtSpec.WallX, .5f, 2 * CourtSpec.WallZ),
                new Color32(24, 58, 108, 255), floorMat, true);
            Box(parent, "PlayingSurface", new Vector3(0, .005f, 0), new Vector3(2 * L, .01f, 2 * W),
                new Color32(30, 92, 172, 255), null, false);

            // Linhas
            Box(parent, "LineN", new Vector3(0, .015f, W), new Vector3(2 * L + .08f, .01f, .08f), white, null, false);
            Box(parent, "LineS", new Vector3(0, .015f, -W), new Vector3(2 * L + .08f, .01f, .08f), white, null, false);
            Box(parent, "LineE", new Vector3(L, .015f, 0), new Vector3(.08f, .01f, 2 * W), white, null, false);
            Box(parent, "LineW", new Vector3(-L, .015f, 0), new Vector3(.08f, .01f, 2 * W), white, null, false);
            Box(parent, "LineMid", new Vector3(0, .015f, 0), new Vector3(.08f, .01f, 2 * W), white, null, false);
            Box(parent, "CenterSpot", new Vector3(0, .015f, 0), new Vector3(.2f, .01f, .2f), white, null, false);

            var circle = new List<Vector3>();
            for (int i = 0; i <= 48; i++)
            {
                float t = i / 48f * Mathf.PI * 2f;
                circle.Add(new Vector3(3f * Mathf.Cos(t), .02f, 3f * Mathf.Sin(t)));
            }
            Arc(parent, "CenterCircle", circle);

            for (int s = -1; s <= 1; s += 2)
            {
                float gx = s * L;

                // Área: dois quartos de círculo de 6 m ligados por uma reta
                for (int z = -1; z <= 1; z += 2)
                {
                    var a = new List<Vector3>();
                    for (int i = 0; i <= 16; i++)
                    {
                        float t = i / 16f * 90f * Mathf.Deg2Rad;
                        a.Add(new Vector3(gx - s * 6f * Mathf.Cos(t), .02f,
                                          z * (CourtSpec.GoalHalfWidth + 6f * Mathf.Sin(t))));
                    }
                    Arc(parent, "Area" + s + z, a);
                }
                Box(parent, "AreaLine" + s, new Vector3(gx - s * 6f, .015f, 0),
                    new Vector3(.08f, .01f, 2 * CourtSpec.GoalHalfWidth), white, null, false);

                // Marca do pênalti (6 m) e segunda marca (10 m)
                Box(parent, "Spot6_" + s, new Vector3(s * 14f, .015f, 0), new Vector3(.2f, .01f, .2f), white, null, false);
                Box(parent, "Spot10_" + s, new Vector3(s * 10f, .015f, 0), new Vector3(.2f, .01f, .2f), white, null, false);

                BuildGoal(parent, s, netMat, onGoal);
            }

            // Paredes e teto invisíveis (mantêm a bola dentro; regras de bola fora entram na Etapa 6)
            Box(parent, "WallN", new Vector3(0, CourtSpec.CeilingY / 2f, CourtSpec.WallZ + .25f),
                new Vector3(2 * CourtSpec.WallX + 1f, CourtSpec.CeilingY, .5f), white, wallMat, true, false);
            Box(parent, "WallS", new Vector3(0, CourtSpec.CeilingY / 2f, -CourtSpec.WallZ - .25f),
                new Vector3(2 * CourtSpec.WallX + 1f, CourtSpec.CeilingY, .5f), white, wallMat, true, false);
            Box(parent, "WallE", new Vector3(CourtSpec.WallX + .25f, CourtSpec.CeilingY / 2f, 0),
                new Vector3(.5f, CourtSpec.CeilingY, 2 * CourtSpec.WallZ), white, wallMat, true, false);
            Box(parent, "WallW", new Vector3(-CourtSpec.WallX - .25f, CourtSpec.CeilingY / 2f, 0),
                new Vector3(.5f, CourtSpec.CeilingY, 2 * CourtSpec.WallZ), white, wallMat, true, false);
            Box(parent, "Ceiling", new Vector3(0, CourtSpec.CeilingY + .25f, 0),
                new Vector3(2 * CourtSpec.WallX + 1f, .5f, 2 * CourtSpec.WallZ + 1f), white, wallMat, true, false);

            // Placas de publicidade
            var board = new Color32(14, 24, 34, 255);
            var strip = new Color32(0, 230, 118, 255);
            for (int s = -1; s <= 1; s += 2)
            {
                Box(parent, "BoardSide" + s, new Vector3(0, .45f, s * (CourtSpec.WallZ + .05f)),
                    new Vector3(2 * CourtSpec.WallX, .9f, .1f), board, null, false);
                Box(parent, "BoardSideStrip" + s, new Vector3(0, .93f, s * (CourtSpec.WallZ + .05f)),
                    new Vector3(2 * CourtSpec.WallX, .06f, .12f), strip, null, false);
                Box(parent, "BoardEnd" + s, new Vector3(s * (CourtSpec.WallX + .05f), .45f, 0),
                    new Vector3(.1f, .9f, 2 * CourtSpec.WallZ), board, null, false);
            }

            // Arquibancadas (lado oposto à câmera e atrás dos gols)
            for (int k = 0; k < 6; k++)
            {
                float h = 1f + k * .9f;
                var c = Color.Lerp(new Color32(22, 30, 46, 255), new Color32(44, 58, 86, 255), k / 5f);
                Box(parent, "StandFar" + k, new Vector3(0, h / 2f, 11.8f + k * 1.2f), new Vector3(52f, h, 1.2f), c, null, false);
                for (int s = -1; s <= 1; s += 2)
                    Box(parent, "StandEnd" + s + k, new Vector3(s * (22.8f + k * 1.2f), h / 2f, 0), new Vector3(1.2f, h, 24f),
                        c, null, false);
            }
        }

        static void BuildGoal(Transform parent, int s, PhysicMaterial netMat, System.Action<int> onGoal)
        {
            float gx = s * CourtSpec.HalfLength;
            float gw = CourtSpec.GoalHalfWidth;

            for (int z = -1; z <= 1; z += 2)
                Cyl(parent, "Post", new Vector3(gx, 1f, z * gw), new Vector3(.08f, 1f, .08f), Quaternion.identity);
            Cyl(parent, "Crossbar", new Vector3(gx, CourtSpec.GoalHeight, 0), new Vector3(.08f, gw, .08f),
                Quaternion.Euler(90, 0, 0));

            Net(parent, new Vector3(gx + s * CourtSpec.GoalDepth, 1f, 0), new Vector3(.05f, 2f, 2 * gw), netMat);
            Net(parent, new Vector3(gx + s * .5f, 1f, gw), new Vector3(1f, 2f, .05f), netMat);
            Net(parent, new Vector3(gx + s * .5f, 1f, -gw), new Vector3(1f, 2f, .05f), netMat);
            Net(parent, new Vector3(gx + s * .5f, 2f, 0), new Vector3(1f, .05f, 2 * gw), netMat);

            var sensor = new GameObject("GoalSensor" + s);
            sensor.transform.SetParent(parent, false);
            sensor.transform.localPosition = new Vector3(gx + s * .6f, 1f, 0);
            var bc = sensor.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = new Vector3(.9f, 2f, 2 * gw);
            var gs = sensor.AddComponent<GoalSensor>();
            gs.side = s;
            gs.onGoal = onGoal;
        }

        static void Cyl(Transform p, string name, Vector3 pos, Vector3 scale, Quaternion rot)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            g.name = name;
            g.transform.SetParent(p, false);
            g.transform.localPosition = pos;
            g.transform.localScale = scale;
            g.transform.localRotation = rot;
            g.GetComponent<Renderer>().material.color = Color.white;
        }

        static void Net(Transform p, Vector3 pos, Vector3 size, PhysicMaterial m)
        {
            var g = Box(p, "Net", pos, size, Color.white, m, true);
            var r = g.GetComponent<Renderer>();
            r.sharedMaterial = Ghost();
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
        }
    }
}
