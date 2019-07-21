/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

namespace Quaver.API.Maps.Processors.Scoring.Data
{
    /// <summary>
    ///     Structure that contains various hit statistics.
    /// </summary>
    public struct HitStatistics
    {
        /// <summary>
        ///     Average hit difference.
        /// </summary>
        public double Mean;

        /// <summary>
        ///     Standard deviation of hit differences.
        /// </summary>
        public double StandardDeviation;
    }
}
