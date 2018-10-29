using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers;

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
                Map = Qua.Parse(path);
            else if (path.EndsWith(".osu"))
                Map = new OsuBeatmap(path).ToQua();

            Mods = (ModIdentifier) Enum.Parse(typeof(ModIdentifier), args[2]);
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
                    difficulty.OverallDifficulty
                }
            }));
        }
    }
}
