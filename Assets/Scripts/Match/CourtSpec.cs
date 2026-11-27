namespace Futsal27.Match
{
    /// <summary>Medidas da quadra de futsal (em metros). X = comprimento, Z = largura, Y = altura.</summary>
    public static class CourtSpec
    {
        public const float HalfLength = 20f;     // quadra 40 x 20 m
        public const float HalfWidth = 10f;
        public const float GoalHalfWidth = 1.5f; // gol 3 x 2 m
        public const float GoalHeight = 2f;
        public const float GoalDepth = 1f;
        public const float BallRadius = 0.15f;   // um pouco maior que o real (0,10 m) para ficar legível no celular
        public const float BallMass = 0.42f;
        public const float WallX = 21.5f;        // limite físico (paredes invisíveis)
        public const float WallZ = 10.5f;
        public const float CeilingY = 8f;
    }
}
