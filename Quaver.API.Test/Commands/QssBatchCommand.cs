using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys;

namespace Quaver.API.Test.Commands
{
    public class QssBatchCommand : Command
    {
        /// <summary>
        ///     The folder in which contains all the .qua or .osu files.
        /// </summary>
        public string BaseFolder { get; }

        public QssBatchCommand(string[] args) : base(args) => BaseFolder = args[1];

        /// <summary>
        ///
        /// </summary>
        public override void Execute()
        {
            var files = Directory.GetFiles(BaseFolder, "*.qua", SearchOption.AllDirectories).ToList();
            files.AddRange(Directory.GetFiles(BaseFolder, "*.osu", SearchOption.AllDirectories));

            foreach (var file in files)
            {
                try
                {
                    Qua map = null;

                    if (file.EndsWith(".qua"))
                        map = Qua.Parse(file);
                    else if (file.EndsWith(".osu"))
                        map = new OsuBeatmap(file).ToQua();

                    if (map == null)
                        continue;;

                    var diffCalc = map.SolveDifficulty();
                    Console.WriteLine($"{map.ToString()} - {diffCalc.OverallDifficulty}");
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}