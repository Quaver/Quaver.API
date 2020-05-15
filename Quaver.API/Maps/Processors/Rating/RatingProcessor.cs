/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

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
        public abstract double CalculateRating(double accuracy, bool failed = false);

        /// <summary>
        ///     Calculates the rating of a given score with a score processor.
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public abstract double CalculateRating(ScoreProcessor processor);

        /// <summary>
        ///     Calculates the accuracy required to achieve a given rating
        /// </summary>
        /// <param name="performanceRating"></param>
        /// <returns></returns>
        public abstract double GetAccuracyFromRating(double performanceRating);
    }
}