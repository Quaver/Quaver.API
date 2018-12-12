using System;
using Quaver.API.Maps.Processors.Scoring;

namespace Quaver.API.Maps.Processors.Rating
{
    public class RatingProcessorKeys : RatingProcessor
    {
        public RatingProcessorKeys(double difficultyRating) : base(difficultyRating)
        {
        }

        public override double CalculateRating(double accuracy) => DifficultyRating * Math.Pow(accuracy / 98, 6);

        public override double CalculateRating(ScoreProcessor processor) => DifficultyRating * Math.Pow(processor.Accuracy / 98, 6);
    }
}
