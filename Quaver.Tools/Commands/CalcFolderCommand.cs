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
            var output = "";
            var error = "";
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

                    Console.WriteLine($"[{i}/{files.Count}] | {map} | {diffCalc.OverallDifficulty}");
                    output += $"{map.Artist} - {map.Title} [{map.DifficultyName}]\t{diffCalc.OverallDifficulty}\n";
                    calculatedMaps.Add(Tuple.Create(i, map.ToString(), diffCalc.OverallDifficulty.ToString(CultureInfo.InvariantCulture)));
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            //Console.WriteLine(table);

            File.WriteAllText($"{BaseFolder}/__diff-sheet.txt", output);
        }
    }
}
