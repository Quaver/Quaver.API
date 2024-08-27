/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys;

namespace Quaver.Tools.Commands
{
    internal class CalcDiffCommand : Command
    {
        /// <summary>
        ///     The map that is having its difficulty calculated
        /// </summary>
        public Qua Map { get; }

        /// <summary>
        ///     The mods used when calculating the difficulty.
        /// </summary>
        public ModIdentifier Mods { get; }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public CalcDiffCommand(string[] args) : base(args)
        {
            var path = args[1];

            if (path.EndsWith(".qua"))
                Map = Qua.Parse(path, false);
            else if (path.EndsWith(".osu"))
                Map = new OsuBeatmap(path).ToQua();

            Mods = (ModIdentifier)Enum.Parse(typeof(ModIdentifier), args[2]);
            Map.ApplyMods(Mods);
        }

        /// <summary>
        /// </summary>
        public override void Execute()
        {
            var difficulty = Map.SolveDifficulty(Mods);

            Console.WriteLine(JObject.FromObject(new
            {
                Metadata = new
                {
                    Map.Artist,
                    Map.Title,
                    Map.DifficultyName,
                    Map.Creator,
                    Map.Mode,
                    Map.Length,
                    Map.MapId,
                    Map.MapSetId,
                    ObjectCount = Map.HitObjects.Count
                },
                Difficulty = new
                {
                    difficulty.OverallDifficulty,
                    DifficultyProcessorKeys.Version
                }
            }));
        }
    }
}
