using System;
using System.Collections.Generic;
using UnityEngine;

namespace Futsal27.Data
{
    [Serializable]
    public class ClubData
    {
        public string id;
        public string name;
        public string shortName;
        public string city;
        public string crestFile;          // PNG dentro da pasta "Escudos"
        public int attack = 75;
        public int midfield = 75;
        public int defense = 75;
        public string homeKit = "#FFFFFF";   // uniforme principal
        public string awayKit = "#111111";   // uniforme reserva
        public string accent = "#00E676";    // detalhes

        public Color HomeColor => Hex(homeKit);
        public Color AwayColor => Hex(awayKit);
        public Color AccentColor => Hex(accent);
        public int Overall => Mathf.RoundToInt((attack + midfield + defense) / 3f);

        public static Color Hex(string h)
        {
            return ColorUtility.TryParseHtmlString(h, out var c) ? c : Color.white;
        }
    }

    [Serializable]
    public class ClubDatabase
    {
        public List<ClubData> clubs = new List<ClubData>();
    }
}
