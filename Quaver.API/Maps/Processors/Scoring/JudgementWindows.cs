using System;
using Quaver.API.Enums;
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

        public Judgement ComboBreakJudgement { get; set; } = Judgement.Miss;

        // Never actually null; the nullability is to allow migration from the old database schema without this column.
        public Judgement? LNMissJudgement { get; set; } = Judgement.Good;

        /// <summary>
        ///     Returns the value of the window from <see cref="Judgement"/>
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public float GetValueFromJudgement(Judgement j)
        {
            switch (j)
            {
                case Judgement.Marv:
                    return Marvelous;
                case Judgement.Perf:
                    return Perfect;
                case Judgement.Great:
                    return Great;
                case Judgement.Good:
                    return Good;
                case Judgement.Okay:
                    return Okay;
                case Judgement.Miss:
                    return Miss;
                default:
                    throw new ArgumentOutOfRangeException(nameof(j), j, null);
            }
        }
    }
}