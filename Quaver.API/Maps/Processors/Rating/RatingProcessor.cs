using Quaver.API.Maps.Processors.Difficulty;
using Quaver.API.Maps.Processors.Scoring;

namespace Quaver.API.Maps.Processors.Rating
{
    public abstract class RatingProcessor
    {
        /// <summary>
        ///     The difficulty rating of the map.
        /// </summary>
        public double DifficultyRating { get; }

         /// <summary>
         /// </summary>
         /// <param name="difficultyRating"></param>
        protected RatingProcessor(double difficultyRating) => DifficultyRating = difficultyRating;

        /// <summary>
        ///     Calculates the rating of a given score w/ raw accuracy
        /// </summary>
        /// <returns></returns>
        public abstract double CalculateRating(double accuracy);

        /// <summary>
        ///     Calculates the rating of a given score with a score processor.
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public abstract double CalculateRating(ScoreProcessor processor);
    }
}