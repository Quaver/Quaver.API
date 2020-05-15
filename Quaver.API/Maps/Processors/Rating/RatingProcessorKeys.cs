/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using Quaver.API.Maps.Processors.Scoring;

namespace Quaver.API.Maps.Processors.Rating
{
    public class RatingProcessorKeys : RatingProcessor
    {
        /// <summary>
        ///     The version of the processor.
        /// </summary>
        public static string Version { get; } = "0.0.1";

        public RatingProcessorKeys(double difficultyRating) : base(difficultyRating)
        {
        }

        public override double CalculateRating(double accuracy, bool failed = false)
        {
            if (failed)
                return 0;

            return DifficultyRating * Math.Pow(accuracy / 98, 6);
        }

        public override double CalculateRating(ScoreProcessor processor)
        {
            if (processor.Failed)
                return 0;

            return DifficultyRating * Math.Pow(processor.Accuracy / 98, 6);
        }

        public override double GetAccuracyFromRating(double performanceRating) => 98 * Math.Pow(performanceRating / DifficultyRating, 1 / 6f);
    }
}
