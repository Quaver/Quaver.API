using SQLite;

namespace Quaver.API.Maps.Processors.Scoring
{
    public class JudgementWindows
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        ///     The preset name of the judgement windows
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     If these windows are Quaver defaults
        /// </summary>
        [Ignore]
        public bool IsDefault { get; set; }

        public float Marvelous { get; set; } = 18;

        public float Perfect { get; set; } = 43;

        public float Great { get; set; } = 76;

        public float Good { get; set; } = 106;

        public float Okay { get; set; } = 127;

        public float Miss { get; set; } = 164;
    }
}