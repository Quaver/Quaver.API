using System;
using System.Collections.Generic;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Replays;

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

        /// <summary>
        ///     ?
        /// </summary>
        private int MultiplierCount { get; set; }

        /// <summary>
        ///     ?
        /// </summary>
        private int MultiplierIndex { get; set; }

        /// <summary>
        ///     ?
        /// </summary>
        private int ScoreCount { get; set; }

        /// <summary>
        ///     Total Accuracy Weight Added
        /// </summary>
        private float AccuracyWeightCount { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override SortedDictionary<Judgement, float> JudgementWindow { get; set; } = new SortedDictionary<Judgement, float>
        {
            {Judgement.Marv, 16},
            {Judgement.Perf, 43},
            {Judgement.Great, 76},
            {Judgement.Good, 106},
            {Judgement.Okay, 127},
            {Judgement.Miss, 142}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override SortedDictionary<Judgement, float> WindowReleaseMultiplier { get; } = new SortedDictionary<Judgement, float>
        {
            {Judgement.Marv, 1.5f},
            {Judgement.Perf, 1.5f},
            {Judgement.Great, 1.5f},
            {Judgement.Good, 1.5f},
            {Judgement.Okay, 1.5f},
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
        public override Dictionary<Judgement, float> JudgementHealthWeighting { get; } = new Dictionary<Judgement, float>()
        {
            {Judgement.Marv, 0.5f},
            {Judgement.Perf, 0.4f},
            {Judgement.Great, 0.2f},
            {Judgement.Good, -3.0f},
            {Judgement.Okay, -4.5f},
            {Judgement.Miss, -6.0f}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, int> JudgementAccuracyWeighting { get; } = new Dictionary<Judgement, int>()
        {
            {Judgement.Marv, 100},
            {Judgement.Perf, 100},
            {Judgement.Great, 50},
            {Judgement.Good, 25},
            {Judgement.Okay, -50},
            {Judgement.Miss, -25}
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
        /// <param name="mods"></param>
        public ScoreProcessorKeys(Qua map, ModIdentifier mods) : base(map, mods)
        {
            TotalJudgements = GetTotalJudgementCount();
            SummedScore = CalculateSummedScore();
        }
        
        /// <summary>
        ///     Ctor - 
        /// </summary>
        /// <param name="replay"></param>
        public ScoreProcessorKeys(Replay replay) : base(replay){}

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="judgement"></param>
        public override void CalculateScore(Judgement judgement, float hitDifference)
        {
            // Add to the current judgements.
            CurrentJudgements[judgement]++;

            // Calculate current accuracy weight (todo: maybe put this in method? idk)
            //todo: create constants
            //todo: balancing
            // 100 = 100% acc weight
            // -150 = lowest acc weight that can be given is -50%, so yea)
            AccuracyWeightCount += 100 + -150 * (Math.Max(Math.Abs(hitDifference) - JudgementWindow[Judgement.Marv], 0)) / (JudgementWindow[Judgement.Miss] - JudgementWindow[Judgement.Marv]);

            // Calculate and set the new accuracy.
            Accuracy = CalculateAccuracy();
            
#region SCORE_CALCULATION                  
            // If the user didn't miss, then we want to update their combo and multiplier
            // accordingly.
            if (judgement != Judgement.Miss)
            {
                //Update Multiplier
                if (judgement == Judgement.Good)
                {
                    MultiplierCount -= 10;
                    if (MultiplierCount < 0)
                        MultiplierCount = 0;
                }
                else
                {
                    if (MultiplierCount < 150)
                        MultiplierCount++;
                    else
                        MultiplierCount = 150; //idk... just to be safe
                }

                // Add to the combo since the user hit.
                Combo++;

                // Set the max combo if applicable.
                if (Combo > MaxCombo)
                    MaxCombo = Combo;
            }
            // The user missed, so we want to update their multipler and reset their combo.
            else
            {
                // Update Multiplier
                MultiplierCount -= 20;
                
                if (MultiplierCount < 0) 
                    MultiplierCount = 0;
                
                // Reset combo.
                Combo = 0;
            }
                   
            // Update multiplier index and score count.
            MultiplierIndex = (int)Math.Floor(MultiplierCount/10f);
            ScoreCount += JudgementScoreWeighting[judgement] + MultiplierIndex * 10;
            
            // Update total score.
            Score = (int)(1000000 * ((double)ScoreCount / SummedScore));
#endregion

#region HEALTH_CALCULATION
            // Add health based on the health weighting for that given judgement.
            var newHealth = Health += JudgementHealthWeighting[judgement];

            // Constrain health from 0-100
            if (newHealth <= 0)
                Health = 0;
            else if (newHealth >= 100)
                Health = 100;
            else
                Health = newHealth;          
#endregion   
        }
        
        /// <inheritdoc />
        /// <summary>
        ///     Calculates the current accuracy.
        /// </summary>
        /// <returns></returns>
        protected override float CalculateAccuracy()
        {
            // OLD (todo: remove later)
            /*
             * float accuracy = 0;
            foreach (var item in CurrentJudgements)
                accuracy += item.Value * JudgementAccuracyWeighting[item.Key];
      
            return Math.Max(accuracy / (TotalJudgementCount * 100), 0) * 100;
            */

            return Math.Max(AccuracyWeightCount / (TotalJudgementCount * 100), 0) * 100;
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