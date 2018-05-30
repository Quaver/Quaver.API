using System;
using System.Collections.Generic;
using Quaver.API.Enums;

namespace Quaver.API.Maps.Processors.Scoring
{
    public abstract class ScoreProcessor
    {
        /// <summary>
        ///     The map that will have its score processed.
        /// </summary>
        public Qua Map { get;  }

        /// <summary>
        ///     The mods for this play.
        /// </summary>
        public ModIdentifier Mods { get; }

        /// <summary>
        ///     The total score the user has.
        /// </summary>
        public int Score { get; protected set; }

        /// <summary>
        ///     The accuracy the user has.
        /// </summary>
        public float Accuracy { get; protected set; }

        /// <summary>
        ///     The current health.
        /// </summary>
        public float Health { get; protected set; } = 100;

        /// <summary>
        ///     The user's current combo.
        /// </summary>
        public int Combo { get; protected set; }

        /// <summary>
        ///     The max combo achieved for this play session.
        /// </summary>
        public int MaxCombo { get; protected set; }

        /// <summary>
        ///     If the score is currently failed.
        /// </summary>
        public bool Failed => Health <= 0;
        
        /// <summary>
        ///     The judgement count for each judgement, initialized to 0 by default.
        /// 
        ///     Note: Not sure if modes will use different judgements, probably not.
        /// </summary>
        public Dictionary<Judgement, int> CurrentJudgements { get; } = new Dictionary<Judgement, int>()
        {
            {Judgement.Marv, 0},
            {Judgement.Perf, 0},
            {Judgement.Great, 0},
            {Judgement.Good, 0},
            {Judgement.Okay, 0},
            {Judgement.Miss, 0}
        };

        /// <summary>
        ///     The judgement windows defined per mode.
        /// </summary>
        public abstract SortedDictionary<Judgement, float> JudgementWindow { get; set; }

        /// <summary>
        ///     The weighting for score defined per mode.
        /// </summary>
        public abstract Dictionary<Judgement, int> JudgementScoreWeighting { get; }

        /// <summary>
        ///     The weighting for health defined per mode.
        /// </summary>
        public abstract Dictionary<Judgement, float> JudgementHealthWeighting { get; }

        /// <summary>
        ///     The weighting for accuracy.
        /// </summary>
        public abstract Dictionary<Judgement, int> JudgementAccuracyWeighting { get; }

        /// <summary>
        ///     The percentage for each grade.
        /// </summary>
        public abstract Dictionary<Grade, int> GradePercentage { get; }

        /// <summary>
        ///     The window multiplier for long notes.
        ///     It multiplies the judgement window by this amount.
        /// </summary>
        public abstract SortedDictionary<Judgement, float> WindowReleaseMultiplier { get; }
        
        /// <summary>
        ///     The total amount of judgements that the user has gotten in this play session.
        /// </summary>
        protected int TotalJudgementCount
        {
            get
            {
                var sum = 0;

                foreach (var item in CurrentJudgements)
                    sum += item.Value;

                return sum;
            }
        }

        /// <summary>
        ///     Ctor - 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mods"></param>
        public ScoreProcessor(Qua map, ModIdentifier mods)
        {
            Map = map;
            Mods = mods;
            
            InitializeMods();
        }

        /// <summary>
        ///     Adds a judgement to the score and recalculates the score.
        /// </summary>
        public abstract void CalculateScore(Judgement judgement);

        /// <summary>
        ///     Calculates the accuracy of the current play session.
        /// </summary>
        /// <returns></returns>
        protected abstract float CalculateAccuracy();

        /// <summary>
        ///     Initializes the mods for this given play.
        ///     (Recalculates hit windows.)
        /// </summary>
        private void InitializeMods()
        {
            Console.WriteLine(Mods);
            
            // The rate of the audio.
            var rate = 1.0;
            
            // Map mods to rate.
            if (Mods.HasFlag(ModIdentifier.Speed05X))
                rate = 0.5f;
            else if (Mods.HasFlag(ModIdentifier.Speed06X))
                rate = 0.6f;
            else if (Mods.HasFlag(ModIdentifier.Speed07X))
                rate = 0.7f;
            else if (Mods.HasFlag(ModIdentifier.Speed08X))
                rate = 0.8f;
            else if (Mods.HasFlag(ModIdentifier.Speed09X))
                rate = 0.9f;
            else if (Mods.HasFlag(ModIdentifier.Speed11X))
                rate = 1.1f;
            else if (Mods.HasFlag(ModIdentifier.Speed12X))
                rate = 1.2f;
            else if (Mods.HasFlag(ModIdentifier.Speed13X))
                rate = 1.3f;
            else if (Mods.HasFlag(ModIdentifier.Speed14X))
                rate = 1.4f;
            else if (Mods.HasFlag(ModIdentifier.Speed15X))
                rate = 1.5f;
            else if (Mods.HasFlag(ModIdentifier.Speed16X))
                rate = 1.6f;
            else if (Mods.HasFlag(ModIdentifier.Speed17X))
                rate = 1.7f;
            else if (Mods.HasFlag(ModIdentifier.Speed18X))
                rate = 1.8f;
            else if (Mods.HasFlag(ModIdentifier.Speed19X))
                rate = 1.9f;
            else if (Mods.HasFlag(ModIdentifier.Speed20X))
                rate = 2.0f;

            for (var i = 0; i < JudgementWindow.Count; i++)
                JudgementWindow[(Judgement) i] *= (float)rate;
        }
    }
}