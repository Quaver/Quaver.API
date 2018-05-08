using System;
using System.Collections.Generic;
using Quaver.API.Enums;

namespace Quaver.API.Maps.Processors.Scoring
{
    public class ScoreProcessorKeys : ScoreProcessor
    {

        /// <summary>
        ///     The maximum amount of judgements.
        ///
        ///     Each normal object counts as 1.
        ///     Additionally a long note counts as two (Beginning and End).
        /// </summary>
        private int TotalJudgements { get; }
        
         /// <summary>
        ///     See: ScoreProcessorKeys.CalculateSummedScore();
        /// </summary>
        private int SummedScore { get; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, int> JudgementWindow { get; } = new Dictionary<Judgement, int>
        {
            {Judgement.Marv, 16},
            {Judgement.Perf, 40},
            {Judgement.Great, 73},
            {Judgement.Good, 103},
            {Judgement.Okay, 127},
            {Judgement.Miss, 164}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, int> JudgementScoreWeighting { get; } = new Dictionary<Judgement, int>()
        {
            {Judgement.Marv, 100},
            {Judgement.Perf, 50},
            {Judgement.Great, 25},
            {Judgement.Good, 10},
            {Judgement.Okay, 5},
            {Judgement.Miss, 0}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, int> JudgementHealthWeighting { get; } = new Dictionary<Judgement, int>()
        {
            {Judgement.Marv, 100},
            {Judgement.Perf, 50},
            {Judgement.Great, 25},
            {Judgement.Good, 10},
            {Judgement.Okay, 5},
            {Judgement.Miss, 0}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Grade, int> GradePercentage { get; } = new Dictionary<Grade, int>()
        {
            {Grade.XX, 100},
            {Grade.X, 100},
            {Grade.SS, 99},
            {Grade.S, 95},
            {Grade.A, 90},
            {Grade.B, 80},
            {Grade.C, 70},
            {Grade.D, 60},
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="map"></param>
        public ScoreProcessorKeys(Qua map) : base(map)
        {
            TotalJudgements = GetTotalJudgementCount();
            SummedScore = CalculateSummedScore();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void CalculateScoreForObject(HitObjectInfo hitObject, int songTime, bool didHit)
        {
        }

        /// <summary>
        ///     Gets the absolute total judgements of the map.
        ///
        ///     Every normal object counts as 1 judgement.
        ///     Every LN counts as 2 judgements (Beginning + End)
        /// </summary>
        /// <returns></returns>
        private int GetTotalJudgementCount()
        {
            var judgements = 0;

            foreach (var o in Map.HitObjects)
            {
                if (o.IsLongNote)
                    judgements += 2;
                else
                    judgements++;
            }

            return judgements;
        }
        
        /// <summary>
        ///     Calculates the max score you can achieve in a song.
        ///         - Counts every Marv Hit
        ///
        ///     The user's current score is divided by this value then multiplied by 1,000,000
        ///     to get the score out of a million - the true max score.
        ///
        ///     When playing, your score multiplier starts at 0, then increases each 10-combo increment.
        ///     The max score multiplier is 2.5x, and doesn't increase after 150 combo.
        ///
        ///     At that point, every marv will be worth 250 points, however a marv without any multiplier
        ///     is worth 100 points.
        ///
        ///     25650 is the value when you add all the scores together for every successful Marv below 150 combo.
        ///     
        /// </summary>
        /// <returns></returns>
        private int CalculateSummedScore()
        {
            int summedScore;

            // Multiplier doesn't increase after this amount.
            const int comboCap = 150;
            
            // Calculate max score
            if (TotalJudgements < comboCap)
            {
                summedScore = 0;
                
                for (var i = 1; i < TotalJudgements + 1; i++)
                    summedScore += 100 + 10 * (int) Math.Floor(i / 10f);
            }
            else
                summedScore = 25650 + (TotalJudgements - (comboCap - 1)) * 250;

            return summedScore;
        }
    }
}