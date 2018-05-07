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
        public int Health { get; protected set; } = 100;

        /// <summary>
        ///     The judgement count for each judgement, initialized to 0 by default.
        /// 
        ///     Note: Not sure if modes will use different judgements, probably not.
        /// </summary>
        public Dictionary<Judgement, int> JudgementCount { get; } = new Dictionary<Judgement, int>()
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
        public abstract Dictionary<Judgement, int> JudgementWindow { get; }

        /// <summary>
        ///     The weighting for score defined per mode.
        /// </summary>
        public abstract Dictionary<Judgement, int> JudgementScoreWeighting { get; }

        /// <summary>
        ///     The weighting for health defined per mode.
        /// </summary>
        public abstract Dictionary<Judgement, int> JudgementHealthWeighting { get; }

        /// <summary>
        ///     The percentage for each grade.
        /// </summary>
        public abstract Dictionary<Grade, int> GradePercentage { get; }
        
        /// <summary>
        ///     Ctor - 
        /// </summary>
        /// <param name="map"></param>
        public ScoreProcessor(Qua map)
        {
            Map = map;
        }

         /// <summary>
        ///     Calculates score and accuracy for a given object and song time.
        /// </summary>
        public abstract void CalculateScoreForObject(HitObjectInfo hitObject, int songTime, bool didHit);
    }
}