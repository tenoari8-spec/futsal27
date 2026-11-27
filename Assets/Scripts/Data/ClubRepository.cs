using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Futsal27.Data
{
    /// <summary>
    /// Base de dados editável de clubes. Fica em clubs.json (persistentDataPath).
    /// Escudos: coloque PNGs na pasta "Escudos" com o nome indicado em crestFile.
    /// </summary>
    public static class ClubRepository
    {
        public static List<ClubData> Clubs { get; private set; } = new List<ClubData>();

        public static string FilePath => Path.Combine(Application.persistentDataPath, "clubs.json");
        public static string CrestDir => Path.Combine(Application.persistentDataPath, "Escudos");

        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(CrestDir);
                if (File.Exists(FilePath))
                {
                    var db = JsonUtility.FromJson<ClubDatabase>(File.ReadAllText(FilePath));
                    if (db != null && db.clubs != null && db.clubs.Count >= 2)
                    {
                        Clubs = db.clubs;
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Falha ao ler clubs.json, usando padrão: " + e.Message);
            }

            Clubs = Defaults();
            Save();
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText(FilePath, JsonUtility.ToJson(new ClubDatabase { clubs = Clubs }, true));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Falha ao salvar clubs.json: " + e.Message);
            }
        }

        static ClubData C(string id, string name, string sh, string city, int a, int m, int d,
                          string home, string away, string accent)
        {
            return new ClubData
            {
                id = id, name = name, shortName = sh, city = city, crestFile = id + ".png",
                attack = a, midfield = m, defense = d,
                homeKit = home, awayKit = away, accent = accent
            };
        }

        // Cores e notas são PLACEHOLDERS: edite clubs.json com os dados reais.
        static List<ClubData> Defaults()
        {
            return new List<ClubData>
            {
                C("magnus",      "Magnus Futsal",    "MAG", "Sorocaba - SP",             84, 83, 81, "#0B3D91", "#FFFFFF", "#F2C200"),
                C("jaragua",     "Jaraguá Futsal",   "JAR", "Jaraguá do Sul - SC",       82, 82, 80, "#C8102E", "#FFFFFF", "#111111"),
                C("corinthians", "Corinthians Futsal","COR", "São Paulo - SP",           80, 79, 80, "#FFFFFF", "#111111", "#111111"),
                C("joinville",   "Joinville Futsal", "JOI", "Joinville - SC",            79, 78, 79, "#1B7F3B", "#FFFFFF", "#FFFFFF"),
                C("atlantico",   "Atlântico Erechim","ATL", "Erechim - RS",              78, 79, 78, "#0D47A1", "#FFFFFF", "#E53935"),
                C("cascavel",    "Cascavel Futsal",  "CAS", "Cascavel - PR",             78, 77, 77, "#F9A825", "#1A1A1A", "#1A1A1A"),
                C("carlosbarbosa","Carlos Barbosa",  "CBA", "Carlos Barbosa - RS",       77, 77, 78, "#B71C1C", "#FFFFFF", "#FFFFFF"),
                C("praiaclube",  "Praia Clube",      "PRA", "Uberlândia - MG",           76, 76, 75, "#F57C00", "#FFFFFF", "#1A1A1A"),
                C("campomourao", "Campo Mourão",     "CMO", "Campo Mourão - PR",         75, 75, 75, "#1565C0", "#FFFFFF", "#FFFFFF"),
                C("marreco",     "Marreco Futsal",   "MAR", "Francisco Beltrão - PR",    75, 74, 75, "#2E7D32", "#FFFFFF", "#FFEB3B"),
                C("pato",        "Pato Futsal",      "PAT", "Pato Branco - PR",          74, 75, 74, "#FBC02D", "#1A1A1A", "#1A1A1A"),
                C("saolourenco", "São Lourenço Futsal","SLO","São Lourenço do Oeste - SC",73, 74, 73, "#C62828", "#FFFFFF", "#FFFFFF"),
            };
        }
    }
}
