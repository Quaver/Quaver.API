namespace Quaver.API.Gameplay
{
    /// <summary>
    ///     The weighting in accuracy that each judge gives
    /// </summary>
    public static class JudgeAccuracyWeighting
    {
        public static readonly int Marv = 100;
        public static readonly int Perf = 100;
        public static readonly int Great = 50;
        public static readonly int Good = -50;
        public static readonly int Okay = -100;
        public static readonly int Miss = 0;
    }
}
