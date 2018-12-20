/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using Newtonsoft.Json.Linq;
using Quaver.API.Maps.Processors.Rating;

namespace Quaver.Tools.Commands
{
    public class CalcRatingCommand : Command
    {
        public double DifficultyRating { get; }

        public double Accuracy { get; }

        public CalcRatingCommand(string[] args) : base(args)
        {
            DifficultyRating = double.Parse(args[1]);
            Accuracy = double.Parse(args[2]);
        }

        public override void Execute() => Console.WriteLine(JObject.FromObject(new
        {
            RatingProcessorKeys.Version,
            Rating = new RatingProcessorKeys(DifficultyRating).CalculateRating(Accuracy)
        }));
    }
}