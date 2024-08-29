/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers;
using Quaver.Tools.Helpers;

namespace Quaver.Tools.Commands
{
    internal class CalcFolderCommand : Command
    {
        /// <summary>
        ///     The folder in which contains all the .qua or .osu files.
        /// </summary>
        public string BaseFolder { get; }

        /// <summary>
        ///     
        /// </summary>
        public ModIdentifier Mods { get; }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public CalcFolderCommand(string[] args) : base(args)
        {
            BaseFolder = args[1];
            Mods = (ModIdentifier)Enum.Parse(typeof(ModIdentifier), args[2]);
        }

        /// <summary> 
        /// </summary>
        public override void Execute()
        {
            var files = Directory.GetFiles(BaseFolder, "*.qua", SearchOption.AllDirectories).ToList();
            files.AddRange(Directory.GetFiles(BaseFolder, "*.osu", SearchOption.AllDirectories));

            var calculatedMaps = new List<Tuple<int, string, string>>();

            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];

                try
                {
                    Qua map = null;

                    if (file.EndsWith(".qua"))
                        map = Qua.Parse(file);
                    else if (file.EndsWith(".osu"))
                        map = new OsuBeatmap(file).ToQua();

                    if (map == null)
                        continue;

                    var diffCalc = map.SolveDifficulty();

                    Console.WriteLine($"[{i}] | {map} | {diffCalc.OverallDifficulty}");
                    calculatedMaps.Add(Tuple.Create(i, map.ToString(), diffCalc.OverallDifficulty.ToString(CultureInfo.InvariantCulture)));
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            var table = calculatedMaps.ToStringTable(new[] { "Id", "Map", "Difficulty" }, a => a.Item1, a => a.Item2, a => a.Item3);
            Console.WriteLine(table);

            File.WriteAllText("./diff-calc.txt", table);
        }
    }
}
