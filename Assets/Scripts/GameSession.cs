using UnityEngine;
using Futsal27.Data;

namespace Futsal27.Core
{
    /// <summary>Estado da partida que será jogada (times e uniformes escolhidos).</summary>
    public static class GameSession
    {
        public static ClubData Home, Away;
        public static bool HomeUsesAwayKit, AwayUsesAwayKit;

        public static Color HomeShirt => HomeUsesAwayKit ? Home.AwayColor : Home.HomeColor;
        public static Color AwayShirt => AwayUsesAwayKit ? Away.AwayColor : Away.HomeColor;
    }
}
