using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override SortedDictionary<Judgement, float> JudgementWindow { get; set; } = new SortedDictionary<Judgement, float>
        {
            {Judgement.Marvelous, 16},
            {Judgement.Perfect, 40},
            {Judgement.Great, 73},
            {Judgement.Good, 103},
            {Judgement.Okay, 127},
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override SortedDictionary<Judgement, float> WindowReleaseMultiplier { get; } = new SortedDictionary<Judgement, float>
        {
            {Judgement.Marvelous, 1.5f},
            {Judgement.Perfect, 1.7f},
            {Judgement.Great, 1.8f},
            {Judgement.Good, 2.0f},
            {Judgement.Okay, 1.0f},
        };
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, int> JudgementScoreWeighting { get; } = new Dictionary<Judgement, int>()
        {
            {Judgement.Marvelous, 100},
            {Judgement.Perfect, 50},
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
            {Judgement.Marvelous, 0.5f},
            {Judgement.Perfect, 0.4f},
            {Judgement.Great, 0.1f},
            {Judgement.Good, -2.0f},
            {Judgement.Okay, -2.5f},
            {Judgement.Miss, -3.0f}
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override Dictionary<Judgement, int> JudgementAccuracyWeighting { get; } = new Dictionary<Judgement, int>()
        {
            {Judgement.Marvelous, 100},
            {Judgement.Perfect, 100},
            {Judgement.Great, 50},
            {Judgement.Good, -50},
            {Judgement.Okay, -100},
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
        public ScoreProcessorKeys(Qua map, ModIdentifier mods) : base(map, mods)
        {
            TotalJudgements = GetTotalJudgementCount();
            SummedScore = CalculateSummedScore();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="judgement"></param>
        public override void CalculateScore(Judgement judgement)
        {
            // Add to the current judgements.
            CurrentJudgements[judgement]++;
            
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
            float accuracy = 0;

            foreach (var item in CurrentJudgements)
                accuracy += item.Value * JudgementAccuracyWeighting[item.Key];
      
            return Math.Max(accuracy / (TotalJudgementCount * 100), 0) * 100;  
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